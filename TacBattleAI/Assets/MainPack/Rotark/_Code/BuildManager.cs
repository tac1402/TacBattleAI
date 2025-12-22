using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
	public List<BuildModel> AllModel = new List<BuildModel>();


	public void Create()
	{
		System.Random rnd = new System.Random();
		BuildingChange[] allBuilding = GetComponentsInChildren<BuildingChange>();

		for (int i = 0; i < allBuilding.Length; i++)
		{
			//allBuilding[i].ModelIndex = rnd.Next(0, AllModel.Count);
			allBuilding[i].ChangeModel(AllModel[allBuilding[i].ModelIndex].model);
		}

	}

}

[Serializable]
public class BuildModel
{
	public GameObject model;
}