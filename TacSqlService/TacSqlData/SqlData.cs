// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Tac.Sql
{
	public enum MessageType
	{
		Message = 0,
		NewSave = 1,
		LoadSave = 2,
		LogCommand = 3,
		LogDataTable = 4
	}

	[Serializable]
	public class Message
	{
		public MessageType MessageType { get; set; }
		public string Info { get; set; }

		public static string Serialize(string message)
		{
			Message m = new Message();
			m.MessageType = MessageType.Message;
			m.Info = message;
			return JsonSerializer.Serialize<Message>(m);
		}

		public static string Serialize(Message message)
		{
			switch (message)
			{
				case NewSave ns:
					ns.MessageType = MessageType.NewSave;
					return JsonSerializer.Serialize<NewSave>(ns);
				case LoadSave ls:
					ls.MessageType = MessageType.LoadSave;
					return JsonSerializer.Serialize<LoadSave>(ls);
				case LogCommand lc:
					lc.MessageType = MessageType.LogCommand;
					return JsonSerializer.Serialize<LogCommand>(lc);
				case LogDataTable ld:
					ld.MessageType = MessageType.LogDataTable;
					return JsonSerializer.Serialize<LogDataTable>(ld);
				case Message m:
					m.MessageType = MessageType.Message;
					return JsonSerializer.Serialize<Message>(m);
				default:
					throw new NotSupportedException($"Unsupported type: {message.GetType()}");
			}
		}
		public static Message Deserialize(string json)
		{
			using var doc = JsonDocument.Parse(json);
			if (!doc.RootElement.TryGetProperty("MessageType", out var typeElement))
				throw new InvalidOperationException("MessageType field is missing");

			int typeNumber = typeElement.GetInt32();
			MessageType messageType = (MessageType)typeNumber;

			// Десериализуем в конкретный тип в зависимости от messageType
			return messageType switch
			{
				MessageType.Message => JsonSerializer.Deserialize<Message>(json),
				MessageType.NewSave => JsonSerializer.Deserialize<NewSave>(json),
				MessageType.LoadSave => JsonSerializer.Deserialize<LoadSave>(json),
				MessageType.LogCommand => JsonSerializer.Deserialize<LogCommand>(json),
				MessageType.LogDataTable => JsonSerializer.Deserialize<LogDataTable>(json),
				_ => throw new NotSupportedException($"Unsupported MessageType: {messageType}")
			};
		}
	}

	[Serializable]
	public class NewSave : Message
	{
		public string SaveName { get; set; }
	}

	[Serializable]
	public class LoadSave : Message
	{
		public string SaveName { get; set; }
	}

	[Serializable]
	public class LogCommand : Message
	{
		public string Operation { get; set; }
		public int CommandId { get; set; }
		public string CommandText { get; set; }
		public List<ParameterData> Parameters { get; set; }
	}

	[Serializable]
	public class ParameterData
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public string DbType { get; set; }
	}


	[Serializable]
	public class LogDataTable : Message
	{
		public List<ColumnInfo> Columns { get; set; }
		public List<List<string?>> Rows { get; set; }
		public int RecordsAffected { get; set; } = -1; // По умолчанию -1
	}

	[Serializable]
	public class ColumnInfo
	{
		public string Name { get; set; }
		public string TypeName { get; set; }
	}


	// PipeConstants.cs
	public static class PipeConstants
	{
		public const string PipeName = "SqlDataPipe";
		public const string ServiceName = "TacSqlService";
		public const string ServicePathInstall = @"P:\TacSqlService\Install";
	}

}
