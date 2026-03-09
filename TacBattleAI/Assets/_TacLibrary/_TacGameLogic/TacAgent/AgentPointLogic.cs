using System;
using System.Collections.Generic;

namespace Tac.Agent
{

	public partial class AgentPoint 
	{
		public string Title;
		public string Info;

		public Queue<AgentInPoint> Agents = new Queue<AgentInPoint>();

		/// <summary>
		/// Время последнего обновления
		/// </summary>
		private GameTime LastGameTime;

		public virtual void Init()
		{
		}


		public virtual void Work(GameTime argGameTime) { }


		protected int WorkingFrom
		{
			get
			{
				int x = (int)WorkingHours_.x;
				return x;
			}
		}
		protected int WorkingTill
		{
			get
			{
				int y = (int)WorkingHours_.y;
				return y;
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
			argAgent.IsBusy = true;
			argAgent.TargetId = 0;
			argAgent.LocatedId = ObjectId;
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
			ap.Agent.IsBusy = false;
			ap.Agent.LocatedId = 0;
			ap.Agent.TargetId = 0;
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

		private void CheckEnter(GameTime argGameTime, List<Agent> argAllAgent)
		{
			if (ObjectId == 3)
			{
				int a = 1;
			}

			List<Agent> tmpAgents = argAllAgent.FindAll(x => x.TargetId == ObjectId);

			for (int j = 0; j < tmpAgents.Count; j++)
			{
				if (IsAgentInEnter(tmpAgents[j].ObjectId) == true)
				{
					int timeEnter = WorkingFrom - 1;
					if (argGameTime.Hour >= timeEnter && argGameTime.Hour <= WorkingTill)
					{
						if (CheckAgentToEnter(tmpAgents[j]))
						{
							Add(tmpAgents[j]);
						}
					}
				}
			}
		}

		public void Tick(GameTime argGameTime, List<Agent> argAllAgent)
		{
			LastGameTime = argGameTime;
			CheckEnter(argGameTime, argAllAgent);
			CheckExit(argGameTime);
			UpdateInfo();
		}



#if OnlyLogic
		protected virtual Vector2_ workingHours() { return new Vector2_(0, 24); }
		private Vector2_ WorkingHours_
		{
			get { return new Vector2_(workingHours().x, workingHours().y); }
		}

		public Vector3_ PointPosition
		{
			get { return Vector3_.zero; }
		}

		public virtual void AddView(Agent argAgent) { }
		public void RemoveView(Agent argAgent) { }

		public bool IsAgentInEnter(int argAgentId) { return true; }

#endif


	}


}
