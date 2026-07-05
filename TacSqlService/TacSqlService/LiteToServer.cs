// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tac.Sql
{
	public class LiteToServer
	{
		private readonly List<string> _alterStatements = new();

		public void Convert(LogData log)
		{
			if (log == null || string.IsNullOrEmpty(log.CommandText))
				return;

			_alterStatements.Clear();
			log.CommandText = ToTSQL(log.CommandText);
			ConvertParameters(log);

			if (string.IsNullOrWhiteSpace(log.CommandText))
				log.CommandText = "-- empty command (PRAGMA removed)";
		}

		private void ConvertParameters(LogData log)
		{
			if (log?.Parameters == null) return;

			foreach (var param in log.Parameters)
			{
				if (param.Value is string strValue)
				{
					strValue = strValue.Trim();

					// Булевы значения → "0"/"1"
					if (strValue.Equals("True", StringComparison.OrdinalIgnoreCase))
					{
						param.Value = "1";
						continue;
					}
					if (strValue.Equals("False", StringComparison.OrdinalIgnoreCase))
					{
						param.Value = "0";
						continue;
					}

					// Заменяем запятую на точку (для чисел с плавающей точкой)
					param.Value = strValue.Replace(',', '.');
				}
			}
		}


		private string ToTSQL(string sqlCommand)
		{
			// 1. Удаляем PRAGMA
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"^\s*PRAGMA\s+[^;]*;?\s*$",
				"",
				RegexOptions.IgnoreCase | RegexOptions.Multiline
			);
			sqlCommand = Regex.Replace(sqlCommand, @"^\s*\r?\n", "", RegexOptions.Multiline);

			// 2. Заменяем кавычки
			sqlCommand = Regex.Replace(sqlCommand, @"""(?<id>[^""]+)""", "[${id}]");

			// 3. Преобразуем sqlite_master
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"SELECT\s+COUNT\(\*\)\s+FROM\s+\[sqlite_master\]\s+WHERE\s+\[type\]\s*=\s*'table'\s+AND\s+\[rootpage\]\s+IS\s+NOT\s+NULL\s*;?",
				"SELECT COUNT(*) FROM sys.tables WHERE type = 'U';",
				RegexOptions.IgnoreCase
			);
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"FROM\s+\[sqlite_master\]",
				"FROM sys.tables",
				RegexOptions.IgnoreCase
			);
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"\[\s*type\s*\]\s*=\s*'table'",
				"type = 'U'",
				RegexOptions.IgnoreCase
			);
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"\s+AND\s+\[\s*rootpage\s*\]\s+IS\s+NOT\s+NULL",
				"",
				RegexOptions.IgnoreCase
			);
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"\[\s*rootpage\s*\]\s+IS\s+NOT\s+NULL\s*(AND\s+)?",
				"",
				RegexOptions.IgnoreCase
			);

			// 4. Добавляем схему dbo
			sqlCommand = AddDboSchema(sqlCommand);

			// 5. Обрабатываем CREATE TABLE
			var pattern = @"CREATE\s+TABLE\s+(?<tableName>\[?[^\s\(]+\]?)\s*\((?<definition>.*)\)\s*;?";
			var matches = Regex.Matches(sqlCommand, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			foreach (Match match in matches)
			{
				var tableName = match.Groups["tableName"].Value;
				if (!tableName.Contains("."))
					tableName = "[dbo]." + tableName;

				var definition = match.Groups["definition"].Value;
				var newDefinition = ParseTableDefinition(definition, tableName);
				var newCreate = $"CREATE TABLE {tableName} ({newDefinition});";
				sqlCommand = sqlCommand.Replace(match.Value, newCreate);
			}

			// 6. Добавляем все ALTER (PRIMARY KEY + FOREIGN KEY)
			if (_alterStatements.Count > 0)
				sqlCommand += Environment.NewLine + string.Join(Environment.NewLine, _alterStatements);

			// 7. Заменяем INTEGER на INT (оставшиеся)
			sqlCommand = Regex.Replace(sqlCommand, @"\bINTEGER\b", "INT", RegexOptions.IgnoreCase);

			// 8. Заменяем TEXT на NVARCHAR(450) – подходит для индексов и ключей
			sqlCommand = Regex.Replace(sqlCommand, @"\bTEXT\b", "NVARCHAR(450)", RegexOptions.IgnoreCase);

			// 9. LIMIT → TOP
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"(?<=SELECT\s)(.*?)(?=\s+LIMIT\s+(\d+)\s*;?)",
				"TOP $2 $1",
				RegexOptions.IgnoreCase | RegexOptions.Singleline
			);
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"\s+LIMIT\s+\d+\s*;?",
				"",
				RegexOptions.IgnoreCase
			);

			// 10. Заменяем SQLite-функцию changes() на @@ROWCOUNT
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"\bchanges\s*\(\s*\)",
				"@@ROWCOUNT",
				RegexOptions.IgnoreCase
			);

			// 11. Заменяем last_insert_rowid() на SCOPE_IDENTITY()
			// Шаблон: SELECT [Id] FROM [dbo].[Table] WHERE @@ROWCOUNT = 1 AND [rowid] = last_insert_rowid();
			sqlCommand = Regex.Replace(
				sqlCommand,
				@"SELECT\s+\[(?<column>\w+)\]\s+FROM\s+\[dbo\]\.\[\w+\]\s+WHERE\s+@@ROWCOUNT\s*=\s*1\s+AND\s+\[rowid\]\s*=\s*last_insert_rowid\s*\(\s*\)\s*;?",
				"SELECT CAST(SCOPE_IDENTITY() AS INT) AS [${column}];",
				RegexOptions.IgnoreCase
			);

			// 12. Обработка IDENTITY_INSERT для INSERT с явным указанием Id
			sqlCommand = WrapIdentityInsert(sqlCommand);

			return sqlCommand.Trim();
		}

		// Добавляет SET IDENTITY_INSERT ON/OFF для INSERT с явным Id
		private string WrapIdentityInsert(string sql)
		{
			// Паттерн: INSERT INTO [dbo].[Table] ( ... Id ... ) VALUES ( ... )
			var pattern = @"INSERT\s+INTO\s+(?<table>\[dbo\]\.\[\w+\])\s*\((?<columns>[^)]+)\)\s+VALUES\s*\((?<values>[^)]+)\)";
			return Regex.Replace(sql, pattern, m =>
			{
				string table = m.Groups["table"].Value;
				string columns = m.Groups["columns"].Value;
				string values = m.Groups["values"].Value;

				// Проверяем, есть ли в колонках "Id" (с учётом пробелов)
				bool hasId = Regex.IsMatch(columns, @"\bId\b", RegexOptions.IgnoreCase);
				if (!hasId)
					return m.Value;

				// Проверяем, что в VALUES не NULL и не DEFAULT (можно пропустить, но для безопасности)
				// Если есть явное значение, оборачиваем
				return $@"
SET IDENTITY_INSERT {table} ON;
{m.Value};
SET IDENTITY_INSERT {table} OFF;";
			}, RegexOptions.IgnoreCase | RegexOptions.Singleline);
		}

		private string AddDboSchema(string sql)
		{
			string pattern = @"\b(FROM|JOIN|UPDATE|INTO|TABLE|DELETE\s+FROM|INSERT\s+INTO)\s+\[([^\]]+)\]";
			return Regex.Replace(sql, pattern, m =>
			{
				string keyword = m.Groups[1].Value;
				string tableName = m.Groups[2].Value;
				if (tableName.Contains(".") || tableName.StartsWith("sys.", StringComparison.OrdinalIgnoreCase))
					return m.Value;
				return $"{keyword} [dbo].[{tableName}]";
			}, RegexOptions.IgnoreCase);
		}

		private string ParseTableDefinition(string definition, string tableName)
		{
			var elements = SplitDefinition(definition);
			var newElements = new List<string>();

			foreach (var element in elements)
			{
				// FOREIGN KEY
				if (Regex.IsMatch(element, @"FOREIGN\s+KEY", RegexOptions.IgnoreCase))
				{
					var fkPattern = @"CONSTRAINT\s+(?<constraint>\[?[a-zA-Z0-9_]+\]?)\s+FOREIGN\s+KEY\s*\((?<columns>[^\)]+)\)\s+REFERENCES\s+(?<refTable>\[?[a-zA-Z0-9_]+\]?)\s*\((?<refColumns>[^\)]+)\)\s*(?:ON\s+DELETE\s+(?<onDelete>RESTRICT|CASCADE|SET\s+NULL|SET\s+DEFAULT|NO\s+ACTION))?\s*(?:ON\s+UPDATE\s+(?<onUpdate>RESTRICT|CASCADE|SET\s+NULL|SET\s+DEFAULT|NO\s+ACTION))?";
					var m = Regex.Match(element, fkPattern, RegexOptions.IgnoreCase);
					if (m.Success)
					{
						string constraintName = m.Groups["constraint"].Value;
						string columns = m.Groups["columns"].Value;
						string refTable = m.Groups["refTable"].Value;
						string refColumns = m.Groups["refColumns"].Value;
						string onDelete = m.Groups["onDelete"].Value;
						string onUpdate = m.Groups["onUpdate"].Value;

						if (onDelete.Equals("RESTRICT", StringComparison.OrdinalIgnoreCase))
							onDelete = "NO ACTION";
						if (onUpdate.Equals("RESTRICT", StringComparison.OrdinalIgnoreCase))
							onUpdate = "NO ACTION";

						if (!refTable.Contains("."))
							refTable = "[dbo]." + refTable;

						string alter = $"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} FOREIGN KEY ({columns}) REFERENCES {refTable} ({refColumns})";
						if (!string.IsNullOrEmpty(onDelete))
							alter += $" ON DELETE {onDelete}";
						if (!string.IsNullOrEmpty(onUpdate))
							alter += $" ON UPDATE {onUpdate}";
						alter += ";";

						_alterStatements.Add(alter);
						continue;
					}
					else
					{
						newElements.Add(element);
					}
				}
				else
				{
					// PRIMARY KEY AUTOINCREMENT
					if (Regex.IsMatch(element, @"PRIMARY\s+KEY\s+AUTOINCREMENT", RegexOptions.IgnoreCase))
					{
						var columnMatch = Regex.Match(element, @"(?<column>\[?[a-zA-Z0-9_]+\]?)\s+");
						var constraintMatch = Regex.Match(element, @"CONSTRAINT\s+(?<constraint>\[?[a-zA-Z0-9_]+\]?)", RegexOptions.IgnoreCase);

						string columnName = columnMatch.Success ? columnMatch.Groups["column"].Value : "Id";
						string constraintName = constraintMatch.Success
							? constraintMatch.Groups["constraint"].Value
							: $"PK_{tableName}_{columnName}";

						string newElement = Regex.Replace(element, @"CONSTRAINT\s+\[?[a-zA-Z0-9_]+\]?\s+", "", RegexOptions.IgnoreCase);
						newElement = Regex.Replace(newElement, @"PRIMARY\s+KEY\s+AUTOINCREMENT", "", RegexOptions.IgnoreCase);
						newElement = Regex.Replace(newElement, @"\bINTEGER\b", "INT IDENTITY(1,1)", RegexOptions.IgnoreCase);
						newElement = Regex.Replace(newElement, @"\s+", " ").Trim();

						newElements.Add(newElement);

						_alterStatements.Add($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} PRIMARY KEY ({columnName});");
					}
					else
					{
						// Обычный столбец
						string newElement = Regex.Replace(element, @"\bINTEGER\b", "INT", RegexOptions.IgnoreCase);
						newElements.Add(newElement);
					}
				}
			}

			return string.Join(", ", newElements);
		}

		private List<string> SplitDefinition(string definition)
		{
			var result = new List<string>();
			int depth = 0;
			int start = 0;

			for (int i = 0; i < definition.Length; i++)
			{
				char c = definition[i];
				if (c == '(') depth++;
				else if (c == ')') depth--;
				else if (c == ',' && depth == 0)
				{
					var part = definition.Substring(start, i - start).Trim();
					if (!string.IsNullOrEmpty(part))
						result.Add(part);
					start = i + 1;
				}
			}

			var lastPart = definition.Substring(start).Trim();
			if (!string.IsNullOrEmpty(lastPart))
				result.Add(lastPart);

			return result;
		}
	}

	/*
		public class LiteToServer
		{
			private readonly List<string> _alterStatements = new();

			public void Convert(LogData log)
			{
				if (log == null || string.IsNullOrEmpty(log.CommandText))
					return;

				_alterStatements.Clear();
				log.CommandText = ToTSQL(log.CommandText);

				if (string.IsNullOrWhiteSpace(log.CommandText))
					log.CommandText = "-- empty command (PRAGMA removed)";
			}

			private string ToTSQL(string sqlCommand)
			{
				// 1. Удаляем PRAGMA
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"^\s*PRAGMA\s+[^;]*;?\s*$",
					"",
					RegexOptions.IgnoreCase | RegexOptions.Multiline
				);
				sqlCommand = Regex.Replace(sqlCommand, @"^\s*\r?\n", "", RegexOptions.Multiline);

				// 2. Заменяем кавычки
				sqlCommand = Regex.Replace(sqlCommand, @"""(?<id>[^""]+)""", "[${id}]");

				// 3. Преобразуем sqlite_master
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"SELECT\s+COUNT\(\*\)\s+FROM\s+\[sqlite_master\]\s+WHERE\s+\[type\]\s*=\s*'table'\s+AND\s+\[rootpage\]\s+IS\s+NOT\s+NULL\s*;?",
					"SELECT COUNT(*) FROM sys.tables WHERE type = 'U';",
					RegexOptions.IgnoreCase
				);
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"FROM\s+\[sqlite_master\]",
					"FROM sys.tables",
					RegexOptions.IgnoreCase
				);
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"\[\s*type\s*\]\s*=\s*'table'",
					"type = 'U'",
					RegexOptions.IgnoreCase
				);
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"\s+AND\s+\[\s*rootpage\s*\]\s+IS\s+NOT\s+NULL",
					"",
					RegexOptions.IgnoreCase
				);
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"\[\s*rootpage\s*\]\s+IS\s+NOT\s+NULL\s*(AND\s+)?",
					"",
					RegexOptions.IgnoreCase
				);

				// 4. Добавляем схему dbo
				sqlCommand = AddDboSchema(sqlCommand);

				// 5. Обрабатываем CREATE TABLE
				var pattern = @"CREATE\s+TABLE\s+(?<tableName>\[?[^\s\(]+\]?)\s*\((?<definition>.*)\)\s*;?";
				var matches = Regex.Matches(sqlCommand, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
				foreach (Match match in matches)
				{
					var tableName = match.Groups["tableName"].Value;
					if (!tableName.Contains("."))
						tableName = "[dbo]." + tableName;

					var definition = match.Groups["definition"].Value;
					var newDefinition = ParseTableDefinition(definition, tableName);
					var newCreate = $"CREATE TABLE {tableName} ({newDefinition});";
					sqlCommand = sqlCommand.Replace(match.Value, newCreate);
				}

				// 6. Добавляем все ALTER (PRIMARY KEY + FOREIGN KEY)
				if (_alterStatements.Count > 0)
					sqlCommand += Environment.NewLine + string.Join(Environment.NewLine, _alterStatements);

				// 7. Заменяем INTEGER на INT (оставшиеся)
				sqlCommand = Regex.Replace(sqlCommand, @"\bINTEGER\b", "INT", RegexOptions.IgnoreCase);

				// 8. Заменяем TEXT
				sqlCommand = Regex.Replace(sqlCommand, @"\bTEXT\b", "NVARCHAR(MAX)", RegexOptions.IgnoreCase);

				// 9. LIMIT → TOP
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"(?<=SELECT\s)(.*?)(?=\s+LIMIT\s+(\d+)\s*;?)",
					"TOP $2 $1",
					RegexOptions.IgnoreCase | RegexOptions.Singleline
				);
				sqlCommand = Regex.Replace(
					sqlCommand,
					@"\s+LIMIT\s+\d+\s*;?",
					"",
					RegexOptions.IgnoreCase
				);

				return sqlCommand.Trim();
			}

			private string AddDboSchema(string sql)
			{
				string pattern = @"\b(FROM|JOIN|UPDATE|INTO|TABLE|DELETE\s+FROM|INSERT\s+INTO)\s+\[([^\]]+)\]";
				return Regex.Replace(sql, pattern, m =>
				{
					string keyword = m.Groups[1].Value;
					string tableName = m.Groups[2].Value;
					if (tableName.Contains(".") || tableName.StartsWith("sys.", StringComparison.OrdinalIgnoreCase))
						return m.Value;
					return $"{keyword} [dbo].[{tableName}]";
				}, RegexOptions.IgnoreCase);
			}

			private string ParseTableDefinition(string definition, string tableName)
			{
				var elements = SplitDefinition(definition);
				var newElements = new List<string>();

				foreach (var element in elements)
				{
					// FOREIGN KEY
					if (Regex.IsMatch(element, @"FOREIGN\s+KEY", RegexOptions.IgnoreCase))
					{
						var fkPattern = @"CONSTRAINT\s+(?<constraint>\[?[a-zA-Z0-9_]+\]?)\s+FOREIGN\s+KEY\s*\((?<columns>[^\)]+)\)\s+REFERENCES\s+(?<refTable>\[?[a-zA-Z0-9_]+\]?)\s*\((?<refColumns>[^\)]+)\)\s*(?:ON\s+DELETE\s+(?<onDelete>RESTRICT|CASCADE|SET\s+NULL|SET\s+DEFAULT|NO\s+ACTION))?\s*(?:ON\s+UPDATE\s+(?<onUpdate>RESTRICT|CASCADE|SET\s+NULL|SET\s+DEFAULT|NO\s+ACTION))?";
						var m = Regex.Match(element, fkPattern, RegexOptions.IgnoreCase);
						if (m.Success)
						{
							string constraintName = m.Groups["constraint"].Value;
							string columns = m.Groups["columns"].Value;
							string refTable = m.Groups["refTable"].Value;
							string refColumns = m.Groups["refColumns"].Value;
							string onDelete = m.Groups["onDelete"].Value;
							string onUpdate = m.Groups["onUpdate"].Value;

							if (onDelete.Equals("RESTRICT", StringComparison.OrdinalIgnoreCase))
								onDelete = "NO ACTION";
							if (onUpdate.Equals("RESTRICT", StringComparison.OrdinalIgnoreCase))
								onUpdate = "NO ACTION";

							if (!refTable.Contains("."))
								refTable = "[dbo]." + refTable;

							string alter = $"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} FOREIGN KEY ({columns}) REFERENCES {refTable} ({refColumns})";
							if (!string.IsNullOrEmpty(onDelete))
								alter += $" ON DELETE {onDelete}";
							if (!string.IsNullOrEmpty(onUpdate))
								alter += $" ON UPDATE {onUpdate}";
							alter += ";";

							_alterStatements.Add(alter);
							continue;
						}
						else
						{
							newElements.Add(element);
						}
					}
					else
					{
						// PRIMARY KEY AUTOINCREMENT
						if (Regex.IsMatch(element, @"PRIMARY\s+KEY\s+AUTOINCREMENT", RegexOptions.IgnoreCase))
						{
							var columnMatch = Regex.Match(element, @"(?<column>\[?[a-zA-Z0-9_]+\]?)\s+");
							var constraintMatch = Regex.Match(element, @"CONSTRAINT\s+(?<constraint>\[?[a-zA-Z0-9_]+\]?)", RegexOptions.IgnoreCase);

							string columnName = columnMatch.Success ? columnMatch.Groups["column"].Value : "Id";
							string constraintName = constraintMatch.Success
								? constraintMatch.Groups["constraint"].Value
								: $"PK_{tableName}_{columnName}";

							string newElement = Regex.Replace(element, @"CONSTRAINT\s+\[?[a-zA-Z0-9_]+\]?\s+", "", RegexOptions.IgnoreCase);
							newElement = Regex.Replace(newElement, @"PRIMARY\s+KEY\s+AUTOINCREMENT", "", RegexOptions.IgnoreCase);
							newElement = Regex.Replace(newElement, @"\bINTEGER\b", "INT IDENTITY(1,1)", RegexOptions.IgnoreCase);
							newElement = Regex.Replace(newElement, @"\s+", " ").Trim();

							newElements.Add(newElement);

							// Добавляем ALTER PRIMARY KEY
							_alterStatements.Add($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} PRIMARY KEY ({columnName});");
						}
						else
						{
							// Обычный столбец
							string newElement = Regex.Replace(element, @"\bINTEGER\b", "INT", RegexOptions.IgnoreCase);
							newElements.Add(newElement);
						}
					}
				}

				return string.Join(", ", newElements);
			}

			private List<string> SplitDefinition(string definition)
			{
				var result = new List<string>();
				int depth = 0;
				int start = 0;

				for (int i = 0; i < definition.Length; i++)
				{
					char c = definition[i];
					if (c == '(') depth++;
					else if (c == ')') depth--;
					else if (c == ',' && depth == 0)
					{
						var part = definition.Substring(start, i - start).Trim();
						if (!string.IsNullOrEmpty(part))
							result.Add(part);
						start = i + 1;
					}
				}

				var lastPart = definition.Substring(start).Trim();
				if (!string.IsNullOrEmpty(lastPart))
					result.Add(lastPart);

				return result;
			}
		}
	*/

}