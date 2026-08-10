using System;
using System.Collections;
using System.Collections.Generic;

using Tac.Person_;

namespace Tac.Society
{
	public class RobotJob : Job
	{
        //[TacLogic] RobotJobLogic logic;
#region Generated Logic
        protected new RobotJobLogic logic
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

        public override void InitData()
        {
            base.InitData();
            logic.AddToCalcPath = AddToCalcPath;
        }

        public void NextHour(GameTime argGameTime) => logic.NextHour(argGameTime);
#endregion

		private void Start()
		{
			StartCoroutine(CalcPath());
		}

        public void AddToCalcPath(Person argPerson)
        {
			AgentPath.Add(argPerson);
		}

	}
}


