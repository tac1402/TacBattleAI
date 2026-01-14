using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Tac;
using Tac.Agent;
using Tac.DConvert;
using Tac.ItemCreate;
using Tac.Person;
using Tac.Society;
using Tac.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class World : Item, ILoadManager
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
		DayNight.NextHour += AgentWalkEmulation;

		UpdateSurface();
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

	#region ILoadManager

	/// <summary>
	/// Все объекты добавленные на сцену
	/// </summary>
	private Dictionary<int, GameObject> allObject = new Dictionary<int, GameObject>();
	public Dictionary<int, GameObject> AllObject { get { return allObject; } }

	public IObject IObject { get { return ItemCreate; } }

	public void ResetGame()
	{
		(this as ILoadManager).ResetGameInner();
		(this as ILoadManager).ResetEvent(DayNight, "NextDay");
		(this as ILoadManager).ResetEvent(DayNight, "NextHour");

		Society.People.Clear();

		/*World.Society.AllBusiness.Clear();

		World.Society.RobotJob.AgentPlans.Clear();
		World.Society.PlayerJob.AgentPlans.Clear();
		*/
	}

	public void RecoverGame()
	{
		ILoadManager iLoad = (this as ILoadManager);

		RunPanel.Init(DayNight, Society);
		DayNight.NextHour += AgentWalkEmulation;

		UpdateSurface();

		foreach (Person p in Society.People.Values)
		{
			if (p.IsActive == false)
			{
				p.IsActive = true;
				p.Init(true);
				p.IsActive = false;
			}
			else
			{
				p.Init(true);
			}
			if (p.IsBusy && p.TargetId != 0)
			{
				p.Walk(p.TargetPoint);
			}
			/*
			p.Places.Clear();
			foreach (var item in p.LoadPlacesId)
			{
				GameObject obj = iLoad.GetObject(item.Value);
				AgentPoint ap = obj.GetComponent<AgentPoint>();
				p.Places.Add(item.Key, ap);
			}*/
		}

	}

	#endregion
}