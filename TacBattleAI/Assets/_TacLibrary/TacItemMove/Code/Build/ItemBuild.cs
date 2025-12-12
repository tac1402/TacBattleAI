using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Camera;
using Tac.ItemCreate;
using Tac.ItemMove;
using Tac.Wireframe;
using UnityEngine;
using UnityEngine.InputSystem;


[Component(typeof(TopCamera), typeof(GhostCache), typeof(ItemCollision))]
public partial class ItemBuild : MonoBehaviour
{
	public TopCamera TopCamera;
	public GhostCache GhostCache;
	public ItemCollision ItemCollision;

	public GameObject Grid;
	public XYZ DiscreteType = XYZ.XYZ;

	private Item2 ModelObjectToPlace;
	private Item2 objectToPlace;

	private bool isBuildMode;
	public bool IsBuildMode
	{
		get { return isBuildMode; }

		set
		{
			isBuildMode = value;

			if (Grid != null)
			{
				if (isBuildMode)
				{
					Grid.SetActive(true);
				}
				else
				{
					Grid.SetActive(false);
				}
			}
		}
	}

	private void Awake()
	{
		StartCoroutine(TaskMoveGhost());
	}

	void Update()
	{
		if (IsBuildMode)
		{
			if (Keyboard.current[Key.Escape].wasPressedThisFrame)
			{
				ResetObjectToPlace();
			}
		}
		if (TopCamera.IsUsingUI() == false && ModelObjectToPlace != null)
		{
			if (Mouse.current.rightButton.wasPressedThisFrame)
			{
				if (objectToPlace != null)
				{
					objectToPlace.TurnNext();
				}
			}

			if (Mouse.current.leftButton.wasPressedThisFrame)
			{
				PlaceObject();
			}
		}
	}

	protected IEnumerator TaskMoveGhost()
	{
		while (true)
		{
			if (IsBuildMode == true)
			{
				MoveGhost();
			}
			yield return new WaitForSeconds(0.3f);
		}
	}

	/// <summary>
	/// Перемещение прототипа над поверхностью игроком
	/// </summary>
	public void MoveGhost()
	{
		if (ModelObjectToPlace != null)
		{
			//Это простой случай установки на террайне
			//Vector3 terrainPoint = TopCamera.GetTerrain(Input.mousePosition).Item1;

			//Это сложный случай с проверкой, где можно строить
			List<Bounds> boundList = new List<Bounds>();
			for (int i = 0; i < objectToPlace.Colliders.Length; i++)
			{
				if (objectToPlace.Colliders[i] != null)
				{
					boundList.Add(objectToPlace.Colliders[i].bounds);
				}
			}
			(Vector3 terrainPoint, GameObject buildObj) = TopCamera.GetAllowBuildPoint(Mouse.current.position.ReadValue(), boundList);

			if (buildObj != null)
			{
				allowHeight = terrainPoint.y;
			}

			Vector3 newPosition = new Vector3(terrainPoint.x, allowHeight, terrainPoint.z);
			Vector3 newDiscretePosition = ModelObjectToPlace.GetDiscrete(newPosition, DiscreteType);

			MoveGhostObject(newDiscretePosition);
		}
	}
	float allowHeight = 0;


	/// <summary>
	/// Передвижение по поверхности прототипа строимого объекта
	/// </summary>
	void MoveGhostObject(Vector3 argNewDiscretePosition)
	{
		if (objectToPlace != null)
		{
			objectToPlace.transform.position = argNewDiscretePosition;

			ItemCollision.OnItemMove(objectToPlace);
		}
	}

	public void SelectEntity(string argModelName)
	{
		GameObject locModelObj = GhostCache.ItemCreate.GetModel(argModelName);
		Item2 locItem = locModelObj.GetComponent<Item2>();

		IsBuildMode = true;

		ModelObjectToPlace = locItem;
		CreateGhostObject();
	}

	/// <summary>
	/// Начало строительства одиночного объекта (в то числе начало линии)
	/// </summary>
	void PlaceObject()
	{
		Item2 locItem = CreateObjectFromGhost(objectToPlace);

		ResetObjectToPlace();
	}

	/// <summary>
	/// Отмена режима строительства
	/// </summary>
	public void ResetObjectToPlace()
	{
		IsBuildMode = false;
		if (objectToPlace != null)
		{
			GhostCache.ItemCreate.DestroyObject(objectToPlace.gameObject);
		}
		objectToPlace = null;
		ModelObjectToPlace = null;
	}

	private void CreateGhostObject()
	{
		if (objectToPlace != null)
		{
			GhostCache.ItemCreate.DestroyObject(objectToPlace.gameObject);
		}

		GhostCache.ItemCreate.PredeffinedObjectId = -1;
		GameObject ghostObject = GhostCache.ItemCreate.CreateObject(ModelObjectToPlace.ModelName, transform.position.x, transform.position.z, null);
		GhostCache.ItemCreate.PredeffinedObjectId = 0;
		ghostObject.transform.SetParent(null);

		objectToPlace = ghostObject.GetComponent<Item2>();
		objectToPlace.Init();
		//objectToPlace.Collider = ghostObject.GetComponentsInChildren<Collider>();

		objectToPlace.SetTurn();
		objectToPlace.WireframeShow(WireframeMode.Green);
	}

	private Item2 CreateObjectFromGhost(Item2 argItem)
	{
		GameObject gameObject = GhostCache.ItemCreate.CreateObject(argItem.ModelName,
			argItem.gameObject.transform.position.x, argItem.gameObject.transform.position.z, argItem.gameObject.transform.position.y, argItem.ModelType);

		Item2 retItem = gameObject.GetComponent<Item2>();

		retItem.SetTurn(argItem.Turn.CurrentTurn);

		return retItem;
	}

}

namespace Tac
{
	public partial class Item2 : Item
	{
		public Vector3 DiscreteStep = Vector3.one;

		public Vector3 GetDiscrete(Vector3 argValue, XYZ argXYZ)
		{
			Vector3 ret = argValue;

			if (XYZ_.IsX(argXYZ))
			{
				ret.x = ((int)(argValue.x / DiscreteStep.x)) * DiscreteStep.x;
			}
			if (XYZ_.IsY(argXYZ))
			{
				ret.y = ((int)(argValue.y / DiscreteStep.y)) * DiscreteStep.y;
			}
			if (XYZ_.IsZ(argXYZ))
			{
				ret.z = ((int)(argValue.z / DiscreteStep.z)) * DiscreteStep.z;
			}
			return ret;
		}

	}
}