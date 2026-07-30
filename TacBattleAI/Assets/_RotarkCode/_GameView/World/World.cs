using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Agent;
using Tac.ItemCreate;
using Tac.Person;
using Tac.Save;
using Tac.Society;
using Tac.UI;
using UnityEF;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class World : Flow, ICell
{
	public WorldLogic logic = new WorldLogic();
	private Society Society => logic.Society;
	private DayNight DayNight => logic.DayNight;


	public List<NavMeshBasic> NavMeshBasic;

	private ItemCreate ItemCreate;
	private RunPanel RunPanel;
	private InfoPanelManager InfoPanelManager;
	private SaveCatalog SaveCatalog;

	public Cell cell { get { return item; } }

	private void Awake()
	{
		UnityDbContext context = new UnityDbContext();
		ItemDb.db = context;

		context.AddTypes("Assembly-CSharp");
		context.AddTypes("TacStandartU");
		context.AddTypes("TacLibrary");

		// Принудительно строим модель – вызовет OnModelCreating
		var model = context.Model;

		context.DebugModel();
		bool isCreated = context.Database.EnsureCreated();
	}

	private void Start()
	{
		ItemCreate = GetComponent<ItemCreate>();
		logic.DayNight = GetComponent<DayNight>();
		logic.Society = GetComponent<Society>();

		//Society = ItemDb<Society>.Create(Society, "", "Society");


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
			SaveCatalog.IDayNight = DayNight as IDayNight;
		}

		CreateWorld();
		Society.InitWorkPlace();

		RunPanel.Init(DayNight, Society);
		DayNight.NextHour += logic.AgentWalkEmulation;

		UpdateSurface();

		item.SaveGraph(this);
	}


	private void Update()
	{
		if (Keyboard.current[Key.F5].wasPressedThisFrame)
		{
			if (SaveCatalog != null)
			{
				if (SaveCatalog.gameObject.activeSelf)
				{
					DayNight.PausePress();
					SaveCatalog.gameObject.SetActive(false);
				}
				else
				{
					if (DayNight.Pause == false) { DayNight.PausePress(); }
					SaveCatalog.gameObject.SetActive(true);
				}
			}
		}
	}

	public void CreateWorld()
	{
		logic.CreateWorld_Logic();
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