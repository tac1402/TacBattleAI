
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Tac.Agent;
using Tac.Person;
using Tac.UI;

using UnityEF;

namespace Tac.Society
{
	public partial class Society
	{
		/// <summary>
		/// Все персонажи в игре
		/// </summary>
		public GDictionary<int, Person.Person> People;

		public RobotJob RobotJob;
		public PlayerJob PlayerJob;

		/// <summary>
		/// Идентификатор персонажа игрока
		/// </summary>
		public int PlayerPersonId = 0;


		public PeopleTable peopleTable = new PeopleTable();

		private PersonName PersonName = new PersonName();
		private System.Random rnd = new System.Random();
		private GameTime oldGameTime;


		public override void InitData()
		{
			People = new GDictionary<int, Person.Person>();
		}

		public void InitLogic()
		{
			if (PeoplePanel != null)
			{
				TableUI tableUI = PeoplePanel.GetComponentInChildren<TableUI>();
				peopleTable.Assign(tableUI, FindAgent);
			}
		}


		public void AddAgentPlan(Person.Person argAgent, bool IsPlayer = false)
		{
			if (IsPlayer)
			{
				PlayerJob.AddPersonPlan(argAgent);
				PlayerPersonId = argAgent.Id;
			}
			else
			{
				RobotJob.AddPersonPlan(argAgent);
			}
		}


		public List<Person.Person> AddPerson(int argCount, Rect_ argLocation, bool IsFamily = true)
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

				person.OnWalkEnd += Person_OnWalkEnd;

				People.Add(person.Id, person);
				peopleTable.Add(person);

				ret.Add(person);
			}

			return ret;
		}

		protected virtual void Person_OnWalkEnd(params object[] argInfo)
		{ 
			Agent.Agent agent = argInfo[0] as Agent.Agent;

			AllAgentPoint[agent.TargetId].WalkToEnter(oldGameTime, agent);
		}
	}
}