// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Data;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Tac.Sql
{
	public class SqlService : BackgroundService
	{
		private readonly ILogger<SqlService> logger;
		private readonly string logDirectory = @"P:\SqlLogs";
		private readonly string logFileBefor = "sql_trace_befor.log";
		private readonly string logFileAfter = "sql_trace_after.log";
		private readonly string logFileError = "sql_trace_error.log";
		private readonly string connectionString;
		private string fullPathBefor;
		private string fullPathAfter;
		private string fullPathError;
		private LiteToServer liteToServer;

		public SqlService(ILogger<SqlService> argLogger, IConfiguration configuration)
		{
			logger = argLogger;
			connectionString = configuration.GetConnectionString("SqlConnection");
			Directory.CreateDirectory(logDirectory);
			fullPathBefor = Path.Combine(logDirectory, logFileBefor);
			fullPathAfter = Path.Combine(logDirectory, logFileAfter);
			fullPathError = Path.Combine(logDirectory, logFileError);
			liteToServer = new LiteToServer();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			logger.LogInformation("Сервис логов SQL запущен.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					// Создаём правило доступа: разрешить всем (Everyone) чтение/запись
					var security = new PipeSecurity();
					var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
					security.AddAccessRule(new PipeAccessRule(everyone, PipeAccessRights.ReadWrite, AccessControlType.Allow));

					// Создаём канал с настройками безопасности
					using var server = NamedPipeServerStreamAcl.Create(
						PipeConstants.PipeName,
						PipeDirection.InOut,
						NamedPipeServerStream.MaxAllowedServerInstances,
						PipeTransmissionMode.Message,
						PipeOptions.Asynchronous,
						0, 0, security);

					logger.LogInformation("Ожидание подключения клиента...");
					await server.WaitForConnectionAsync(stoppingToken);
					logger.LogInformation("Клиент подключён.");
					await ProcessClientAsync(server, stoppingToken);
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					await File.AppendAllTextAsync(fullPathError, ex.Message + "\n" + ex.StackTrace);
					await Task.Delay(1000, stoppingToken);
				}
			}
		}

		private async Task ProcessClientAsync(NamedPipeServerStream server, CancellationToken ct)
		{
			try
			{
				using var reader = new StreamReader(server, Encoding.UTF8);

				// Читаем команды, пока клиент подключён
				while (true)
				{
					if (ct.IsCancellationRequested)
						break;

					string message = await reader.ReadLineAsync();
					if (message == null) // клиент закрыл соединение
						break;

					if (!string.IsNullOrEmpty(message))
					{
						var log = JsonSerializer.Deserialize<LogData>(message);
						if (log != null)
						{
							await WriteLogAsync(log, fullPathBefor);
							await ExecuteSqlAsync(log);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Логируем ошибку (но не прерываем работу сервера)
				await File.AppendAllTextAsync(fullPathError, ex.Message + "\n" + ex.StackTrace);
			}
			finally
			{
				// Безопасное закрытие соединения
				try
				{
					if (server.IsConnected)
						server.Disconnect();
				}
				catch (InvalidOperationException) { /* труба уже закрыта */ }
				catch (IOException) { /* ошибка ввода-вывода */ }
				finally
				{
					server.Close();
					server.Dispose();
				}
			}
		}

		private async Task WriteLogAsync(LogData log, string argFullPath)
		{
			string entry = $"[{log.Operation}] ID={log.CommandId}\n" +
						   $"{log.CommandText}\n";
			if (log.Parameters != null && log.Parameters.Count > 0)
			{
				foreach (var p in log.Parameters)
					entry += $"    {p.Name} = {p.Value} ({p.DbType})\n";
			}
			entry += new string('-', 80) + "\n";

			await File.AppendAllTextAsync(argFullPath, entry);
		}

		private async Task ExecuteSqlAsync(LogData log)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				logger.LogWarning("Строка подключения не задана, выполнение SQL невозможно.");
				return;
			}

			try
			{
				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				liteToServer.Convert(log);

				WriteLogAsync(log, fullPathAfter);

				using var cmd = new SqlCommand(log.CommandText, connection);
				cmd.CommandType = CommandType.Text;

				if (log.Parameters != null)
				{
					foreach (var p in log.Parameters)
					{
						var param = new SqlParameter(p.Name, ConvertToSqlDbType(p.DbType))
						{
							Value = p.Value == "NULL" ? DBNull.Value : (object)p.Value
						};
						cmd.Parameters.Add(param);
					}
				}

				if (log.Operation.Contains("Reader"))
				{
					using var reader = await cmd.ExecuteReaderAsync();
					// Можно также прочитать данные, но мы просто логируем факт выполнения
					logger.LogInformation($"ExecuteReader выполнен. Затронуто строк: {reader.RecordsAffected}");
				}
				else if (log.Operation.Contains("NonQuery"))
				{
					int affected = await cmd.ExecuteNonQueryAsync();
					logger.LogInformation($"ExecuteNonQuery выполнен. Затронуто строк: {affected}");
				}
				else if (log.Operation.Contains("Scalar"))
				{
					object result = await cmd.ExecuteScalarAsync();
					logger.LogInformation($"ExecuteScalar выполнен. Результат: {result}");
				}
				else
				{
					logger.LogWarning($"Неизвестная операция: {log.Operation}. Выполняем ExecuteNonQuery по умолчанию.");
					await cmd.ExecuteNonQueryAsync();
				}
			}
			catch (Exception ex)
			{
				await File.AppendAllTextAsync(fullPathError, ex.Message + "\n" + ex.StackTrace);
			}
		}

		private SqlDbType ConvertToSqlDbType(string dbType)
		{
			if (Enum.TryParse<SqlDbType>(dbType, true, out var result))
				return result;
			return SqlDbType.NVarChar;
		}


	}
}
