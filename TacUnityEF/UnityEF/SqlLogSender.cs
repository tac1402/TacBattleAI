// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tac.Sql;

namespace UnityEF
{
	public static class SqlLogSender
	{
		public static bool SendLog(LogData log)
		{
			//if (!ServiceAvailabilityCache.IsAvailable())
			//	return false;

			try
			{
				using var client = new NamedPipeClientStream(".", PipeConstants.PipeName, PipeDirection.InOut);
				client.Connect(500);
				using var writer = new StreamWriter(client, Encoding.UTF8);
				string json = JsonSerializer.Serialize(log);
				writer.WriteLine(json);
				writer.Flush();
				return true;
			}
			catch(Exception ex) 
			{
				ServiceAvailabilityCache.SetUnavailable();
				return false;
			}
		}

		public static LogDataTable SendAndReceive(LogData log)
		{
			try
			{
				LogDataTable retTable = null;
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
					retTable = JsonSerializer.Deserialize<LogDataTable>(responseJson);
					File.AppendAllText("response.log", responseJson + "\n");
				}
				return retTable;
			}
			catch (Exception ex)
			{
				// Логируем ошибку (можно использовать ILogger)
				File.AppendAllText("error.log", ex.Message + "\n" + ex.StackTrace + "\n");
				return null;
			}
		}

	}
}
