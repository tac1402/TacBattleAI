using System.Collections;
using System.Collections.Generic;
using Tac.Person;
using UnityEF;

namespace Tac.Society
{
	public partial class Job
	{
		/// <summary>
		/// Планы персонажей на игровой день
		/// </summary>
		public Dictionary<int, PersonPlan> PersonPlans = new Dictionary<int, PersonPlan>();


		public void AddPersonPlan(Person.Person argPerson)
		{
			PersonPlans.Add(argPerson.Id, new PersonPlan(argPerson));
		}


	}
}
