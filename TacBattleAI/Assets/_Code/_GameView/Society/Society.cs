using System.Collections.Generic;
using UnityEngine;

using Tac.ItemCreate;
using Tac.DConvert;

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

	public Person CreatePerson(GenderType argGenderType, Vector2_ argPosition)
	{
		Person person = null;
		if (argGenderType == GenderType.Men)
		{
			int menIndex = rnd.Next(0, MenModel.Count);

			PrefabId menId = MenModel[menIndex].GetComponent<PrefabId>();
			GameObject menObj = ItemCreate.CreateObject(menId.PrefabName, argPosition.x, argPosition.y, 0);
			person = menObj.GetComponent<Person>();

			person.Gender = GenderType.Men;
		}
		else if (argGenderType == GenderType.Women)
		{
			int womenIndex = rnd.Next(0, WomenModel.Count);

			PrefabId womenId = WomenModel[womenIndex].GetComponent<PrefabId>();
			GameObject womenObj = ItemCreate.CreateObject(womenId.PrefabName, argPosition.x, argPosition.y, 0);
			person = womenObj.GetComponent<Person>();

			person.Gender = GenderType.Women;
		}
		return person;
	}
}
