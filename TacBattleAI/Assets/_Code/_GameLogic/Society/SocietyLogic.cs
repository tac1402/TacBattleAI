
using System.Collections.Generic;

using Tac.Person;

namespace Tac.Society
{
	public partial class Society
	{
		/// <summary>
		/// Все персонажи в игре
		/// </summary>
		public Dictionary<int, Person.Person> People = new Dictionary<int, Person.Person>();


		public RobotJob RobotJob;
		public PlayerJob PlayerJob;
		public DayNight DayNight;

		/// <summary>
		/// Идентификатор персонажа игрока
		/// </summary>
		public int PlayerPersonId = 0;


		public PeopleTable peopleTable = new PeopleTable();

		private PersonName PersonName = new PersonName();
		private System.Random rnd = new System.Random();



		public void AddAgentPlan(Person.Person argAgent, bool IsPlayer = false)
		{
			if (IsPlayer)
			{
				PlayerJob.AddPersonPlan(argAgent);
				PlayerPersonId = argAgent.ObjectId;
			}
			else
			{
				RobotJob.AddPersonPlan(argAgent);
			}
		}


		public List<Person.Person> AddPeople(int argCount, Rect_ argLocation, bool IsFamily = true)
		{
			List<Person.Person> ret = new List<Person.Person>();
			RandomWorld randomWorld = new RandomWorld();

			string surname = "";
			if (IsFamily)
			{
				surname = PersonName.GetSurname();
			}

			for (int i = 0; i < argCount; i++)
			{
				int isMen = rnd.Next(100);
				Person.Person person;

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
}