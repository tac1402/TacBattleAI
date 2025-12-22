using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tac.Society;

public class BuildingChange : MonoBehaviour
{

	public int ModelIndex = 0;

	GameObject main;


	void Start ()
	{
	}

	public void ChangeModel()
	{
		/*
		main = GameObject.Find("Rotark");

		BuildManager manager = main.GetComponent<BuildManager>();

		MeshFilter locMeshFilter = gameObject.GetComponent<MeshFilter>();
		MeshRenderer locMeshRenderer = gameObject.GetComponent<MeshRenderer>();

		locMeshFilter.mesh = manager.AllModel[ModelIndex].mesh;
		locMeshRenderer.material = manager.AllModel[ModelIndex].material;

		transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
		transform.localScale = new Vector3(1, 1, 1);
		*/

		//BoxCollider locBoxCollider = gameObject.GetComponent<BoxCollider>();
		//DestroyImmediate(locBoxCollider);

		gameObject.AddComponent<Business>();

	}
	

}


