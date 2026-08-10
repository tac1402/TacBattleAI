using System;
using System.Collections;
using System.Collections.Generic;
using Tac.Agent_;
using Tac.Person_;

namespace Tac.Society
{
	public class RobotJobLogic : JobLogic
	{
		public delegate void AddToCalcPathDelegate(Person argPerson);
		internal AddToCalcPathDelegate AddToCalcPath;

		public void NextHour(GameTime argGameTime)
		{
			CheckAgent(argGameTime);
		}

		private void CheckAgent(GameTime argGameTime)
		{
			foreach (var plan in PersonPlans.Values)
			{
				if (plan.Person.IsBusy == false)
				{
					plan.CalculateActual(argGameTime);

					AgentPoint agentPoint = plan.GetActual();
					if (agentPoint != null)
					{
						plan.Person.TargetPoint = agentPoint.PointPosition;
						plan.Person.PathStatus = 1;
						if (AddToCalcPath != null)
						{
							AddToCalcPath(plan.Person);
						}
						plan.Person.SetTarget(agentPoint.Id);
					}
				}
			}
		}
	}
}