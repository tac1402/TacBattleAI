using System.Collections;
using System.Collections.Generic;
using Tac.ItemCreate;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{

	private ItemCreate ItemCreate;

	private void Start()
	{
		ItemCreate = GetComponent<ItemCreate>();
		ItemCreate.Init();

		GameObject rock = ItemCreate.CreateObject("Rock_A", 490, 485, 12);

	}

}