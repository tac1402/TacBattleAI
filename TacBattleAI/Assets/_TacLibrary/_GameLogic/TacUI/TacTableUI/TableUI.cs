using System;
using System.Collections.Generic;

namespace Tac.UI
{
	public partial class TableUI
	{
		public int RowCount;
		public List<int> columnWidth = new List<int>();
		public List<int> fontSize = new List<int>();
		public List<string> header = new List<string>();

		public int RowInPage = 30;
		public int PageNumber = 1;
		public int PageCount = 1;

		public Table Table;

		public void AddTable(Table argTable)
		{
			Table = argTable;
		}

		public void AddColumn(string argColumnName, int argColumnWidth, int argFontSize = 16)
		{
			header.Add(argColumnName);
			columnWidth.Add(argColumnWidth);
			fontSize.Add(argFontSize);
		}

		public void ClearColumn()
		{
			header.Clear();
			columnWidth.Clear();
		}

		public void NextPage()
		{
			if (PageNumber < PageCount)
			{
				ShowPage(PageNumber + 1);
			}
		}
		public void PrevPage()
		{
			if (PageNumber > 1)
			{
				ShowPage(PageNumber - 1);
			}
		}


#if OnlyLogic
		public void Clear()
		{
			ClearColumn();
		}

		public void ShowPage(int argPageNumber)
		{
			int RowFrom = RowInPage * (argPageNumber - 1);
			int RowTill = Math.Min(Table.Rows.Count, RowInPage * argPageNumber);

			PageNumber = argPageNumber;
			PageCount = Table.Rows.Count / RowInPage + 1;
		}
#endif

	}

	public class Table
	{
		public List<Row> Rows = new List<Row>();
	}

	public class Row
	{
		public List<string> Column = new List<string>();
	}

}