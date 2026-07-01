// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using Tac.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

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
	}
}
