using System.Collections;
using System.Collections.Generic;
using Tac;
using Tac.Agent_;
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

		public void AddPersonPlan(Person_.Person argPerson)
		{
			PersonPlans.Add(argPerson.Id, new PersonPlan(argPerson));
		}


	}


}

