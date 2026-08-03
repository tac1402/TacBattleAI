using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
    public class PlayerJob : Job
    {
		protected new PlayerJobLogic logic { get { return baseLogic as PlayerJobLogic; } set { baseLogic = value; } }
		protected override void CreateLogic()
		{
			pathExt = new NavMeshPathExt();
			baseLogic = new PlayerJobLogic(pathExt.CalculatePath);
		}

		private void Awake()
		{
			StartCoroutine(CalcPath());
		}

		public void NextHour(GameTime argGameTime) => logic.NextHour(argGameTime);
	}
}
