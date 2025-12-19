// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Tac.UI
{
    public partial class TableUI : MonoBehaviour
    {
		public GameObject TablePanel;
		public GameObject RowPrefab;
		public GameObject CellPrefab;

		public TMP_Text PageInfo;

		private List<GameObject> Rows = new List<GameObject>();

		public void Clear()
		{
			ClearColumn();
			foreach (GameObject row in Rows)
			{
				Destroy(row);
			}
			Rows.Clear();
		}


		public void ShowPage(int argPageNumber)
		{
			int RowFrom = RowInPage * (argPageNumber - 1);
			int RowTill = Math.Min(Table.Rows.Count, RowInPage * argPageNumber);

			PageNumber = argPageNumber;
			PageCount = Table.Rows.Count / RowInPage + 1;

			PageInfo.text = PageNumber.ToString() + "/" + PageCount.ToString();


			for (int i = 0; i < Rows.Count; i++)
			{
				Destroy(Rows[i]);
			}

			AddRow(header, true);
			for (int i = RowFrom ; i < RowTill; i++)
			{
				AddRow(Table.Rows[i].Column);
			}
		}

		private void AddRow(List<string> columnText, bool argIsBold = false)
		{
			GameObject row = Instantiate(RowPrefab, TablePanel.transform);
			for (int j = 0; j < columnWidth.Count; j++)
			{
				GameObject cell = Instantiate(CellPrefab, row.transform);
				LayoutElement layout = cell.GetComponent<LayoutElement>();
				layout.minWidth = columnWidth[j];
				TMP_Text text = cell.GetComponent<TMP_Text>();
				text.fontSize = fontSize[j];
				text.text = columnText[j];
				if (argIsBold)
				{
					text.fontStyle = FontStyles.Bold;
				}
			}
			Rows.Add(row);
		}

	}
}
