// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tac.HealthSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Tac.Agent
{
	public partial class Agent : Item
	{
		public NavMeshAgent agent;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				if (StatusBar != null)
				{
					StatusBar.HealthBar.Text.text = name;
				}
			}
		}


		public float WalkDistance;

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

		public int PathStatus = 0; // 0 - нет пути, 1 - нужно посчитать, 2 - путь расчитан

		/// <summary>
		/// Возникает, когда агент заканчивает движение к заданной цели
		/// </summary>
		public event Send OnWalkEnd;

		/// <summary>
		/// Цель движения агента, если он движется
		/// </summary>
		public Vector3_ TargetPoint = Vector3_.zero;

		/// <summary>
		/// Контроль дистанции, можно использовать только внутри класса, в т.ч. partial
		/// </summary>
		private event Send OnCheckDistance;
		private Vector3 previousPosition;

		private System.Random rnd = new System.Random();


		internal StatusBar StatusBar;
		private LineRenderer PathRender;
		private float PathHeightOffset = 0.25f;

		public void Init(bool argRecoverMode = false)
		{
			HealthState = new HealthState(rnd);
			if (argRecoverMode == false)
			{
				Precision.State = 70;
				Charge.State = 100;
			}

			StatusBar = GetComponentInChildren<StatusBar>();
			if (StatusBar != null)
			{
				StatusBar.Init();
				StatusBar.SetHealth(HealthState.Health);
				StatusBar.SetStamina(Charge.State);
			}
			PathRender = GetComponentInChildren<LineRenderer>();

			if (argRecoverMode == false)
			{
				AddStatsSkills();
			}

			agent = GetComponent<NavMeshAgent>();
			if (agent != null)
			{
				agent.enabled = true;
				StartCoroutine(DrawPath());
			}
			StartCoroutine(Tick());
		}

		public virtual void AddStatsSkills() { }

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
				WalkDistance = 0;
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
			WalkDistance = 0;
			agent.stoppingDistance = stoppingDistance;
			currentPathIndex++;
			agent.SetDestination(PathPoints[currentPathIndex]);
			if (agent.isStopped)
			{
				agent.isStopped = false;
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

		public void CheckDistance()
		{
			WalkDistance += Vector3.Distance(transform.position, previousPosition);
			if (OnCheckDistance != null)
			{
				OnCheckDistance();
			}
			previousPosition = transform.position;
		}

		public void CheckWalkEnd()
		{
			if (TargetPoint == Vector3_.zero) { return; }
			if (PathStatus != 2) { return; }

			if (PathPoints.Count != 0 && currentPathIndex != PathPoints.Count - 1)
			{
				float d1 = Distance(transform.position, PathPoints[currentPathIndex]);
				if (d1 <= agent.stoppingDistance)
				{
					currentPathIndex++;
					agent.SetDestination(PathPoints[currentPathIndex]);
				}
			}
			else
			{
				float d = Distance(transform.position, TargetPoint.To());
				if (d <= agent.stoppingDistance)
				{
					agent.isStopped = true;
					TargetPoint = Vector3_.zero;
					WalkDistance = 0;

					currentPathIndex = 0;
					PathPoints.Clear();
					PathStatus = 0;

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

		public List<Vector3> PathPoints;
		private int currentPathIndex;

		public void SetPath(NavMeshPath2 argPath)
		{
			if (argPath.status == NavMeshPathStatus.PathComplete)
			{
				for (int i = 0; i < argPath.corners.Length; i++)
				{
					PathPoints.Add(argPath.corners[i]);
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
							PathRender.SetPosition(i, PathPoints[i] + Vector3.up * PathHeightOffset);
						}
					}
				}
				yield return Wait;
			}
		}


	}


}
