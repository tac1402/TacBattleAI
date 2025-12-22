using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tac.Society;
using Tac;

public class BuildingChange : MonoBehaviour
{

	public int ModelIndex = 0;

	GameObject main;


	void Start ()
	{
	}

	public void ChangeModel(GameObject argObject)
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

		//gameObject.AddComponent<Business>();
		
		MeshFilter locMeshFilter = gameObject.GetComponent<MeshFilter>();
		MeshRenderer locMeshRenderer = gameObject.GetComponent<MeshRenderer>();
		MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

		GameObject newObject = Instantiate(argObject);
		newObject.transform.parent = main.transform;

		/*
		GameObject pivot = new GameObject("Pivot");
		pivot.transform.parent = transform;

		GameObject view = new GameObject("View");
		view.transform.parent = pivot.transform;

		GameObject view2 = new GameObject(gameObject.name);
		view2.transform.parent = view.transform;

		MeshFilter mesh = view2.AddComponent<MeshFilter>();
		MeshRenderer renderer = view2.AddComponent<MeshRenderer>();

		mesh.mesh = locMeshFilter.sharedMesh;
		renderer.materials = locMeshRenderer.sharedMaterials;

		BoxCollider box = view2.AddComponent<BoxCollider>();
		box.size = mesh.mesh.bounds.size;

		Business item2 = gameObject.AddComponent<Business>();
		item2.ModelName = gameObject.name;
		item2.GroupId = 2;
		*/

		DestroyImmediate(locMeshFilter);
		DestroyImmediate(locMeshRenderer);
		DestroyImmediate(meshCollider);

	}


}


