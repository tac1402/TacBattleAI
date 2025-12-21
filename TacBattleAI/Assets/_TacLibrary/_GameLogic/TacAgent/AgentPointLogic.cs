using System;
using System.Collections.Generic;

namespace Tac.Agent
{

	public partial class AgentPoint 
	{
		public string Title;
		public string Info;

		public Queue<AgentInPoint> Agents = new Queue<AgentInPoint>();

		//public DayNightController DayNightController;

		/*protected virtual void ChangeType() { }
		private PropertyType propertyType = PropertyType.None;

		public PropertyType PropertyType
		{
			get { return propertyType; }
			set
			{
				propertyType = value;
				ChangeType();
			}
		}*/

		public virtual void Init() 
		{
			//DayNightController.NextHour += Work;
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
		protected virtual void CheckTruck() { }

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
			//point.EnterTime = DayNightController.DateTimeNow;
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
		public Vector3_ TruckPointPosition
		{
			get { return Vector3_.zero; }
		}

		public virtual void AddView(Agent argAgent) { }
		public void RemoveView(Agent argAgent) { }

		private void CheckEnter(GameTime argGameTime, List<Agent> argAllAgent)
		{
			if (Id == 3)
			{
				int a = 1;
			}

			List<Agent> tmpAgents = argAllAgent.FindAll(x => x.TargetId == Id);

			for (int j = 0; j < tmpAgents.Count; j++)
			{
				int timeEnter = WorkingFrom - 1 ;
				if (argGameTime.Hour >= timeEnter && argGameTime.Hour <= WorkingTill)
				{
					if (CheckAgentToEnter(tmpAgents[j]))
					{
						Add(tmpAgents[j]);
					}
				}
			}
		}

		public void Tick(GameTime argGameTime, List<Agent> argAllAgent)
		{
			if (Id == 1)
			{
				int a = 1;
			}

			CheckEnter(argGameTime, argAllAgent);
			CheckExit(argGameTime);
			CheckTruck();
			UpdateInfo();
		}

#endif


	}


}
