using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
    public class PlayerJob : Job
    {
        //[TacLogic] PlayerJobLogic logic;
#region Generated Logic
        protected PlayerJobLogic logic
        {
            get
            {
                return baseLogic as PlayerJobLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new PlayerJobLogic();
        }

        public void NextHour(GameTime argGameTime) => logic.NextHour(argGameTime);
#endregion

		private void Start()
		{
			pathExt = new NavMeshPathExt();
			logic.pathCalculator = pathExt.CalculatePath;
			StartCoroutine(CalcPath());
		}

	}
}
