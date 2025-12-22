using System;
using System.Collections;
using System.Collections.Generic;


namespace Tac.Society
{
	public partial class PlayerJob : Job
	{
		public override void Init(DayNight argDayNight)
		{
			base.Init(argDayNight);

			DayNight.NextHour += NextHour;
		}

		private void NextHour(GameTime argGameTime)
		{
			//CheckAgent();
		}

		bool isDayPlanCreated = false;
		/*private void CheckAgent()
		{

			if (DayNightController.Time >= new TimeSpan(6, 0, 0) && DayNightController.Time <= new TimeSpan(7, 0, 0) && isDayPlanCreated == true)
			{
				isDayPlanCreated = false;
			}

			// Строим план на сутки в 7.00
			if (DayNightController.Time >= new TimeSpan(7, 0, 0) && DayNightController.Time <= new TimeSpan(8, 0, 0) && isDayPlanCreated == false)
			{
				foreach (var plan in AgentPlans.Values)
				{
					if (WorkMode == true)
					{
						WorkError = CheckJob(plan.Person.ObjectId, Factory, true);
					}
					else if (plan.Person.WorkPlace != null)
					{
						FireFromBusiness(plan.Person);
					}
				}

				CreateDayPlan();
				isDayPlanCreated = true;
			}


			foreach (var plan in AgentPlans.Values)
			{
				if (plan.Person.IsBusy == false)
				{
					AgentPoint agentPoint = plan.Remove();
					if (agentPoint != null)
					{
						plan.Person.TargetId = agentPoint.Id;
						plan.Person.Walk(agentPoint.PointPosition);
						plan.Person.IsBusy = true;
					}
				}
			}
		}*/

	}
}
