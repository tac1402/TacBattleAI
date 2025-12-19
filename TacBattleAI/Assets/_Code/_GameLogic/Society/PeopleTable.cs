using System.Collections;
using System.Collections.Generic;
using Tac.UI;
using Tac.Person;

namespace Tac.Society
{

	public class PeopleTable
	{
		List<Person.Person> allPeople = new List<Person.Person>();
		public TableUI tableUI;

		public void Assign(TableUI argTableUI)
		{
			tableUI = argTableUI;
		}

		public void Add(Person.Person argPerson)
		{
			allPeople.Add(argPerson);
		}

		public void Show()
		{
			Table myTable = new Table();

			tableUI.AddColumn("Id", 50);
			tableUI.AddColumn("Name", 200);
			tableUI.AddColumn("Age", 50);
			tableUI.AddColumn("Edu", 50);

			tableUI.AddColumn("WorkPlace", 200);
			tableUI.AddColumn("Money", 100);
			tableUI.AddColumn("Salary", 100);
			tableUI.AddColumn("Work", 70);

			tableUI.AddColumn("Fatigue", 70);
			tableUI.AddColumn("Hunger", 70);

			tableUI.AddColumn("IsBusy", 100);
			tableUI.AddColumn("Located", 200);
			tableUI.AddColumn("Target", 200);

			for (int i = 0; i < allPeople.Count; i++)
			{
				Row tableRow = new Row();

				tableRow.Column.Add(allPeople[i].ObjectId.ToString());
				tableRow.Column.Add(allPeople[i].Name);
				/*tableRow.Column.Add(allPeople[i].Age.ToString());
				tableRow.Column.Add(allPeople[i].Education.ToString("F2"));

				string workPlace = "";
				if (allPeople[i].WorkPlace != null)
				{
					workPlace = allPeople[i].WorkPlace.PropertyType.ToString() + "-" + allPeople[i].WorkPlace.Id.ToString();
				}
				tableRow.Column.Add(workPlace);

				tableRow.Column.Add(allPeople[i].Money.ToString("F2"));
				tableRow.Column.Add(allPeople[i].Salary.ToString("F2"));
				tableRow.Column.Add(allPeople[i].WorkExperience.ToString());
				tableRow.Column.Add(allPeople[i].Fatigue.ToString("F0"));
				tableRow.Column.Add(allPeople[i].Hunger.ToString("F0"));
				

				tableRow.Column.Add(allPeople[i].IsBusy.ToString());
				string located = Main.World.Society.GetLocated(allPeople[i]);
				tableRow.Column.Add(located);
				string target = Main.World.Society.GetTarget(allPeople[i]);
				tableRow.Column.Add(target);
				*/

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
