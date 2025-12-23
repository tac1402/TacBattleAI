using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
	public List<BuildModel> AllModel = new List<BuildModel>();


	public void Create()
	{
		System.Random rnd = new System.Random();
		//BuildingChange[] allBuilding = GetComponentsInChildren<BuildingChange>();

		/*
		Street[] streets = GetComponentsInChildren<Street>();
		for (int i = 0; i < streets.Length; i++)
		{
			NavMeshModifier navMesh = streets[i].gameObject.AddComponent<NavMeshModifier>();
			navMesh.overrideArea = true;
			navMesh.area = 4;
		}*/

		//int i = 0;
		
		/*for (int i = 0; i < allBuilding.Length; i++)
		{
			//allBuilding[i].ModelIndex = rnd.Next(0, AllModel.Count);
			allBuilding[i].ChangeModel(AllModel[allBuilding[i].ModelIndex].model);
		}*/

		/*for (int i = 0; i < allBuilding.Length; i++)
		{
			if (allBuilding[i].ModelIndex == 100)
			{
				DestroyImmediate(allBuilding[i].gameObject);
			}
		}*/


	}

}

[Serializable]
public class BuildModel
{
	public GameObject model;
}