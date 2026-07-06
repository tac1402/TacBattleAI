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
		private readonly string logFileReceive = "sql_trace_receive.log";
		private readonly string logFileError = "sql_trace_error.log";
		private string connectionString;
		private string fullPathBefor;
		private string fullPathAfter;
		private string fullPathError;
		private string fullPathReceive;
		private LiteToServer liteToServer;
		private DataTableConverter dtConverter = new DataTableConverter();
		private DebugType debugType;


		public SqlService(ILogger<SqlService> argLogger, IConfiguration configuration)
		{
			logger = argLogger;
			connectionString = configuration.GetConnectionString("SqlConnection");

			if (Enum.TryParse<DebugType>(configuration["Debug:DebugType"], ignoreCase: true, out var parsedDebugType))
			{
				debugType = parsedDebugType;
			}
			else
			{
				debugType = DebugType.None;
			}

			Directory.CreateDirectory(logDirectory);
			fullPathBefor = Path.Combine(logDirectory, logFileBefor);
			fullPathAfter = Path.Combine(logDirectory, logFileAfter);
			fullPathError = Path.Combine(logDirectory, logFileError);
			fullPathReceive = Path.Combine(logDirectory, logFileReceive);
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
				var reader = new StreamReader(server, Encoding.UTF8, leaveOpen: true);

				// Читаем команды, пока клиент подключён
				while (true)
				{
					if (ct.IsCancellationRequested)
						break;

					string message = await reader.ReadLineAsync();
					if (message == null) // клиент закрыл соединение
						break;

					if (string.IsNullOrEmpty(message) == false)
					{
						Message msg = Message.Deserialize(message);

						if (msg != null)
						{
							if (debugType != DebugType.None)
							{
								await WriteLogAsync(msg, fullPathBefor);
							}

							string response = "";
							switch (msg)
							{
								case LogCommand log:
									response = await ExecuteSqlAsync(log);
									break;

								case NewSave save:
									response = await ProcessSaveAsync(save.SaveName);
									response = Message.Serialize(response);
									break;

								default:
									response = Message.Serialize("ERROR: Unsupported message type");
									break;
							}

							var writer = new StreamWriter(server, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
							await writer.WriteLineAsync(response);
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


		private async Task WriteLogAsync(Message message, string filePath)
		{
			string entry;
			switch (message)
			{
				case LogCommand log:
					entry = $"[{log.Operation}] ID={log.CommandId}\n" +
							$"{log.CommandText}\n";
					if (log.Parameters != null && log.Parameters.Count > 0)
					{
						foreach (var p in log.Parameters)
							entry += $"    {p.Name} = {p.Value} ({p.DbType})\n";
					}
					entry += new string('-', 80) + "\n";
					break;

				case NewSave save:
					entry = $"[NewSave] SaveName={save.SaveName}\n" +
							$"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
							new string('-', 80) + "\n";
					break;

				default:
					entry = $"[Unknown] {message.GetType().Name}\n" +
							new string('-', 80) + "\n";
					break;
			}

			await File.AppendAllTextAsync(filePath, entry);
		}

		private async Task<string> ProcessSaveAsync(string saveName)
		{
			try
			{
				var builder = new SqlConnectionStringBuilder(connectionString);
				string originalDatabase = builder.InitialCatalog;
				string newDatabase = $"{originalDatabase}_{saveName}";

				// Подключение к master
				builder.InitialCatalog = "master";
				string masterConnectionString = builder.ConnectionString;

				using (var masterConn = new SqlConnection(masterConnectionString))
				{
					await masterConn.OpenAsync();

					// Проверяем, существует ли база
					string checkCommand = $"SELECT COUNT(*) FROM sys.databases WHERE name = N'{newDatabase}'";
					using (var checkCmd = new SqlCommand(checkCommand, masterConn))
					{
						int exists = (int)await checkCmd.ExecuteScalarAsync();
						if (exists > 0)
						{
							// Отключаем AUTO_CLOSE перед удалением (избегаем ошибки 615)
							string alterCommand = $"ALTER DATABASE [{newDatabase}] SET AUTO_CLOSE OFF";
							using (var alterCmd = new SqlCommand(alterCommand, masterConn))
							{
								await alterCmd.ExecuteNonQueryAsync();
							}

							// Удаляем базу
							string dropCommand = $"DROP DATABASE [{newDatabase}]";
							using (var dropCmd = new SqlCommand(dropCommand, masterConn))
							{
								await dropCmd.ExecuteNonQueryAsync();
							}
						}
					}

					// Создаём новую базу
					string createCommand = $"CREATE DATABASE [{newDatabase}]";
					using (var createCmd = new SqlCommand(createCommand, masterConn))
					{
						await createCmd.ExecuteNonQueryAsync();
					}

					// После создания базы очищаем пул соединений
					SqlConnection.ClearAllPools();

					// ---- ОБНОВЛЯЕМ СТРОКУ ПОДКЛЮЧЕНИЯ ----
					builder.InitialCatalog = newDatabase; 
					connectionString = builder.ConnectionString;
				}

				return $"OK: Database '{newDatabase}' created (previous dropped if existed)";
			}
			catch (Exception ex)
			{
				await File.AppendAllTextAsync(fullPathError, $"[SaveError] {ex.Message}\n{ex.StackTrace}");
				return $"ERROR: {ex.Message}";
			}
		}

		private async Task<string> ExecuteSqlAsync(LogCommand log)
		{
			try
			{
				using SqlConnection connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				liteToServer.Convert(log);

				if (debugType != DebugType.None)
				{
					await WriteLogAsync(log, fullPathAfter);
				}

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
					DataTable dataTable = new DataTable();
					dataTable.Load(reader); // загружаем все строки

					// Преобразуем DataTable в LogDataTable
					LogDataTable logTable = dtConverter.Convert(dataTable);
					logTable.RecordsAffected = reader.RecordsAffected;
					// Сериализуем в JSON
					string json = Message.Serialize(logTable);

					if (json != "" && debugType != DebugType.None)
					{
						await File.AppendAllTextAsync(fullPathReceive, json + "\n");
					}

					return json;
				}
				else if (log.Operation.Contains("NonQuery"))
				{
					int affected = await cmd.ExecuteNonQueryAsync();
					return string.Empty;
				}
				else if (log.Operation.Contains("Scalar"))
				{
					object result = await cmd.ExecuteScalarAsync();
					return string.Empty;
				}
				else
				{
					logger.LogWarning($"Неизвестная операция: {log.Operation}. Выполняем ExecuteNonQuery по умолчанию.");
					await cmd.ExecuteNonQueryAsync();
					return string.Empty;
				}
			}
			catch (Exception ex)
			{
				await File.AppendAllTextAsync(fullPathError, ex.Message + "\n" + ex.StackTrace);
				return string.Empty;
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
