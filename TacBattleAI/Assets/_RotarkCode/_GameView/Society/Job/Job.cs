using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DnaCore;
using UnityEF;

namespace Tac.Society
{
	public partial class Job : Flow, ICell
	{
		public LList<Agent.Agent> AgentPath;

		private NavMeshPathExt pathExt;

		public Cell cell { get { return item; } }


		public override void InitData()
		{
			base.InitData();
			AgentPath = new LList<Agent.Agent>();
		}

		private void Awake()
		{
			pathExt = new NavMeshPathExt();
			StartCoroutine(CalcPath());
		}

		private IEnumerator CalcPath()
		{
			while (true)
			{
				for (int i = AgentPath.Count - 1; i >= 0; i--)
				{
					Agent.Agent agent = AgentPath[i];
					if (agent.PathStatus == 1)
					{
						NavMeshPath2 path = pathExt.CalculatePath(agent.transform.position, agent.TargetPoint.To());
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
				yield return new WaitForSeconds(0.1f);
			}
		}

	}
}
