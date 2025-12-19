using System.Collections;
using System.Collections.Generic;
using Tac.ItemCreate;
using UnityEngine;
using UnityEngine.UIElements;

public partial class World : MonoBehaviour
{

	private ItemCreate ItemCreate;

	private void Start()
	{
		ItemCreate = GetComponent<ItemCreate>();

		Society.AddModel();
		ItemCreate.Init();
		Society.Init();

		GameObject rock = ItemCreate.CreateObject("Rock_A", 490, 485, 12);

		CreateWorld();
	}

	public void CreateWorld()
	{
		CreateWorld_Logic();
	}

}