using System.Collections.Generic;
using UnityEngine;

using Tac.Organization;
using Tac.Person;

namespace Tac.Society
{
	public partial class Society : MonoBehaviour
	{
		public List<GameObject> MenModel;
		public List<GameObject> WomenModel;
		public ItemCreate.ItemCreate ItemCreate;

		public Business[] AllBusiness;

		public void Init()
		{
			RobotJob = GetComponent<RobotJob>();
			PlayerJob = GetComponent<PlayerJob>();

			PersonName.LoadName(rnd);
			AllBusiness = GetComponentsInChildren<Business>();

			for (int i = 0; i < AllBusiness.Length; i++)
			{
				AllBusiness[i].ObjectId = ItemCreate.GetNewId();
			}
		}

		public void InitWorkPlace()
		{
			foreach (Person.Person p in People.Values)
			{
				int pointIndex = rnd.Next(0, AllBusiness.Length);
				p.WorkPlace = AllBusiness[pointIndex];
			}
		}

		public void NextHour(GameTime argGameTime)
		{
			PlayerJob.NextHour(argGameTime);
			RobotJob.NextHour(argGameTime);

			/*ChangeSalaryDeficit();
			CheckJob();
			CalcScore();

			PurchaseProduct();

			if (DayNight.Time.Hours.In(6, 12, 18))
			{
				Eat();
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