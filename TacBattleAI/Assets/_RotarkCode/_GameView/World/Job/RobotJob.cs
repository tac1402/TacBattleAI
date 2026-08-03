using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
	public class RobotJob : Job
	{
		protected new RobotJobLogic logic { get { return baseLogic as RobotJobLogic; } set { baseLogic = value; } }
		protected override void CreateLogic()
		{
			pathExt = new NavMeshPathExt();
			baseLogic = new RobotJobLogic(pathExt.CalculatePath);
		}


		private void Awake()
		{
			StartCoroutine(CalcPath());
		}

		public void NextHour(GameTime argGameTime) => logic.NextHour(argGameTime);
	}
}


