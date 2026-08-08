// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Data;

namespace Tac.Sql
{

	public class DataTableConverter
	{
		// Преобразование DataTable → LogDataTable
		public LogDataTable Convert(DataTable dt)
		{
			var logTable = new LogDataTable
			{
				Columns = new List<ColumnInfo>(),
				Rows = new List<List<string>>()
			};

			foreach (DataColumn col in dt.Columns)
			{
				logTable.Columns.Add(new ColumnInfo
				{
					Name = col.ColumnName,
					TypeName = col.DataType.Name
				});
			}

			foreach (DataRow row in dt.Rows)
			{
				var rowValues = new List<string>();
				foreach (var item in row.ItemArray)
				{
					rowValues.Add(item == DBNull.Value ? null : item.ToString());
				}
				logTable.Rows.Add(rowValues);
			}

			return logTable;
		}

		// Восстановление DataTable из LogDataTable
		public DataTable Restore(LogDataTable logTable)
		{
			var dt = new DataTable();

			// Создаём колонки
			foreach (var colInfo in logTable.Columns)
			{
				// Получаем тип по имени – для стандартных типов System.*
				Type colType = Type.GetType($"System.{colInfo.TypeName}") ?? typeof(string);
				dt.Columns.Add(colInfo.Name, colType);
			}

			// Добавляем строки
			foreach (var rowValues in logTable.Rows)
			{
				var row = dt.NewRow();
				for (int i = 0; i < rowValues.Count; i++)
				{
					var val = rowValues[i];
					if (val == null)
					{
						row[i] = DBNull.Value;
					}
					else
					{
						Type targetType = dt.Columns[i].DataType;
						row[i] = System.Convert.ChangeType(val, targetType);
					}
				}
				dt.Rows.Add(row);
			}

			return dt;
		}
	}
}