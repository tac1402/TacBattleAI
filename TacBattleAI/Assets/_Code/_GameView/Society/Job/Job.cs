using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tac.Society
{
	public partial class Job : MonoBehaviour
	{
		protected Queue<Agent.Agent> AgentPath = new Queue<Agent.Agent> ();


		private void Awake()
		{
			StartCoroutine(CalcPath());
		}


		private IEnumerator CalcPath()
		{
			while (true)
			{
				for (int i = 0; i < AgentPath.Count; i++)
				{
					Agent.Agent agent = AgentPath.Peek();
					if (agent.PathStatus == 1 || agent.PathStatus == 3)
					{
						agent.CalculatePath();
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
