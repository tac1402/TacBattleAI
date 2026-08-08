
using System.Collections.Generic;
using Tac.Agent_;
using Tac.Person_;
using Tac.ItemCreate_;
using Tac.UI;

using UnityEF;
using UnityEngine;

namespace Tac.Society
{
	public class SocietyLogic : Logic
	{
		/// <summary>
		/// Все персонажи в игре
		/// </summary>
		public GDictionary<int, Person_.Person> People;

		public RobotJob RobotJob;
		public PlayerJob PlayerJob;
		public ItemCreate ItemCreate;
		internal PeopleTable PeopleTable = new PeopleTable();
		public List<AgentPoint> AllAgentPoint;

		internal PersonName PersonName = new PersonName();
		internal System.Random rnd = new System.Random();
		internal GameTime oldGameTime;


		public List<Person_.Person> AddPerson(int argCount, Rect_ argLocation, List<Person_.Person> argFakePerson, bool IsFamily = true)
		{
			List<Person_.Person> ret = new List<Person_.Person>();
			RandomWorld randomWorld = new RandomWorld();

			string surname = "";
			if (IsFamily)
			{
				surname = PersonName.GetSurname();
			}

			for (int i = 0; i < argCount; i++)
			{
				Person_.Person person = argFakePerson[i];
				person.Position = randomWorld.GetRandomPosition(argLocation).To3();

				person = AddPerson(person);

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

				ret.Add(person);
			}
			return ret;
		}

		public Person_.Person AddPerson(Person_.Person argPerson)
		{
			if (argPerson.RecoverMode == true)
			{
				ItemCreate.PredeffinedObjectId = argPerson.Id;
			}
			GameObject prefab = ItemCreate.CreateObject(argPerson.ModelName, argPerson.Position.x, argPerson.Position.z);
			Person_.Person person = prefab.GetComponent<Person_.Person>();

			person.Init(argPerson.RecoverMode);
			person.CheckPosition();
			person.OnWalkEnd += Person_OnWalkEnd;

			if (argPerson.RecoverMode == false)
			{
				People.Add(person.Id, person);
			} // else во время загрузки person будет автоматически присоединен к People, поэтому прямое присваивание не нужно

			PeopleTable.Add(person);
			return person;
		}


		protected virtual void Person_OnWalkEnd(params object[] argInfo)
		{
			Agent_.Agent agent = argInfo[0] as Agent_.Agent;

			AllAgentPoint[agent.TargetId].WalkToEnter(oldGameTime, agent);
		}


		/*public List<Person.Person> AddPerson(int argCount, Rect_ argLocation, bool IsFamily = true)
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

		public Person.Person CreatePerson(GenderType argGenderType, Vector2_ argPosition)
		{
			Person.Person person = null;
			if (argGenderType == GenderType.Men)
			{
				int menIndex = logic.rnd.Next(0, MenModel.Count);

				Flow menId = MenModel[menIndex].GetComponent<Flow>();
				GameObject menObj = ItemCreate.CreateObject(menId.ModelName, argPosition.x, argPosition.y);
				person = menObj.GetComponent<Person.Person>();

				person.Gender = GenderType.Men;
			}
			else if (argGenderType == GenderType.Women)
			{
				int womenIndex = logic.rnd.Next(0, WomenModel.Count);

				Flow womenId = WomenModel[womenIndex].GetComponent<Flow>();
				GameObject womenObj = ItemCreate.CreateObject(womenId.ModelName, argPosition.x, argPosition.y);
				person = womenObj.GetComponent<Person.Person>();

				person.Gender = GenderType.Women;
			}
			return person;
		}*/


	}
}