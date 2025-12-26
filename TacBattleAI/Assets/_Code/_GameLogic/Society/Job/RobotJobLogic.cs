using System;
using System.Collections;
using System.Collections.Generic;
using Tac.Agent;

namespace Tac.Society
{
	public partial class RobotJob : Job
	{

		public void NextHour(GameTime argGameTime)
		{
			CheckAgent(argGameTime);
		}


		bool isDayPlanCreated = false;
		private void CheckAgent(GameTime argGameTime)
		{
			if (argGameTime.Hour >= 6 && argGameTime.Hour <= 7 && isDayPlanCreated == true)
			{
				isDayPlanCreated = false;
			}

			// Строим план на сутки в 7.00
			if (argGameTime.Hour >= 7 && argGameTime.Hour <= 8 && isDayPlanCreated == false)
			{
				CreateDayPlan();
				isDayPlanCreated = true;
			}

			foreach (var plan in PersonPlans.Values)
			{
				if (plan.Person.IsBusy == false)
				{
					AgentPoint agentPoint = plan.Remove();
					if (agentPoint != null)
					{
						/*if (agentPoint.ObjectId == 3)
						{
							int a = 1;
						}*/

						plan.Person.TargetId = agentPoint.ObjectId;
						plan.Person.Walk(agentPoint.PointPosition);
						plan.Person.IsBusy = true;
						plan.Person.LocatedId = -1;
					}
				}
			}
		}
	}
}