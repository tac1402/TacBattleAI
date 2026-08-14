// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using Tac.HealthSystem;

using UnityEF;

#if OnlyUnity
using UnityEngine;
using UnityEngine.AI;
#endif

namespace Tac.Agent_
{
	public partial class Agent : Spatial
	{
        //[TacLogic] AgentLogic logic;
#region Generated Logic
        protected AgentLogic logic
        {
            get
            {
                return baseLogic as AgentLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new AgentLogic();
        }

        public int TargetId => logic.TargetId;
        public bool IsBusy => logic.IsBusy;
        public int LocatedId => logic.LocatedId;
        public bool UseHealthState => logic.UseHealthState;

        public void AddStatsSkills() => logic.AddStatsSkills();
        public void ApplyDamage(float argDamage) => logic.ApplyDamage(argDamage);
        public void ApplyDamage(BodyParts argBodyPart, float argDamage) => logic.ApplyDamage(argBodyPart, argDamage);
        public void CalcHealth() => logic.CalcHealth();
        public void SetTarget(int argId) => logic.SetTarget(argId);
        public void SetLocated(int argId) => logic.SetLocated(argId);
        public void ResetLocated() => logic.ResetLocated();
        public float Health => logic.Health;
        public bool IsDead => logic.IsDead;
        public PhysicalSkill Charge => logic.Charge;
        public PhysicalSkill Precision => logic.Precision;

        public event Change ChangeHealth { add => logic.ChangeHealth += value; remove => logic.ChangeHealth -= value; }

		#endregion


		public string agentName;

		public string Name
		{
			get { return agentName; }
			set
			{
				agentName = value;

#if OnlyUnity
				name = value;
				if (StatusBar != null)
				{
					StatusBar.HealthBar.Text.text = agentName;
				}
#endif
			}
		}

		public int PathStatus
		{
			get { return logic.PathStatus; }
			set
			{
				logic.PathStatus = value;

#if OnlyUnity
				if (StatusBar != null)
				{
					switch (logic.PathStatus)
					{ 
						case 0:
							StatusBar.ChangeMaterial(Color.white);
							break;
						case 1:
							StatusBar.ChangeMaterial(Color.yellow);
							break;
						case 2:
							StatusBar.ChangeMaterial(Color.green);
							break;
					}
				}
#endif
			}
		}

		/// <summary>
		/// Возникает, когда агент заканчивает движение к заданной цели
		/// </summary>
		public event Send OnWalkEnd;

		/// <summary>
		/// Точка на карте куда движется агент
		/// </summary>
		public Vector3_ TargetPoint = Vector3_.zero;

		public void CancelTarget()
		{
#if OnlyUnity
			agent.isStopped = true;
			TargetPoint = Vector3_.zero;
			walkDistance = 0;

			currentPathIndex = 0;
			PathPoints.Clear();
			PathStatus = 0;
#endif
		}


#if OnlyUnity
		public NavMeshAgent agent;

		internal float walkDistance;

		public float WalkDistance { get { return walkDistance; } }

		/// <summary>
		/// Двигается ли юнит к цели
		/// </summary>
		public bool isMoving = false;
		public bool IsMoving
		{
			get { return isMoving; }
			set
			{
				isMoving = value;
				if (isMoving == true)
				{
					StatusBar.ChangeMaterial(Color.green);
				}
				else
				{
					StatusBar.ChangeMaterial(Color.yellow);
				}
			}
		}

		/// <summary>
		/// Текущий путь агента
		/// </summary>
		public LList<LVector3> PathPoints;
		[Mapped]
		private int currentPathIndex;

		/// <summary>
		/// Контроль дистанции, можно использовать только внутри класса, в т.ч. partial
		/// </summary>
		private event Send OnCheckDistance;
		private Vector3 previousPosition;

		internal StatusBar StatusBar;
		private LineRenderer PathRender;
		private float PathHeightOffset = 0.25f;


		public override void InitDataCustom()
		{
			PathPoints = new LList<LVector3>();
		}


		public void Init(bool argRecoverMode = false)
		{
			logic.HealthState = new HealthState(logic.rnd);
			if (argRecoverMode == false)
			{
				logic.Precision.State = 70;
				logic.Charge.State = 100;
			}

			StatusBar = GetComponentInChildren<StatusBar>();
			if (StatusBar != null)
			{
				StatusBar.Init();
				StatusBar.SetHealth(logic.HealthState.Health);
				StatusBar.SetStamina(logic.Charge.State);
			}
			PathRender = GetComponentInChildren<LineRenderer>();

			if (argRecoverMode == false)
			{
				logic.AddStatsSkills();
			}

			agent = GetComponent<NavMeshAgent>();
			if (agent != null)
			{
				agent.enabled = true;
				StartCoroutine(DrawPath());
			}
			logic.ChangeHealth += new Change(Logic_ChangeHealth);

			StartCoroutine(Tick());
		}

		private void Logic_ChangeHealth()
		{
			if (StatusBar != null)
			{
				StatusBar.SetHealth(logic.HealthState.Health, logic.previousHealth);
				StatusBar.SetStamina(logic.Charge.State, logic.previousStamina);
			}
			if (logic.IsDead == true)
			{
				agent.enabled = false;
			}
		}


		/// <summary>
		/// Двигаться к 
		/// </summary>
		public void Walk(Vector3_ argTarget, float stoppingDistance = 0.1f)
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(argTarget.To(), out hit, 100.0f, NavMesh.AllAreas))
			{
				argTarget.From(hit.position);
			}

