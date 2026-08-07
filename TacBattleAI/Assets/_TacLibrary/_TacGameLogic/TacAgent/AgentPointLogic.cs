using System;
using System.Collections.Generic;
using UnityEF;

namespace Tac.Agent_
{
	public class AgentPointLogic : Logic
	{

		public delegate void AddViewDelegate(Agent agent);
		public delegate void RemoveViewDelegate(Agent agent);
		public delegate bool IsAgentInEnterDelegate(int agentId);

		public AddViewDelegate AddView;
		public RemoveViewDelegate RemoveView;
		public IsAgentInEnterDelegate IsAgentInEnter;

		public string Title;
		public string Info;

		public LQueue<AgentInPoint> Agents = new LQueue<AgentInPoint>();

		/// <summary>
		/// Время последнего обновления
		/// </summary>
		private GameTime LastGameTime;

		/// <summary>
		/// Список ресурсов, которые агент может получить работая в этой точке в базовых величинах.
		/// В качестве именований нужно использовать теже наименования что и в статах персонажа. 
		/// </summary>
		public List<NamedValue> BaseWorkPayment = new List<NamedValue>();

		internal int workingFrom;
		protected int WorkingFrom
		{
			get { return workingFrom; } 
		}
		internal int workingTill;
		protected int WorkingTill
		{
			get { return workingTill; }
		}


		public virtual void Work(GameTime argGameTime) { }


		public bool IsOpen(int currentTime)
		{
			if (WorkingFrom <= WorkingTill)
			{
				// Обычный интервал в пределах суток
				return currentTime >= WorkingFrom && currentTime <= WorkingTill;
			}
			else
			{
				// Интервал переходит через полночь (например, 22:00 - 06:00)
				return currentTime >= WorkingFrom || currentTime <= WorkingTill;
			}
		}

		public virtual bool CheckAgentToEnter(Agent argAgent)
		{
			return true;
		}


		public virtual bool CheckAgentToExit(Agent argAgent)
		{
			return false;
		}

		protected virtual void UpdateInfo() { }


		public virtual void Add(Agent argAgent)
		{
			argAgent.SetLocated(Id);
			AgentInPoint point = new AgentInPoint();
			point.Agent = argAgent;
			point.EnterTime = LastGameTime;
			Agents.Enqueue(point);

			AddView(argAgent);
		}

		public Agent Remove()
		{
			AgentInPoint ap = Agents.Dequeue();
			RemoveView(ap.Agent);
			ap.Agent.ResetLocated();
			return ap.Agent;
		}

		public Agent Remove(int argAgentId)
		{
			AgentInPoint ap = Agents.Remove(argAgentId);
			RemoveView(ap.Agent);
			ap.Agent.ResetLocated();
			return ap.Agent;
		}

		private void CheckExit(GameTime argGameTime)
		{
			if (Agents.Count > 0)
			{
				int tmpAgentCount = Agents.Count;
				for (int i = 0; i < tmpAgentCount; i++)
				{
					AgentInPoint ap = Agents.Peek();
					bool retExit = CheckAgentToExit(ap.Agent);

					if (argGameTime.Hour >= WorkingTill)
					{
						Agent agent = Remove();
					}
					else if (retExit == true)
					{
						Agent agent = Remove();
					}
				}
			}
		}

		public bool DebugEnter = false;

		private void CheckEnter(GameTime argGameTime, List<Agent> argAllAgent)
		{
			if (DebugEnter)
			{
				int a = 1;
			}

			List<Agent> tmpAgents = argAllAgent.FindAll(x => x.TargetId == Id);

			for (int j = 0; j < tmpAgents.Count; j++)
			{
				if (IsAgentInEnter(tmpAgents[j].Id) == true)
				{
					WalkToEnter(argGameTime, tmpAgents[j]);
				}
			}
		}

		public void WalkToEnter(GameTime argGameTime, Agent argAgent)
		{
			int timeEnter = WorkingFrom - 1;
			if (argGameTime.Hour >= timeEnter && argGameTime.Hour <= WorkingTill)
			{
				if (CheckAgentToEnter(argAgent))
				{
					Add(argAgent);
				}
			}
		}

		public virtual void Tick(GameTime argGameTime, List<Agent> argAllAgent)
		{
			LastGameTime = argGameTime;
			CheckEnter(argGameTime, argAllAgent);
			CheckExit(argGameTime);
			UpdateInfo();
		}

		public delegate string GetInfoDelegate(int argId);
		public GetInfoDelegate GetInfoHandler;
		public string GetInfo()
		{
			string ret = Info;
			if (GetInfoHandler != null)
			{
				ret += "\n" + GetInfoHandler(Id);
			}
			return ret;
		}


	}


}
