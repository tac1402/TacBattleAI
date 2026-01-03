// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using UnityEngine;
using UnityEngine.AI;


namespace Tac.Agent
{
	public partial class Agent
	{
		private bool hasTeleported = false;

		private PathDivider divider;
		private Vector3 agentStartPosition;

		public void WalkTeleport(Vector3_ argTarget)
		{ 
			divider = new PathDivider();
			divider.DividePath(agent, transform.position, argTarget.To(), 60f);

			Gradient gradient = divider.CreatePhaseGradient(new Color[] { Color.green, Color.red, Color.green });
			Path.colorGradient = gradient;
			agentStartPosition = agent.transform.position;

			Walk(argTarget);
			hasTeleported = false;

			OnCheckDistance += Teleport_OnCheckDistance;
		}

		/// <summary>
		/// В нужное время телепортируется в точку из которой сможет прейти в назначенное время
		/// </summary>
		private void Teleport_OnCheckDistance(params object[] argInfo)
		{
			Vector3 agentWorldOffset = transform.parent.position - agentStartPosition;
			Path.gameObject.transform.localPosition = -agentWorldOffset;

			// Начал движение, но еще не телепортировался
			if (agent.hasPath == false || hasTeleported == true) return;

			if (WalkDistance >= divider.firstSegmentLength)
			{
				ExecuteTeleport();
			}
		}

		void ExecuteTeleport()
		{
			Vector3 savedDestination = agent.destination;

			// Останавливаем агента
			agent.isStopped = true;

			agent.nextPosition = transform.position;

			// Телепортируем
			agent.Warp(divider.teleportPoint);

			// Продолжаем движение к той же цели
			agent.isStopped = false;

			agent.SetDestination(savedDestination);

			hasTeleported = true;
		}

	}

	public class PathDivider
	{
		public float safetyMargin = 0.1f; // 10% погрешность

		private NavMeshPath path;

		public float totalPathLength;
		public float firstSegmentLength;
		public float lastSegmentLength;
		public Vector3 teleportPoint;

		public void DividePath(NavMeshAgent agent, Vector3 startPos, Vector3 endPos, float totalGameTime)
		{
			path = new NavMeshPath();
			bool isPath = NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);

			if (isPath)
			{
				totalPathLength = CalculatePathLength(path.corners); // Вычисляем общую длину пути

				float availableRealTime = totalGameTime / 2f; // Половина времени на каждый отрезок
				float safeTime = availableRealTime * (1f - safetyMargin); // Минус 10%

				// Вычисляем максимальную длину для первого и последнего отрезков
				float maxWalkLength = agent.speed * safeTime;

				// Проверяем, достаточно ли длины пути
				if (totalPathLength <= maxWalkLength * 2f)
				{
					// Если путь короткий, делим его пополам
					firstSegmentLength = totalPathLength / 2f;
					lastSegmentLength = totalPathLength / 2f;
				}
				else
				{
					// Если путь длинный, используем максимальную длину
					firstSegmentLength = maxWalkLength;
					lastSegmentLength = maxWalkLength;
				}


				teleportPoint = GetPointAtDistance(path.corners, totalPathLength - lastSegmentLength);
			}


		}

		private float CalculatePathLength(Vector3[] corners)
		{
			float length = 0f;
			for (int i = 0; i < corners.Length - 1; i++)
			{
				length += Vector3.Distance(corners[i], corners[i + 1]);
			}
			return length;
		}

		private Vector3 GetPointAtDistance(Vector3[] corners, float targetDistance)
		{
			float accumulated = 0f;

			for (int i = 0; i < corners.Length - 1; i++)
			{
				Vector3 start = corners[i];
				Vector3 end = corners[i + 1];
				float segmentDist = Vector3.Distance(start, end);

				if (accumulated + segmentDist >= targetDistance)
				{
					float t = (targetDistance - accumulated) / segmentDist;
					return Vector3.Lerp(start, end, t);
				}

				accumulated += segmentDist;
			}

			// Если targetDistance больше длины пути, возвращаем последнюю точку
			return corners[corners.Length - 1];
		}

		public Gradient CreatePhaseGradient(Color[] phaseColors)
		{
			float middleSegmentLength = totalPathLength - firstSegmentLength - lastSegmentLength;
			float[] phaseLengths = new float[] { firstSegmentLength, middleSegmentLength, lastSegmentLength };

			Gradient gradient = new Gradient();
			GradientColorKey[] colorKeys = new GradientColorKey[phaseLengths.Length * 2];
			GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

			float accumulated = 0f;
			int keyIndex = 0;

			for (int i = 0; i < phaseLengths.Length; i++)
			{
				float phaseStart = accumulated / totalPathLength;
				accumulated += phaseLengths[i];
				float phaseEnd = accumulated / totalPathLength;

				// Начало фазы
				colorKeys[keyIndex++] = new GradientColorKey(phaseColors[i], phaseStart);
				// Конец фазы (если не последняя)
				if (i < phaseLengths.Length - 1)
				{
					colorKeys[keyIndex++] = new GradientColorKey(phaseColors[i], phaseEnd - 0.001f);
				}
				else
				{
					colorKeys[keyIndex++] = new GradientColorKey(phaseColors[i], phaseEnd);
				}
			}

			alphaKeys[0] = new GradientAlphaKey(1f, 0f);
			alphaKeys[1] = new GradientAlphaKey(1f, 1f);

			gradient.SetKeys(colorKeys, alphaKeys);
			return gradient;
		}

	}
}
