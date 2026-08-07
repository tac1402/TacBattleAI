using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac.Society
{
	public class RobotJob : Job
	{
        //[TacLogic] RobotJobLogic logic;
#region Generated Logic
        protected RobotJobLogic logic
        {
            get
            {
                return baseLogic as RobotJobLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new RobotJobLogic();
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


