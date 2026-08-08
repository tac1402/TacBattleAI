using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEF;
using UnityEngine;
using UnityEngine.AI;
using static Tac.Agent_.AgentPointLogic;

namespace Tac.Agent_
{
	
	public class AgentPoint : Spatial
	{
        //[TacLogic] AgentPointLogic logic;
#region Generated Logic
        protected AgentPointLogic logic
        {
            get
            {
                return baseLogic as AgentPointLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new AgentPointLogic();
        }

        public string Title => logic.Title;
        public LQueue<AgentInPoint> Agents => logic.Agents;
        public List<NamedValue> BaseWorkPayment => logic.BaseWorkPayment;
        public GetInfoDelegate GetInfoHandler { set => logic.GetInfoHandler = value; }

        public bool IsOpen(int currentTime) => logic.IsOpen(currentTime);
        public void Add(Agent argAgent) => logic.Add(argAgent);
        public Agent Remove() => logic.Remove();
        public Agent Remove(int argAgentId) => logic.Remove(argAgentId);
        public void WalkToEnter(GameTime argGameTime, Agent argAgent) => logic.WalkToEnter(argGameTime, argAgent);
        public void Tick(GameTime argGameTime, List<Agent> argAllAgent) => logic.Tick(argGameTime, argAllAgent);
        public override void InitData()
        {
            base.InitData();
            logic.AddView = AddView;
            logic.RemoveView = RemoveView;
            logic.IsAgentInEnter = IsAgentInEnter;
            logic.Agents = new LQueue<AgentInPoint>();
        }

        public string GetInfo() => logic.GetInfo();
#endregion

		/// <summary>
		/// Рабочие часы (не находятся в логике, т.к. задаются в Юнити редакторе)
		/// </summary>
		public Vector2 WorkingHours;

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

		public LayerMask AgentLayer;


		public override void InitDataCustom()
		{
			BuildItem item = GetComponent<BuildItem>();
			if (item != null)
			{
				Id = item.Id;
			}

			logic.workingFrom = (int)WorkingHours.x;
			logic.workingTill = (int)WorkingHours.y;
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

				if (agent != null && agent.Id == argAgentId && agent.TargetId == Id)
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
