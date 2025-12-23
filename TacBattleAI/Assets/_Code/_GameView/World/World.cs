using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Agent;
using Tac.ItemCreate;
using Tac.Society;

using UnityEngine;
using UnityEngine.UIElements;

public partial class World : MonoBehaviour
{
	public List<NavMeshBasic> NavMeshBasic;


	private ItemCreate ItemCreate;
	private RunPanel RunPanel;

	private void Start()
	{
		ItemCreate = GetComponent<ItemCreate>();
		DayNight = GetComponent<DayNight>();
		Society = GetComponent<Society>();
		Society.DayNight = DayNight;

		Society.AddModel();
		ItemCreate.Init();
		Society.Init();

		GameObject ui = GameObject.Find("UI");
		if (ui != null)
		{
			RunPanel = ui.GetComponentInChildren<RunPanel>(true);
		}

		//GameObject rock = ItemCreate.CreateObject("Rock_A", 490, 485, 12);

		CreateWorld();
		Society.InitWorkPlace();
		Society.RobotJob.CreateDayPlan();

		RunPanel.Init(DayNight, Society);

		UpdateSurface();
	}

	public void CreateWorld()
	{
		CreateWorld_Logic();
	}


	public void UpdateSurface()
	{
		for (int i = 0; i < NavMeshBasic.Count; i++)
		{
			if (NavMeshBasic[i] != null)
			{
				NavMeshBasic[i].UpdateSurface();
			}
		}
	}
}