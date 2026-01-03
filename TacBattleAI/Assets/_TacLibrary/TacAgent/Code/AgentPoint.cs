using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Tac.Agent
{
	public partial class AgentPoint : Item
	{
		public Vector3 Size = new Vector3(10, 3, 10);
		public GameObject Point;

		public Vector3_ PointPosition
		{
			get { return Point.transform.position.To2().To3().To_(); } // Обнуление высоты
		}

		private Vector3 NearPosition(Vector3 position)
		{
			NavMeshHit hit;
			Vector3 ret = Vector3.zero;
			if (NavMesh.SamplePosition(position, out hit, 100.0f, NavMesh.AllAreas))
			{
				ret = hit.position;
			}
			return ret;
		}


		public Vector3 EnterSize = new Vector3(2, 2, 2);
		public bool HideAgent = true;
		public Vector3 TruckPointSize = new Vector3(4, 2, 4);

		public LayerMask AgentLayer;

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

		/// <summary>
		/// Находится ли агент на входе
		/// </summary>
		public bool IsAgentInEnter(int argAgentId)
		{
			bool ret = false;
			Collider[] c = Physics.OverlapBox(Point.transform.position, EnterSize / 2f, Point.transform.rotation, AgentLayer);
			for (int j = 0; j < c.Length; j++)
			{
				Agent agent = c[j].gameObject.GetComponent<Agent>();

				if (agent != null && agent.ObjectId == argAgentId && agent.TargetId == ObjectId)
				{ 
					ret = true; 
					break;
				}
			}
			return ret;
		}


			void OnDrawGizmos()
		{
			if (Point != null)
			{
				// Сохраняем текущую матрицу Gizmos
				Matrix4x4 originalMatrix = Gizmos.matrix;

				// Устанавливаем матрицу с позицией и поворотом
				Gizmos.matrix = Matrix4x4.TRS(
					Point.transform.position,  // позиция
					Point.transform.rotation,  // поворот (если нужно использовать поворот объекта Point)
					Vector3.one                // масштаб
				);

				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube(Vector3.zero, EnterSize);

				// Восстанавливаем оригинальную матрицу
				Gizmos.matrix = originalMatrix;
			}
		}

	}



}
