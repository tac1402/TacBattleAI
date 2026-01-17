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
		public StatusBar StatusBar;

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
		/// ƒвигаетс€ ли юнит к цели
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

		public int PathStatus = 0; // 0 - нет пути, 1 - нужно посчитать, 2 - путь расчитан, 3 - частично расчитан

		/// <summary>
		/// ¬озникает, когда агент заканчивает движение к заданной цели
		/// </summary>
		public event Send OnWalkEnd;

		/// <summary>
		/// ÷ель движени€ агента, если он движетс€
		/// </summary>
		public Vector3_ TargetPoint = Vector3_.zero;

		/// <summary>
		///  онтроль дистанции, можно использовать только внутри класса, в т.ч. partial
		/// </summary>
		private event Send OnCheckDistance;
		private Vector3 previousPosition;

		private System.Random rnd = new System.Random();


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

			agent = GetComponent<NavMeshAgent>();
			if (agent != null)
			{
				agent.enabled = true;
				StartCoroutine(DrawPath());
			}
			StartCoroutine(Tick());
		}


		/// <summary>
		/// ƒвигатьс€ к 
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
			agent.SetPath(PathFull[0]);
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

			float d = Vector3.Distance(transform.position, TargetPoint.To());
			if (d <= agent.stoppingDistance)
			{
				agent.isStopped = true;
				TargetPoint = Vector3_.zero;
				WalkDistance = 0;
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
		/// Ќайти ближайшую доступную позицию на NavMesh карте и поместить в неЄ агента
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
		public List<NavMeshPath> PathFull;

		public void CalculatePath()
		{

			if (PathStatus == 1 || PathStatus == 3)
			{
				Vector3 from = transform.position;
				Vector3 to = TargetPoint.To();
				if (PathStatus == 1)
				{
					PathPoints = new List<Vector3>();
					PathFull = new List<NavMeshPath>();
				}
				if (PathStatus == 3)
				{
					NavMeshPath lastPath = PathFull[PathFull.Count - 1];
					int lastIndex = lastPath.corners.Length - 1;
					from = lastPath.corners[lastIndex];
				}

				NavMeshPath tmpPath = new NavMeshPath();
				bool isPath = NavMesh.CalculatePath(from, to, NavMesh.AllAreas, tmpPath);

				if (isPath)
				{
					if (tmpPath.status == NavMeshPathStatus.PathComplete)
					{
						PathStatus = 2;
					}
					else if (tmpPath.status == NavMeshPathStatus.PathPartial)
					{
						PathStatus = 3;
					}

					PathFull.Add(tmpPath);
					for (int i = 0; i < tmpPath.corners.Length; i++)
					{
						PathPoints.Add(tmpPath.corners[i]);
					}
				}
			}
		}



		public LineRenderer PathRender;
		private float PathHeightOffset = 0.25f;

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
