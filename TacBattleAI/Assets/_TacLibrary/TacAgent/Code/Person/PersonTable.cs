using System.Collections;
using System.Collections.Generic;
using Tac.UI;

namespace Tac.Person_
{

	public class PeopleTable
	{
		List<Person> allPeople = new List<Person>();
		public TableUI tableUI;

		public void Assign(TableUI argTableUI, TableUI.ButtonClickDelegate argButtonClick)
		{
			tableUI = argTableUI;
			tableUI.ButtonClickHandler = argButtonClick;
		}

		public void Add(Person argPerson)
		{
			allPeople.Add(argPerson);
		}

		public void Show()
		{
			Table myTable = new Table();

			tableUI.AddColumn("Id", 50);
			tableUI.AddColumn("Name", 200);
			tableUI.AddColumn("Find", 200, ColumnType.Button);

			for (int i = 0; i < allPeople.Count; i++)
			{
				Row tableRow = new Row();

				tableRow.Column.Add(allPeople[i].Id.ToString());
				tableRow.Column.Add(allPeople[i].Name);

				myTable.Rows.Add(tableRow);
			}
			tableUI.AddTable(myTable);
			tableUI.ShowPage(1);

		}

		public void Hide()
		{
			tableUI.Clear();
		}

	}
}