			if (agent.destination.To2() != argTarget.To2() /*&& IsDead == false*/)
			{
				walkDistance = 0;
				agent.stoppingDistance = stoppingDistance;
				TargetPoint = argTarget;
				agent.SetDestination(argTarget.To());
				if (agent.isStopped)
				{
					agent.isStopped = false;
				}
			}
		}

		public void Walk(float stoppingDistance = 0.1f)
		{
			walkDistance = 0;
			agent.stoppingDistance = stoppingDistance;
			currentPathIndex++;
			if (PathPoints.Count > currentPathIndex)
			{
				agent.SetDestination(PathPoints[currentPathIndex].To());
				if (agent.isStopped)
				{
					agent.isStopped = false;
				}
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				CheckDistance();
				CheckWalkEnd();
				yield return new WaitForSeconds(0.1f);
			}
		}

		private void CheckDistance()
		{
			walkDistance += Vector3.Distance(transform.position, previousPosition);
			if (OnCheckDistance != null)
			{
				OnCheckDistance();
			}
			previousPosition = transform.position;
		}

		private void CheckWalkEnd()
		{
			if (TargetPoint == Vector3_.zero) { return; }
			if (PathStatus != 2) { return; }

			if (PathPoints.Count != 0 && currentPathIndex != PathPoints.Count - 1 && PathPoints.Count > currentPathIndex)
			{
				float d1 = Distance(transform.position, PathPoints[currentPathIndex].To());
				if (d1 <= agent.stoppingDistance)
				{
					currentPathIndex++;
					agent.SetDestination(PathPoints[currentPathIndex].To());
				}
			}
			else
			{
				float d = Distance(transform.position, TargetPoint.To());
				if (d <= agent.stoppingDistance)
				{
					CancelTarget();

					if (OnWalkEnd != null)
					{
						OnWalkEnd(this);
					}
				}
			}
		}

		public int WalkDimension = 2;
		public float Distance(Vector2 from, Vector2 to)
		{ 
			return Distance(from.To3(), to.To3());
		}
		public float Distance(Vector3 from, Vector3 to)
		{ 
			float distance = 0;
			switch (WalkDimension)
			{
				case 2:
					distance = Vector2.Distance(from.To2(), to.To2());
					break;
				case 3:
					distance = Vector3.Distance(from, to);
					break;
			}
			return distance;
		}


		public void Stop()
		{
			try
			{
				agent.isStopped = true;
			}
			catch (System.Exception ex)
			{
				int a = 1;
			}
		}


		/// <summary>
		/// Найти ближайшую доступную позицию на NavMesh карте и поместить в неё агента
		/// </summary>
		public void CheckPosition()
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(transform.position, out hit, 100.0f, NavMesh.AllAreas))
			{
				transform.position = hit.position;
			}
		}

		public void SetPath(NavMeshPath argPath)
		{
			if (argPath.status == NavMeshPath.NavMeshPathStatus.PathComplete)
			{
				for (int i = 0; i < argPath.corners.Length; i++)
				{
					
					PathPoints.Add(new LVector3(argPath.corners[i]));
				}
				currentPathIndex = 0;
				PathStatus = 2;
			}
		}




		private IEnumerator DrawPath()
		{
			WaitForSeconds Wait = new WaitForSeconds(0.1f);

			while (true)
			{
				if (PathPoints != null)
				{
					if (PathRender != null)
					{
						PathRender.positionCount = PathPoints.Count;
						for (int i = 0; i < PathPoints.Count; i++)
						{
							PathRender.SetPosition(i, PathPoints[i].To() + Vector3.up * PathHeightOffset);
						}
					}
				}
				yield return Wait;
			}
		}
#endif

	}


}
