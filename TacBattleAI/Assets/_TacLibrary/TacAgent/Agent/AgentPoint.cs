using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tac.Agent
{
	public partial class AgentPoint : Item
	{
		public Vector3 Size = new Vector3(10, 3, 10);
		public GameObject Point;

		public Vector3_ PointPosition
		{
			get { return Point.transform.position.To_(); }
		}
		public GameObject TruckPoint;
		public Vector3_ TruckPointPosition
		{
			get { return TruckPoint.transform.position.To_(); }
		}


		public Vector3 EnterSize = new Vector3(2, 2, 2);
		public bool HideAgent = true;
		public Vector3 TruckPointSize = new Vector3(4, 2, 4);

		public LayerMask AgentLayer;
		public LayerMask TruckLayer;

		/// <summary>
		/// Рабочие часы
		/// </summary>
		public Vector2 WorkingHours;

		private Vector2_ WorkingHours_
		{
			get { return new Vector2_(WorkingHours.x, WorkingHours.y); }
		}



		private void Awake()
		{
			GameObject world = GameObject.Find("World");
			if (world != null)
			{ 
				//DayNightController = world.GetComponent<DayNightController>();
			}
			Item2 item = GetComponent<Item2>();
			if (item != null)
			{
				ObjectId = item.ObjectId;
			}

			Init();

			StartCoroutine(Tick());
		}

		public virtual void AddView(Agent argAgent)
		{
			if (HideAgent == true)
			{
				argAgent.gameObject.SetActive(false);
			}
		}

		public void RemoveView(Agent argAgent)
		{
			if (HideAgent == true)
			{
				argAgent.gameObject.SetActive(true);
			}
			argAgent.agent.velocity = new Vector3(0, 0, 0);
		}



		private IEnumerator Tick()
		{
			while (true)
			{
				CheckEnter();
				//CheckExit();
				CheckTruck();
				UpdateInfo();
				yield return new WaitForSeconds(1.0f);
			}
		}

		private void CheckEnter()
		{
			//Collider[] c = Physics.OverlapBox(Point.transform.position, EnterSize / 2f, Quaternion.identity, AgentLayer);
			/*for (int j = 0; j < c.Length; j++)
			{
				TimeSpan timeEnter = WorkingFrom.Subtract(new TimeSpan(1, 0, 0));
				if (DayNightController.Time >= timeEnter && DayNightController.Time <= WorkingTill)
				{
					Agent agent = c[j].gameObject.GetComponent<Agent>();

					if (agent.TargetId == Id)
					{
						if (CheckAgentToEnter(agent))
						{
							Add(agent);
						}
					}
				}
			}*/

		}

		protected void CheckExit(GameTime argGameTime)
		{
			if (Agents.Count > 0)
			{
				int tmpAgentCount = Agents.Count;
				for (int i = 0; i < tmpAgentCount; i++)
				{
					AgentInPoint ap = Agents.Peek();
					bool retExit = CheckAgentToExit(ap.Agent);

					if (argGameTime.Hour >= WorkingTill)
					{
						Agent agent = Remove();
					}
					else if (retExit == true)
					{
						Agent agent = Remove();
					}
				}
			}
		}

		void OnDrawGizmos()
		{
			if (Point != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube(Point.transform.position, EnterSize);
			}
			if (TruckPoint != null)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(TruckPoint.transform.position, TruckPointSize);
			}
		}

	}



}
