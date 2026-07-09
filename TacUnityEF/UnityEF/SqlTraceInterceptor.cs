// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using Tac;
using Tac.Sql;

namespace UnityEF
{
	public class SqlTraceInterceptor : DbCommandInterceptor
	{
		private string logDeclare = "sql_declare.log";
		private string logTrace = "sql_trace.log";

		private Dictionary<string, int> SqlToId = new Dictionary<string, int>();
		private Dictionary<int, string> IdToSql = new Dictionary<int, string>();
		private DataTableConverter dtConverter = new DataTableConverter();
		private DataTable endResult;
		private int endRecordsAffected;

		private int NextCommandId = 0;

		private bool _serviceChecked = false;

		public bool SendMode = true;
		public DebugType DebugType = DebugType.InfoSql;
		public bool LoadMode;

		public SqlTraceInterceptor()
		{
		}

		private void EnsureService()
		{
			if (!_serviceChecked)
			{
				_serviceChecked = true;
				ServiceManager.EnsureServiceInstalledAndRunning();

				if (LoadMode == false)
				{
					NewSave save = new NewSave
					{
						MessageType = MessageType.NewSave,
						SaveName = "autosave"
					};
					Message response = SendAndReceive(save);
				}
				else
				{
					LoadSave load = new LoadSave
					{
						MessageType = MessageType.LoadSave,
						SaveName = "autosave"
					};
					Message response = SendAndReceive(load);
				}

			}
		}

		public int GetSqlId(string operation)
		{
			int ret = 0;
			if (SqlToId.ContainsKey(operation) == false)
			{
				NextCommandId++;

				SqlToId.Add(operation, NextCommandId);
				IdToSql.Add(NextCommandId, operation);
				ret = NextCommandId;

				if (DebugType != DebugType.None)
				{
					File.AppendAllText(logDeclare, "CommandId = " + NextCommandId.ToString() + "\n");
					File.AppendAllText(logDeclare, operation + "\n");
				}
			}
			else
			{ 
				ret = SqlToId[operation];
			}
			return ret;
		}

		private void Write(DbCommand command, string operation)
		{
			if (SendMode)
			{
				Send(command, operation);
			}
			else
			{
				var sb = new StringBuilder();

				int id = GetSqlId(command.CommandText);

				sb.AppendLine(operation + " " + id.ToString());

				if (command.Parameters.Count > 0)
				{
					foreach (DbParameter p in command.Parameters)
					{
						sb.Append("    ");
						sb.Append(p.ParameterName);
						sb.Append(" = ");

						sb.Append(p.Value == DBNull.Value ? "NULL" : p.Value);

						sb.Append(" (");
						sb.Append(p.DbType);
						sb.AppendLine(")");
					}
				}
				sb.AppendLine();

				File.AppendAllText(logTrace, sb.ToString());
			}
		}

		private LogCommand BuildLogData(DbCommand command, string operation)
		{
			var log = new LogCommand
			{
				Operation = operation,
				CommandId = GetSqlId(command.CommandText),
				CommandText = command.CommandText
			};

			if (command.Parameters.Count > 0)
			{
				log.Parameters = new List<ParameterData>();
				foreach (DbParameter p in command.Parameters)
				{
					log.Parameters.Add(new ParameterData
					{
						Name = p.ParameterName,
						Value = GetParameterValue(p),
						DbType = p.DbType.ToString()
					});
				}
			}
			return log;
		}

		private string GetParameterValue(DbParameter parameter)
		{
			// Обработка null и DBNull (исходное поведение)
			if (parameter.Value == DBNull.Value || parameter.Value == null)
				return "NULL";

			// Всегда используем инвариантную культуру для получения строки
			string stringValue = Convert.ToString(parameter.Value, CultureInfo.InvariantCulture);

			// Если полученная строка — "NaN", "Infinity" или аналоги, заменяем на "0"
			if (IsNonNumericString(stringValue))
				return "0";

			return stringValue;
		}

		private bool IsNonNumericString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return false;

			string trimmed = value.Trim();
			string lower = trimmed.ToLowerInvariant();

			return lower == "nan" || lower == "infinity" || lower == "-infinity" || lower == "+infinity";
		}

