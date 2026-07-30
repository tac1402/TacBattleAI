using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Person;
using UnityEF;

namespace Tac.Society
{
	public class JobLogic : Logic
	{
		/// <summary>
		/// Планы персонажей на игровой день
		/// </summary>
		public Dictionary<int, PersonPlan> PersonPlans = new Dictionary<int, PersonPlan>();

		protected List<Agent.Agent> AgentPath = new List<Agent.Agent>();

		public delegate NavMeshPath PathCalculator(Vector3_ from, Vector3_ to);
		protected PathCalculator pathCalculator;

		public JobLogic() { }
		public JobLogic(PathCalculator argPathCalculator) 
		{
			pathCalculator = argPathCalculator;
		}

		public void AddPersonPlan(Person.Person argPerson)
		{
			PersonPlans.Add(argPerson.Id, new PersonPlan(argPerson));
		}

		public void CalcPath()
		{
			for (int i = AgentPath.Count - 1; i >= 0; i--)
			{
				Agent.Agent agent = AgentPath[i];
				if (agent.PathStatus == 1)
				{
					NavMeshPath path = pathCalculator(agent.transform.position.To_(), agent.TargetPoint);
					agent.SetPath(path);


					if (agent.PathStatus != 2)
					{
						break;
					}
					else
					{
						AgentPath.RemoveAt(i);
						agent.WalkTeleport();
					}
				}
			}
		}

	}


}

