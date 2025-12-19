// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Tac.HealthSystem;

namespace Tac.Agent
{
	public partial class Agent : Item
	{
		public NavMeshAgent agent;
		public StatusBar StatusBar;
		public AgentAnimator agentAnimator;


		/// <summary>
		/// Заряд
		/// </summary>
		private PhysicalSkill charge = new PhysicalSkill(1, 100);
		/// <summary>
		/// Заряд/Выносливость
		/// </summary>
		public PhysicalSkill Charge { get { return charge; } set { charge = value; } }

		/// <summary>
		/// Меткость
		/// </summary>
		private PhysicalSkill precision = new PhysicalSkill(1, 100);
		/// <summary>
		/// Меткость
		/// </summary>
		public PhysicalSkill Precision { get { return precision; } set { precision = value; } }

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
		/// Возникает, когда агент заканчивает движение к заданной цели
		/// </summary>
		public event Send OnWalkEnd;
		/// <summary>
		/// Цель движения агента, если он движется
		/// </summary>
		public Vector3 WalkTarget = Vector3.zero;

		private System.Random rnd = new System.Random();


		public void Init()
		{
			HealthState = new HealthState(rnd);
			Precision.State = 70;
			Charge.State = 100;

			StatusBar = GetComponentInChildren<StatusBar>();
			if (StatusBar != null)
			{
				StatusBar.Init();
				StatusBar.SetHealth(HealthState.Health);
				StatusBar.SetStamina(Charge.State);
			}

			agent = GetComponent<NavMeshAgent>();
			if (agent != null)
			{
				agent.enabled = true;
			}
			StartCoroutine(Tick());
		}


		/// <summary>
		/// Двигаться к 
		/// </summary>
		public void Walk(Vector3 argTarget, float stoppingDistance = 0.1f)
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(argTarget, out hit, 100.0f, NavMesh.AllAreas))
			{
				argTarget = hit.position;
			}

			if (agent.destination.To2() != argTarget.To2() /*&& IsDead == false*/)
			{
				agent.stoppingDistance = stoppingDistance;
				WalkTarget = argTarget;
				agent.SetDestination(argTarget);
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
				CheckWalkEnd();
				yield return new WaitForSeconds(0.1f);
			}
		}

		public void CheckWalkEnd()
		{
			if (WalkTarget == Vector3.zero) { return; }

			float d = Vector3.Distance(transform.position, WalkTarget);
			if (d < 1.0)
			{
				agent.isStopped = true;
				WalkTarget = Vector3.zero;
				if (OnWalkEnd != null)
				{
					OnWalkEnd(this);
				}
			}
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

		public LineRenderer Path;
		private float PathHeightOffset = 0.25f;

		private IEnumerator DrawPath()
		{
			WaitForSeconds Wait = new WaitForSeconds(0.1f);

			while (true)
			{
				if (agent.path != null)
				{
					if (Path != null)
					{
						Path.positionCount = agent.path.corners.Length;
						for (int i = 0; i < agent.path.corners.Length; i++)
						{
							Path.SetPosition(i, agent.path.corners[i] + Vector3.up * PathHeightOffset);
						}
					}
				}
				yield return Wait;
			}
		}


		public void ApplyDamage(float argDamage)
		{
			BodyParts bodyPart = (BodyParts)rnd.Next(1, 11);
			ApplyDamage(bodyPart, argDamage);
		}

		public void ApplyDamage(BodyParts argBodyPart, float argDamage)
		{
			HealthState.Body[argBodyPart].State -= argDamage;

			CalcHealth();

			if (IsDead == true)
			{
				agent.enabled = false;
			}
		}


		public void CalcHealth()
		{
			float previousHealth = HealthState.Health;
			float previousStamina = Charge.State;

			HealthState.CalcHealth();

			// Расчитать снижение меткости при изменениие здоровья
			Precision.Recalc(Health);

			if (StatusBar != null)
			{
				StatusBar.SetHealth(HealthState.Health, previousHealth);
				StatusBar.SetStamina(Charge.State, previousStamina);
			}
		}

	}


}
