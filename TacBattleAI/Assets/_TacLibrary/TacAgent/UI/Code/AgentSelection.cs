using System.Collections;
using System.Collections.Generic;
using Tac.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tac.Agent
{
	[Component(typeof(Agent), typeof(StatusBar), typeof(TopCamera))]
	public class AgentSelection : MonoBehaviour
    {
		public TopCamera TopCamera;
		public StatusBar StatusBar;

		public event AgentInfo OnAgentSelect;
		public event AgentInfo OnAgentDeselect;


		private Agent selectedAgent;
		public Agent SelectedAgent
		{ 
			get { return selectedAgent; }
			internal set 
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


		void Awake()
		{
			TopCamera.InitAgent = true;
			TopCamera.OnAgentTap += OnAgentTap;
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

namespace Tac.Camera
{
	public delegate void AgentInfo(Agent.Agent argAgent);

	public partial class TopCamera
	{
		public LayerMask AgentLayer;

		internal event AgentInfo OnAgentTap;

		private bool initAgent;
		internal bool InitAgent
		{
			get { return initAgent; }
			set 
			{ 
				initAgent = value;
				if (initAgent)
				{
					OnUpdate += Camera_OnUpdateAgent;
				}
            }
		}

		private Agent.Agent selectedAgent;


		private void Camera_OnUpdateAgent()
		{
			UpdateAgentTap();
		}

		private void UpdateAgentTap()
		{
			if (Mouse.current.leftButton.wasPressedThisFrame == false) { return; }

			GameObject go = GetAgent(Mouse.current.position.ReadValue());
			if (go != null)
			{
				Agent.Agent agentTapped = go.GetComponent<Agent.Agent>();

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

		/// <summary>
		/// Получить агента по координатам, выполнив рейкаст
		/// </summary>
		internal protected GameObject GetAgent(Vector2 touch)
		{
			Ray ray = camera.ScreenPointToRay(touch);
			return GetRaycast(ray, AgentLayer).Item2;
		}


	}

}