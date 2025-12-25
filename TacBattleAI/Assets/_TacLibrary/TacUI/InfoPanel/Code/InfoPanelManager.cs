// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using Tac.Agent;
using Tac.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tac.UI
{
	[Component(typeof(AgentPoint), typeof(AgentSelection), typeof(TopCamera))]
	public class InfoPanelManager : MonoBehaviour
	{
		public AgentSelection AgentSelection;
		public TopCamera TopCamera;

		private Dictionary<string, InfoPanel> AllPanel = new Dictionary<string, InfoPanel>();
		private InfoPanel CurrentPanel;

		private AgentPoint currentAgentPoint;
		private Item2 currentItem;
		private Person.Person currentPerson;

		private SelectionType Type = SelectionType.None;
		public void Init()
		{
			AllPanel.Add("BuildingPanel", GetPanel("BuildingPanel"));
			AllPanel.Add("PersonPanel", GetPanel("PersonPanel"));

			if (TopCamera != null)
			{
				TopCamera.InitItem = true;
				TopCamera.OnItemTap += OnItemTap;
			}

			if (AgentSelection != null)
			{
				AgentSelection.OnAgentSelect += OnAgentSelect;
			}

			StartCoroutine(Tick());
		}

		private enum SelectionType
		{
			None = 0,
			AgentPoint = 1,
			Item = 2,
			Person = 3,
		}

		private void ShowHide(SelectionType argType, bool argHide = false)
		{
			if (Type != argType)
			{
				if (CurrentPanel != null)
				{
					CurrentPanel.gameObject.SetActive(false);
				}
				switch (Type)
				{
					case SelectionType.AgentPoint:
						currentAgentPoint = null;
						break;
					case SelectionType.Item:
						currentItem = null;
						break;
					case SelectionType.Person:
						currentPerson = null;
						break;
				}
			}

			if (argHide == false)
			{
				switch (argType)
				{
					case SelectionType.AgentPoint:
						CurrentPanel = AllPanel["BuildingPanel"];
						break;
					case SelectionType.Person:
						CurrentPanel = AllPanel["PersonPanel"];
						break;
				}

				if (CurrentPanel != null)
				{
					CurrentPanel.gameObject.SetActive(true);
				}
				Type = argType;
			}
		}


		private void OnItemTap(Item2 argItem)
		{
			currentItem = argItem;
			if (currentItem != null)
			{
				currentAgentPoint = currentItem.gameObject.GetComponent<AgentPoint>();
				if (currentAgentPoint != null)
				{
					ShowHide(SelectionType.AgentPoint);
				}
				else
				{
					ShowHide(SelectionType.Item);
				}
			}
		}

		public void OnAgentSelect(Agent.Agent argAgent)
		{
			currentPerson = argAgent as Person.Person;
			if (currentPerson != null)
			{
				ShowHide(SelectionType.Person);
			}
		}



		private IEnumerator Tick()
		{
			while (true)
			{
				if (CurrentPanel != null)
				{
					string title = "";
					string info = "";

					switch (Type)
					{
						case SelectionType.AgentPoint:
							title = currentAgentPoint.Title;
							/*string ownerInfo = Society.GetOwnerInfo(currentAgentPoint);

							Business business = currentAgentPoint as Business;
							string businessInfo = "";
							if (business != null)
							{
								float pCompetence = business.CalcOwnerCompetence();
								float bCompetence = business.CalcBusinessCompetence() / 10f;

								int worker = (int) (pCompetence / bCompetence) + 1;
								if (worker <= 10)
								{
									businessInfo = "Компетентность владельца = " + pCompetence.ToString("F1") + " позволяет нанять до " + worker.ToString() + " сотрудников\n";
								}
								else
								{
									businessInfo = "Компетентность владельца = " + pCompetence.ToString("F1") + " позволяет нанять всех сотрудников\n";
								}
							}

							info = "Id = " + currentAgentPoint.Id.ToString() + "\n" + currentAgentPoint.Info + ownerInfo + businessInfo;
							*/
							break;
						case SelectionType.Person:
							title = currentPerson.Name;
							info = currentPerson.InfoTxt;
							break;
					}

					CurrentPanel.SetTitle(title);
					CurrentPanel.SetInfo(info);
				}
				yield return new WaitForSeconds(1.0f);
			}
		}

		private InfoPanel GetPanel(string argPanelName)
		{
			InfoPanel panel = null;
			GameObject ui = GameObject.Find("UI");
			if (ui != null)
			{
				InfoPanel[] infoPanels = ui.GetComponentsInChildren<InfoPanel>(true);
				for (int i = 0; i < infoPanels.Length; i++)
				{
					if (infoPanels[i].Name == argPanelName)
					{
						panel = infoPanels[i];
						break;
					}
				}
			}
			return panel;
		}

	}
}

namespace Tac.Camera
{
	public delegate void ItemInfo(Item2 argItem);

	public partial class TopCamera
	{
		public event ItemInfo OnItemTap;

		private bool initItem;
		public bool InitItem
		{
			get { return initItem; }
			set
			{
				initItem = value;
				if (initItem)
				{
					OnUpdate += Camera_OnUpdateItem;
				}
			}
		}

		private Item2 selectedItem;


		private void Camera_OnUpdateItem()
		{
			UpdateItemTap();
		}

		public void UpdateItemTap()
		{
			if (Mouse.current.leftButton.wasPressedThisFrame == false) { return; }

			GameObject go = GetBuilding(Mouse.current.position.ReadValue());
			Item2 itemTapped = Item2.GetItem(go);

			if (itemTapped != null)
			{
				//Если был выделен, снять выделение
				if (selectedItem != null && selectedItem != itemTapped)
				{
					if (OnItemTap != null)
					{
						OnItemTap(selectedItem);
					}
				}

				//Выделить
				selectedItem = itemTapped;
				if (OnItemTap != null)
				{
					OnItemTap(itemTapped);
				}
			}
			else
			{
				selectedItem = null;
			}
		}

	}

}