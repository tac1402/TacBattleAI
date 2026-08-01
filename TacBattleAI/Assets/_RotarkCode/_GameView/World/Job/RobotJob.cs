using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
	public class RobotJob : Job
	{
		protected new RobotJobLogic logic => baseLogic as RobotJobLogic;
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


