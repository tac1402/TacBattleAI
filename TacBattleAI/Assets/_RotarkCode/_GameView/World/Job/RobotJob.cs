using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
	public class RobotJob : Job
	{
		private new RobotJobLogic logic;
		private void Awake()
		{
			pathExt = new NavMeshPathExt();
			logic = new RobotJobLogic(pathExt.CalculatePath);
			StartCoroutine(CalcPath());
		}

		public void NextHour(GameTime argGameTime) => logic.NextHour(argGameTime);
	}
}


