using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Agent;
using Tac.ItemCreate;
using Tac.Society;
using Tac.UI;

using UnityEngine;
using UnityEngine.InputSystem;

public partial class World : Item
{
	public List<NavMeshBasic> NavMeshBasic;


	private ItemCreate ItemCreate;
	private RunPanel RunPanel;
	private InfoPanelManager InfoPanelManager;
	private SaveCatalog SaveCatalog;

	private void Start()
	{
		ItemCreate = GetComponent<ItemCreate>();
		DayNight = GetComponent<DayNight>();
		Society = GetComponent<Society>();

		Society.AddModel();
		ItemCreate.Init();
		Society.Init();

		GameObject ui = GameObject.Find("UI");
		if (ui != null)
		{
			RunPanel = ui.GetComponentInChildren<RunPanel>(true);
			InfoPanelManager = ui.GetComponentInChildren<InfoPanelManager>(true);
			InfoPanelManager.Init();
			SaveCatalog = ui.GetComponentInChildren<SaveCatalog>(true);
			SaveManager saveManager = GetComponent<SaveManager>();
			saveManager.World = this;
			SaveCatalog.ISaveManager = saveManager;
			SaveCatalog.IDayNight = DayNight as IDayNight;
		}

		//GameObject rock = ItemCreate.CreateObject("Rock_A", 490, 485, 12);

		CreateWorld();
		Society.InitWorkPlace();
		Society.RobotJob.CreateDayPlan();

		RunPanel.Init(DayNight, Society);

		UpdateSurface();

		DayNight.NextHour += AgentWalkEmulation;
	}

	private void Update()
	{
		if (Keyboard.current[Key.F5].wasPressedThisFrame)
		{
			if (SaveCatalog != null)
			{
				if (SaveCatalog.gameObject.activeSelf)
				{
					SaveCatalog.gameObject.SetActive(false);
				}
				else
				{
					SaveCatalog.gameObject.SetActive(true);
				}
			}
		}
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