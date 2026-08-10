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
#endregion

		protected NavMeshPathExt pathExt;
		protected List<Agent> AgentPath = new List<Agent>();

		public override void InitDataCustom()
		{
			pathExt = new NavMeshPathExt();
		}

		private void Start()
		{
			StartCoroutine(CalcPath());
		}

		protected IEnumerator CalcPath()
		{
			while (true)
			{
				CalcAgentPath();
				yield return new WaitForSeconds(0.1f);
			}
		}

		private void CalcAgentPath()
		{
			for (int i = AgentPath.Count - 1; i >= 0; i--)
			{
				Agent agent = AgentPath[i];
				if (agent.PathStatus == 1)
				{
					NavMeshPath path = pathExt.CalculatePath(agent.Position, agent.TargetPoint);
					agent.SetPath(path);


					if (agent.PathStatus != 2)
					{
						break;
					}
					else
					{
						AgentPath.RemoveAt(i);
						agent.WalkTeleport();
					}
				}
			}
		}
	}
}
