
using System.Collections.Generic;

using Tac;
using Tac.Person;
using Tac.Society;

public partial class Society
{
	/// <summary>
	/// Все персонажи в игре
	/// </summary>
	public Dictionary<int, Person> People = new Dictionary<int, Person>();

	public PeopleTable peopleTable = new PeopleTable();

	private PersonName PersonName = new PersonName();


	public List<Person> AddPeople(int argCount, Rect_ argLocation, bool IsFamily = true)
	{
		List<Person> ret = new List<Person>();
		RandomWorld randomWorld = new RandomWorld();

		string surname = "";
		if (IsFamily)
		{
			surname = PersonName.GetSurname();
		}

		for (int i = 0; i < argCount; i++)
		{
			int isMen = rnd.Next(100);
			Person person;

			Vector2_ position = randomWorld.GetRandomPosition(argLocation);

			if (isMen > 50)
			{
				person = CreatePerson(GenderType.Men, position);
			}
			else
			{
				person = CreatePerson(GenderType.Women, position);
			}

			if (IsFamily == true)
			{
				person.Name = PersonName.GetFamilyName(person.Gender, surname);
				if (person.Name == "")
				{
					person.Name = PersonName.GetUniqueName(person.Gender);
				}
			}
			else
			{
				person.Name = PersonName.GetUniqueName(person.Gender);
			}

			person.Init();
			person.CheckPosition();

			People.Add(person.ObjectId, person);
			peopleTable.Add(person);

			ret.Add(person);
		}

		return ret;
	}

}
