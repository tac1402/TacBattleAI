using System.Collections;
using System.Collections.Generic;
using Tac.ItemMove;
using UnityEngine;



namespace Tac.Agent
{
	//[Component(typeof(TopCamera))]
	public class AgentSelection : MonoBehaviour
    {
		//public DragCamera CameraManager;

		private Agent selectedAgent;
		public Agent SelectedAgent
		{ 
			get { return selectedAgent; }
			set 
			{
				
				if (value == null && selectedAgent != null)
				{
					if (OnAgentDeselect != null)
					{
						OnAgentDeselect(selectedAgent);
					}
				}

				selectedAgent = value;

				if (selectedAgent != null) 
				{
					StatusBar = selectedAgent.SelectionUI.GetComponent<StatusBar>();
					StatusBar.Init();
					if (OnAgentSelect != null)
					{
						OnAgentSelect(selectedAgent);
					}
				}
			}
		}
		public StatusBar StatusBar;

		public event AgentInfo OnAgentSelect;
		public event AgentInfo OnAgentDeselect;



		void Awake()
		{
			//CameraManager.InitAgent = true;
			//CameraManager.OnAgentTap += OnAgentTap;
		}

		public void OnAgentTap(Agent argAgent)
		{
			if (SelectedAgent != null)
			{
				if (SelectedAgent.SelectionUI != null) { StatusBar.Select(false); }
				SelectedAgent = null;
			}

			if (argAgent != null)
			{
				SelectedAgent = argAgent;
				if (SelectedAgent.SelectionUI != null) { StatusBar.Select(true); }
			}
		}
	}

	public partial class Agent
	{
		public GameObject SelectionUI;
	}

}

namespace Tac.ItemMove
{
	public delegate void AgentInfo(Tac.Agent.Agent argAgent);

	/*public partial class DragCamera
	{
		public LayerMask AgentLayer;

		public event AgentInfo OnAgentTap;

		private bool initAgent;
		public bool InitAgent
		{
			get { return initAgent; }
			set 
			{ 
				initAgent = value;
				if (initAgent)
				{
					OnUpdate += DragCamera_OnUpdate;
				}
            }
		}

		private TacAgentLearning.Agent selectedAgent;


		private void DragCamera_OnUpdate()
		{
			UpdateAgentTap();
		}

		private void UpdateAgentTap()
		{
			if (Input.GetMouseButtonUp(0) == false) { return; }

			if (isPanningSceneStarted) { return; }
			if (isDraggingItem) { return; }

			GameObject go = GetAgent(Input.mousePosition);
			if (go != null)
			{
				TacAgentLearning.Agent agentTapped = go.GetComponent<TacAgentLearning.Agent>();

				if (agentTapped != null)
				{
					//Если был выделен, снять выделение
					if (selectedAgent != null && selectedAgent != agentTapped)
					{
						if (OnAgentTap != null)
						{
							OnAgentTap(selectedAgent);
						}
					}

					//Выделить
					selectedAgent = agentTapped;
					if (OnAgentTap != null)
					{
						OnAgentTap(agentTapped);
					}
				}
			}
			else
			{
				selectedAgent = null;
				if (OnAgentTap != null)
				{
					OnAgentTap(null);
				}
			}
		}

		public GameObject GetAgent(Vector2 touch)
		{
			Ray ray = Camera.ScreenPointToRay(touch);
			return GetRaycast(ray, AgentLayer).Item2;
		}


	}*/

}