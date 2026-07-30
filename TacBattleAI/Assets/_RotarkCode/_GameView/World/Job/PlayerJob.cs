using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
    public class PlayerJob : Job
    {
		private new PlayerJobLogic logic;
		private void Awake()
		{
			pathExt = new NavMeshPathExt();
			logic = new PlayerJobLogic(pathExt.CalculatePath);
			StartCoroutine(CalcPath());
		}

		public void NextHour(GameTime argGameTime) => logic.NextHour(argGameTime);
	}
}
