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
				using var client = new NamedPipeClientStream(".", PipeConstants.PipeName, PipeDirection.InOut);
				client.Connect(500); // таймаут 500 мс

				using var writer = new StreamWriter(client, Encoding.UTF8);
				string json = JsonSerializer.Serialize(log);
				writer.WriteLine(json);
				writer.Flush();

				// Читаем ответ от службы
				using var reader = new StreamReader(client, Encoding.UTF8);
				string responseJson = reader.ReadLine();
				if (string.IsNullOrEmpty(responseJson))
					return null;

				return JsonSerializer.Deserialize<LogDataTable>(responseJson);
			}
			catch (Exception ex)
			{
				// Логируем ошибку (можно использовать ILogger)
				return null;
			}
		}

	}
}