		private void Send(DbCommand command, string operation)
		{
			// Первая проверка сервиса
			EnsureService();

			LogCommand log = BuildLogData(command, operation);
			LogDataTable result = SendAndReceive(log) as LogDataTable;

			if (result != null)
			{
				endResult = dtConverter.Restore(result);
				endRecordsAffected = result.RecordsAffected;
			}
			else { endResult = null; endRecordsAffected = -1; }

			if (DebugType != DebugType.None)
			{
				var sb = new StringBuilder();
				sb.AppendLine($"{operation} {log.CommandId}");
				if (log.Parameters != null)
					foreach (var p in log.Parameters)
						sb.AppendLine($"    {p.Name} = {p.Value} ({p.DbType})");
				sb.AppendLine();
				File.AppendAllText(logTrace, sb.ToString());
			}
		}


		private void WriteException(Exception ex)
		{
			File.AppendAllText(logTrace, ex + Environment.NewLine + Environment.NewLine);
		}

		public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
		{
			Write(command, "ExecuteReader");
			InterceptionResult<DbDataReader> ret;
			if (endResult != null)
			{
				DbDataReader dtReader = endResult.CreateDataReader();
				DbDataReaderExt dbDataReaderExt = new DbDataReaderExt(dtReader, endRecordsAffected);
				ret = InterceptionResult<DbDataReader>.SuppressWithResult(dbDataReaderExt);
			}
			else 
			{
				var emptyTable = new DataTable();
				ret = InterceptionResult<DbDataReader>.SuppressWithResult(emptyTable.CreateDataReader());
			}
			return ret;
		}

		public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
		{
			//WriteExecuted(eventData.Duration);
			return result;
		}

		public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
		{
			Write(command, "ExecuteNonQuery");
			return InterceptionResult<int>.SuppressWithResult(0);
		}

		public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
		{
			//WriteExecuted(eventData.Duration);
			return result;
		}

		public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
		{
			WriteException(eventData.Exception);
		}


		/*private Message SendAndReceive(LogCommand log)
		{
			try
			{
				Message retMessage = null;
				using var client = new NamedPipeClientStream(".", PipeConstants.PipeName, PipeDirection.InOut);
				client.Connect(500); // таймаут 500 мс

				StreamWriter writer = new StreamWriter(client, Encoding.UTF8, 4096, true);
				string json = JsonSerializer.Serialize(log);
				writer.WriteLine(json);
				writer.Flush();

				// Читаем ответ от службы
				StreamReader reader = new StreamReader(client, Encoding.UTF8, false, 4096, true);
				string responseJson = reader.ReadLine();
				if (string.IsNullOrEmpty(responseJson) == false)
				{
					retMessage = Message.Deserialize(responseJson);
					File.AppendAllText("response.log", responseJson + "\n");
				}
				return retMessage;
			}
			catch (Exception ex)
			{
				// Логируем ошибку (можно использовать ILogger)
				File.AppendAllText("error.log", ex.Message + "\n" + ex.StackTrace + "\n");
				return null;
			}
		}*/

		private Message SendAndReceive(Message request)
		{
			try
			{
				Message retMessage = null;
				using var client = new NamedPipeClientStream(".", PipeConstants.PipeName, PipeDirection.InOut);
				client.Connect(500);

				// Отправка запроса
				using (var writer = new StreamWriter(client, Encoding.UTF8, 4096, true))
				{
					string json = Message.Serialize(request);
					writer.WriteLine(json);
					writer.Flush();
				}

				// Чтение ответа
				using (var reader = new StreamReader(client, Encoding.UTF8, false, 4096, true))
				{
					string responseJson = reader.ReadLine();
					if (string.IsNullOrEmpty(responseJson) == false)
					{
						if (DebugType != DebugType.None)
						{
							File.AppendAllText("response.log", responseJson + "\n");
						}

						retMessage = Message.Deserialize(responseJson);
					}
				}
				return retMessage;
			}
			catch (Exception ex)
			{
				File.AppendAllText("error.log", ex.Message + "\n" + ex.StackTrace + "\n");
				return new Message { Info = $"ERROR: {ex.Message}" }; 
			}
		}


	}
}
