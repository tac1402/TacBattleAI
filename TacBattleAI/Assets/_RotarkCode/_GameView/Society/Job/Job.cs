using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Tac.Society
{
	public partial class Job : MonoBehaviour
	{
		public Queue<Agent.Agent> AgentPath = new Queue<Agent.Agent> ();

		private NavMeshPathExt pathExt;

		private void Awake()
		{
			pathExt = new NavMeshPathExt();
			StartCoroutine(CalcPath());
		}


		private IEnumerator CalcPath()
		{
			while (true)
			{
				for (int i = 0; i < AgentPath.Count; i++)
				{
					Agent.Agent agent = AgentPath.Peek();
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
							AgentPath.Dequeue();
							agent.WalkTeleport();
						}
					}
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

	}
}
