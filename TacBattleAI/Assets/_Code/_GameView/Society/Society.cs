using System.Collections.Generic;
using UnityEngine;

using Tac;
using Tac.ItemCreate;
using Tac.Person;

public partial class Society : MonoBehaviour
{
	public List<GameObject> MenModel;
	public List<GameObject> WomenModel;
	public ItemCreate ItemCreate;

	private System.Random rnd = new System.Random();

	public void Init()
	{
		PersonName.LoadName(rnd);
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

	public Person CreatePerson(GenderType argGenderType, Vector2_ argPosition)
	{
		Person person = null;
		if (argGenderType == GenderType.Men)
		{
			int menIndex = rnd.Next(0, MenModel.Count);

			Item menId = MenModel[menIndex].GetComponent<Item>();
			GameObject menObj = ItemCreate.CreateObject(menId.ModelName, argPosition.x, argPosition.y);
			person = menObj.GetComponent<Person>();

			person.Gender = GenderType.Men;
		}
		else if (argGenderType == GenderType.Women)
		{
			int womenIndex = rnd.Next(0, WomenModel.Count);

			Item womenId = WomenModel[womenIndex].GetComponent<Item>();
			GameObject womenObj = ItemCreate.CreateObject(womenId.ModelName, argPosition.x, argPosition.y);
			person = womenObj.GetComponent<Person>();

			person.Gender = GenderType.Women;
		}
		return person;
	}
}
