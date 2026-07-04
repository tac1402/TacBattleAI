// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tac.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Data;

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

		private int NextCommandId = 0;

		private bool _serviceChecked = false;

		private bool SendMode = true;

		public SqlTraceInterceptor()
		{
		}

		// Этот метод вызывается перед первой отправкой
		private void EnsureService()
		{
			if (!_serviceChecked)
			{
				_serviceChecked = true;
				ServiceManager.EnsureServiceInstalledAndRunning();
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
				File.AppendAllText(logDeclare, operation + "\n");
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

		private LogData BuildLogData(DbCommand command, string operation)
		{
			var log = new LogData
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
						Value = p.Value == DBNull.Value ? "NULL" : p.Value?.ToString(),
						DbType = p.DbType.ToString()
					});
				}
			}
			return log;
		}

		private void Send(DbCommand command, string operation)
		{
			// Первая проверка сервиса
			EnsureService();

			var log = BuildLogData(command, operation);
			//bool sent = SqlLogSender.SendLog(log);
			LogDataTable result = SqlLogSender.SendAndReceive(log);

			if (result != null)
			{
				endResult = dtConverter.Restore(result);
			}
			else { endResult = null; }

			/*if (!sent)
			{
				var sb = new StringBuilder();
				sb.AppendLine($"{operation} {log.CommandId}");
				if (log.Parameters != null)
					foreach (var p in log.Parameters)
						sb.AppendLine($"    {p.Name} = {p.Value} ({p.DbType})");
				sb.AppendLine();
				File.AppendAllText(logTrace, sb.ToString());
			}*/
		}




		private void WriteExecuted(TimeSpan duration)
		{
			//File.AppendAllText(	_logFile, $"Completed in {duration.TotalMilliseconds:F2} ms{Environment.NewLine}{Environment.NewLine}");
		}

		private void WriteException(Exception ex)
		{
			File.AppendAllText(logTrace, ex + Environment.NewLine + Environment.NewLine);
		}

		public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
		{
			Write(command, "ExecuteReader");
			return result;
		}

		public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
		{
			//WriteExecuted(eventData.Duration);
			DbDataReader ret = null;
			if (endResult != null)
			{
				ret = endResult.CreateDataReader();
			}
			else { ret = result; }

			return ret;
		}

		public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
		{
			Write(command, "ExecuteNonQuery");
			return result;
		}

		public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
		{
			WriteExecuted(eventData.Duration);
			return result;
		}

		public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
		{
			Write(command, "ExecuteScalar");
			return result;
		}

		public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object result)
		{
			WriteExecuted(eventData.Duration);
			return result;
		}

		public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
		{
			WriteException(eventData.Exception);
		}
	}
}
