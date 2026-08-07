using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Tac.Agent_;
using UnityEF;
using UnityEngine;
using UnityEngine.AI;
using Tac.Person_;


namespace Tac.Society
{
	public class Job : Flow
	{
        //[TacLogic] JobLogic logic;
#region Generated Logic
        protected JobLogic logic
        {
            get
            {
                return baseLogic as JobLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new JobLogic();
        }

        public Dictionary<int, PersonPlan> PersonPlans => logic.PersonPlans;

        public void AddPersonPlan(Person_.Person argPerson) => logic.AddPersonPlan(argPerson);
        public void CalcAgentPath() => logic.CalcAgentPath();
#endregion

		protected NavMeshPathExt pathExt;

		private void Start()
		{
			pathExt = new NavMeshPathExt();
			logic.pathCalculator = pathExt.CalculatePath;
			StartCoroutine(CalcPath());
		}

		protected IEnumerator CalcPath()
		{
			while (true)
			{
				logic.CalcAgentPath();
				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}
