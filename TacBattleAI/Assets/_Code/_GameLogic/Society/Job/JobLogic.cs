using System.Collections;
using System.Collections.Generic;
using Tac.Person;

namespace Tac.Society
{
	public partial class Job
	{
		/// <summary>
		/// Планы персонажей на игровой день
		/// </summary>
		public Dictionary<int, PersonPlan> PersonPlans = new Dictionary<int, PersonPlan>();


		private bool IsInit = false;

		public void AddPersonPlan(Person.Person argPerson)
		{
			PersonPlans.Add(argPerson.ObjectId, new PersonPlan(argPerson));
		}


		public void CreateDayPlan()
		{
			foreach (var plan in PersonPlans.Values)
			{
				plan.DayPlan.Clear();

				plan.Add(plan.Person.WorkPlace);
			}
		}

	}
}
