using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Person_;
using UnityEF;

namespace Tac.Society
{
	public class JobLogic : Logic
	{
		/// <summary>
		/// Планы персонажей на игровой день
		/// </summary>
		public Dictionary<int, PersonPlan> PersonPlans = new Dictionary<int, PersonPlan>();

		protected List<Agent_.Agent> AgentPath = new List<Agent_.Agent>();

		public delegate NavMeshPath PathCalculator(Vector3_ from, Vector3_ to);
		internal PathCalculator pathCalculator;

		public void AddPersonPlan(Person_.Person argPerson)
		{
			PersonPlans.Add(argPerson.Id, new PersonPlan(argPerson));
		}

		public void CalcAgentPath()
		{
			for (int i = AgentPath.Count - 1; i >= 0; i--)
			{
				Agent_.Agent agent = AgentPath[i];
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

