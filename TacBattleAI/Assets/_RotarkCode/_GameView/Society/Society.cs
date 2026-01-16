using System.Collections.Generic;
using UnityEngine;

using Tac.Person;
using System.Linq;
using Tac.Agent;

namespace Tac.Society
{
	public partial class Society : MonoBehaviour
	{
		public List<GameObject> MenModel;
		public List<GameObject> WomenModel;
		public ItemCreate.ItemCreate ItemCreate;

		public List<AgentPoint> AllAgentPoint;

		public void Init()
		{
			RobotJob = GetComponent<RobotJob>();
			PlayerJob = GetComponent<PlayerJob>();

			PersonName.LoadName(rnd);
			AllAgentPoint = GetComponentsInChildren<AgentPoint>().ToList();

			for (int i = 0; i < AllAgentPoint.Count; i++)
			{
				AllAgentPoint[i].ObjectId = ItemCreate.GetNewId();
			}
		}

		public void InitWorkPlace()
		{
			foreach (Person.Person p in People.Values)
			{
				int pointIndex = rnd.Next(0, AllAgentPoint.Count);
				p.WorkPlace = AllAgentPoint[pointIndex];
			}
		}

		public void NextHour(GameTime argGameTime)
		{
			PlayerJob.NextHour(argGameTime);
			RobotJob.NextHour(argGameTime);

			/*
			List<Person.Person> tmp = People.Values.ToList();
			if (tmp[2].WorkPlace != null)
			{
				tmp[2].WorkPlace.Add(tmp[1]);
			}*/
		}


		public void AddModel()
		{
			for (int i = 0; i < MenModel.Count; i++)
			{
				ItemCreate.AddModel(MenModel[i]);
			}
			for (int i = 0; i < WomenModel.Count; i++)
			{
				ItemCreate.AddModel(WomenModel[i]);
			}
		}

		public Person.Person CreatePerson(GenderType argGenderType, Vector2_ argPosition)
		{
			Person.Person person = null;
			if (argGenderType == GenderType.Men)
			{
				int menIndex = rnd.Next(0, MenModel.Count);

				Item menId = MenModel[menIndex].GetComponent<Item>();
				GameObject menObj = ItemCreate.CreateObject(menId.ModelName, argPosition.x, argPosition.y);
				person = menObj.GetComponent<Person.Person>();

				person.Gender = GenderType.Men;
			}
			else if (argGenderType == GenderType.Women)
			{
				int womenIndex = rnd.Next(0, WomenModel.Count);

				Item womenId = WomenModel[womenIndex].GetComponent<Item>();
				GameObject womenObj = ItemCreate.CreateObject(womenId.ModelName, argPosition.x, argPosition.y);
				person = womenObj.GetComponent<Person.Person>();

				person.Gender = GenderType.Women;
			}
			return person;
		}

	}
}