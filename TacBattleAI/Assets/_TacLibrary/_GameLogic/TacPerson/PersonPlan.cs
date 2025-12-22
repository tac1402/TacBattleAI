using System.Collections;
using System.Collections.Generic;
using Tac.Agent;

namespace Tac.Person
{

	public partial class PersonPlan
	{
		public Person Person;
		public Queue<AgentPoint> DayPlan = new Queue<AgentPoint>();

		public PersonPlan(Person argPerson)
		{
			Person = argPerson;
		}

		public void Add(AgentPoint argAgentPlan)
		{
			DayPlan.Enqueue(argAgentPlan);
		}

		public AgentPoint Remove()
		{
			if (DayPlan.Count > 0)
			{
				AgentPoint agentPoint = DayPlan.Dequeue();
				return agentPoint;
			}
			else { return null; }
		}
	}
}