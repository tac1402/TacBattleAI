using System;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.LightTransport;

public class NavMeshPathExt : IDisposable
{
	private int areaMask;
	private int agentTypeId;
	private NavMeshWorld world;
	private NavMeshQuery query;

	/// <summary>
	/// Создаёт экземпляр с настройками по умолчанию.
	/// </summary>
	/// <param name="argAreaMask">Битовая маска разрешённых областей (по умолчанию все).</param>
	/// <param name="nodePoolSize">Размер пула узлов для самых длинных путей.</param>
	public NavMeshPathExt(int argAreaMask = NavMesh.AllAreas, int argAgentTypeId = 0, int nodePoolSize = 65535)
	{
		areaMask = argAreaMask;
		agentTypeId = argAgentTypeId;
		world = NavMeshWorld.GetDefaultWorld();
		query = new NavMeshQuery(world, Allocator.Persistent, nodePoolSize);
	}

	public void Dispose()
	{
		query.Dispose();
	}
	public NavMeshPath2 CalculatePath(Vector3 sourcePosition, Vector3 targetPosition)
	{
		NavMeshPath2 path = new NavMeshPath2();

		// Используем радиус для поиска ближайшей точки на NavMesh
		var startLocation = query.MapLocation(sourcePosition, Vector3.one, agentTypeId);
		var targetLocation = query.MapLocation(targetPosition, Vector3.one, agentTypeId);

		if (!query.IsValid(startLocation) || !query.IsValid(targetLocation))
		{
			path.status = NavMeshPathStatus.PathInvalid;
			return path;
		}

		// Создаём массив стоимостей для фильтрации по areaMask
		var costs = new NativeArray<float>(32, Allocator.Temp);
		for (int i = 0; i < 32; i++)
		{
			// Базовая стоимость для разрешённых областей
			costs[i] = 1f;

			// Особые случаи
			if (i == 2) costs[i] = 2f;   // третья область (индекс 2)
			if (i == 3) costs[i] = 10f;  // четвёртая область (индекс 3)
		}

		// Запускаем поиск
		var status = query.BeginFindPath(startLocation, targetLocation, areaMask, costs);
		int maxIter = 10000; // защита от бесконечного цикла
		int usedIter = 0;
		while (status == PathQueryStatus.InProgress && maxIter-- > 0)
		{
			status = query.UpdateFindPath(100, out usedIter);
		}

		if (status == PathQueryStatus.Success)
		{
			query.EndFindPath(out int polyCount);

			if (polyCount == 0)
			{
				path.status = NavMeshPathStatus.PathInvalid;
				return path;
			}

			// Получаем список полигонов
			var polygons = new NativeArray<PolygonId>(polyCount, Allocator.Temp);
			query.GetPathResult(polygons);

			// Строим сглаженный путь через вспомогательный метод
			const int maxStraightPath = 256;
			var straightPath = new NativeArray<NavMeshLocation>(maxStraightPath, Allocator.Temp);
			var straightPathFlags = new NativeArray<StraightPathFlags>(maxStraightPath, Allocator.Temp);
			var straightPathPolys = new NativeArray<float>(maxStraightPath, Allocator.Temp);
			int straightPathCount = 0;

			var findStatus = FindStraightPath(
				query,
				startLocation.position,
				targetLocation.position,
				polygons,
				polyCount,
				ref straightPath,
				ref straightPathFlags,
				ref straightPathPolys,
				ref straightPathCount,
				maxStraightPath
			);

			if (findStatus == PathQueryStatus.Success && straightPathCount > 0)
			{
				var corners = new Vector3[straightPathCount];
				for (int i = 0; i < straightPathCount; i++)
				{
					corners[i] = straightPath[i].position;
				}
				path.corners = corners;
				path.status = NavMeshPathStatus.PathComplete;
			}
			else
			{
				path.status = NavMeshPathStatus.PathInvalid;
			}

			// Очистка
			polygons.Dispose();
			straightPath.Dispose();
			straightPathFlags.Dispose();
			straightPathPolys.Dispose();
		}

		return path;
	}

	public static PathQueryStatus FindStraightPath(NavMeshQuery query, Vector3 startPos, Vector3 endPos, NativeSlice<PolygonId> path, int pathSize, 
		ref NativeArray<NavMeshLocation> straightPath, ref NativeArray<StraightPathFlags> straightPathFlags, ref NativeArray<float> vertexSide, 
		ref int straightPathCount, int maxStraightPath)
	{
		if (!query.IsValid(path[0]))
		{
			straightPath[0] = new NavMeshLocation(); // empty terminator
			return PathQueryStatus.Failure; // | PathQueryStatus.InvalidParam;
		}

		straightPath[0] = query.CreateLocation(startPos, path[0]);

		straightPathFlags[0] = StraightPathFlags.Start;

		var apexIndex = 0;
		var n = 1;

		if (pathSize > 1)
		{
			var startPolyWorldToLocal = query.PolygonWorldToLocalMatrix(path[0]);

			var apex = startPolyWorldToLocal.MultiplyPoint(startPos);
			var left = new Vector3(0, 0, 0); // Vector3.zero accesses a static readonly which does not work in burst yet
			var right = new Vector3(0, 0, 0);
			var leftIndex = -1;
			var rightIndex = -1;

			for (var i = 1; i <= pathSize; ++i)
			{
				var polyWorldToLocal = query.PolygonWorldToLocalMatrix(path[apexIndex]);

				Vector3 vl, vr;
				if (i == pathSize)
				{
					vl = vr = polyWorldToLocal.MultiplyPoint(endPos);
				}
				else
				{
					var success = query.GetPortalPoints(path[i - 1], path[i], out vl, out vr);
					if (!success)
					{
						return PathQueryStatus.Failure; // | PathQueryStatus.InvalidParam;
					}

					vl = polyWorldToLocal.MultiplyPoint(vl);
					vr = polyWorldToLocal.MultiplyPoint(vr);
				}

				vl = vl - apex;
				vr = vr - apex;

				// Ensure left/right ordering
				if (Perp2D(vl, vr) < 0)
					Swap(ref vl, ref vr);

				// Terminate funnel by turning
				if (Perp2D(left, vr) < 0)
				{
					var polyLocalToWorld = query.PolygonLocalToWorldMatrix(path[apexIndex]);
					var termPos = polyLocalToWorld.MultiplyPoint(apex + left);

					n = RetracePortals(query, apexIndex, leftIndex, path, n, termPos, ref straightPath, ref straightPathFlags, maxStraightPath);
					if (vertexSide.Length > 0)
					{
						vertexSide[n - 1] = -1;
					}

					//Debug.Log("LEFT");

					if (n == maxStraightPath)
					{
						straightPathCount = n;
						return PathQueryStatus.Success; // | PathQueryStatus.BufferTooSmall;
					}

					apex = polyWorldToLocal.MultiplyPoint(termPos);
					left.Set(0, 0, 0);
					right.Set(0, 0, 0);
					i = apexIndex = leftIndex;
					continue;
				}
				if (Perp2D(right, vl) > 0)
				{
					var polyLocalToWorld = query.PolygonLocalToWorldMatrix(path[apexIndex]);
					var termPos = polyLocalToWorld.MultiplyPoint(apex + right);

					n = RetracePortals(query, apexIndex, rightIndex, path, n, termPos, ref straightPath, ref straightPathFlags, maxStraightPath);
					if (vertexSide.Length > 0)
					{
						vertexSide[n - 1] = 1;
					}

					//Debug.Log("RIGHT");

					if (n == maxStraightPath)
					{
						straightPathCount = n;
						return PathQueryStatus.Success; // | PathQueryStatus.BufferTooSmall;
					}

					apex = polyWorldToLocal.MultiplyPoint(termPos);
					left.Set(0, 0, 0);
					right.Set(0, 0, 0);
					i = apexIndex = rightIndex;
					continue;
				}

				// Narrow funnel
				if (Perp2D(left, vl) >= 0)
				{
					left = vl;
					leftIndex = i;
				}
				if (Perp2D(right, vr) <= 0)
				{
					right = vr;
					rightIndex = i;
				}
			}
		}

		// Remove the the next to last if duplicate point - e.g. start and end positions are the same
		// (in which case we have get a single point)
		if (n > 0 && (straightPath[n - 1].position == endPos))
			n--;

		n = RetracePortals(query, apexIndex, pathSize - 1, path, n, endPos, ref straightPath, ref straightPathFlags, maxStraightPath);
		if (vertexSide.Length > 0)
		{
			vertexSide[n - 1] = 0;
		}

		if (n == maxStraightPath)
		{
			straightPathCount = n;
			return PathQueryStatus.Success; // | PathQueryStatus.BufferTooSmall;
		}

		// Fix flag for final path point
		straightPathFlags[n - 1] = StraightPathFlags.End;

		straightPathCount = n;
		return PathQueryStatus.Success;
	}

	public static float Perp2D(Vector3 u, Vector3 v)
	{
		return u.z * v.x - u.x * v.z;
	}

	public static void Swap(ref Vector3 a, ref Vector3 b)
	{
		var temp = a;
		a = b;
		b = temp;
	}

	// Retrace portals between corners and register if type of polygon changes
	public static int RetracePortals(NavMeshQuery query, int startIndex, int endIndex, NativeSlice<PolygonId> path, int n, Vector3 termPos, ref NativeArray<NavMeshLocation> straightPath, ref NativeArray<StraightPathFlags> straightPathFlags, int maxStraightPath)
	{
		for (var k = startIndex; k < endIndex - 1; ++k)
		{
			var type1 = query.GetPolygonType(path[k]);
			var type2 = query.GetPolygonType(path[k + 1]);
			if (type1 != type2)
			{
				Vector3 l, r;
				var status = query.GetPortalPoints(path[k], path[k + 1], out l, out r);
				Vector3 cpa1, cpa2;
				SegmentSegmentCPA(out cpa1, out cpa2, l, r, straightPath[n - 1].position, termPos);
				straightPath[n] = query.CreateLocation(cpa1, path[k + 1]);

				straightPathFlags[n] = (type2 == NavMeshPolyTypes.OffMeshConnection) ? StraightPathFlags.OffMeshConnection : 0;
				if (++n == maxStraightPath)
				{
					return maxStraightPath;
				}
			}
		}
		straightPath[n] = query.CreateLocation(termPos, path[endIndex]);
		straightPathFlags[n] = query.GetPolygonType(path[endIndex]) == NavMeshPolyTypes.OffMeshConnection ? StraightPathFlags.OffMeshConnection : 0;
		return ++n;
	}

	// Calculate the closest point of approach for line-segment vs line-segment.
	public static bool SegmentSegmentCPA(out Vector3 c0, out Vector3 c1, Vector3 p0, Vector3 p1, Vector3 q0, Vector3 q1)
	{
		var u = p1 - p0;
		var v = q1 - q0;
		var w0 = p0 - q0;

		float a = Vector3.Dot(u, u);
		float b = Vector3.Dot(u, v);
		float c = Vector3.Dot(v, v);
		float d = Vector3.Dot(u, w0);
		float e = Vector3.Dot(v, w0);

		float den = (a * c - b * b);
		float sc, tc;

		if (den == 0)
		{
			sc = 0;
			tc = d / b;

			// todo: handle b = 0 (=> a and/or c is 0)
		}
		else
		{
			sc = (b * e - c * d) / (a * c - b * b);
			tc = (a * e - b * d) / (a * c - b * b);
		}

		c0 = Vector3.Lerp(p0, p1, sc);
		c1 = Vector3.Lerp(q0, q1, tc);

		return den != 0;
	}

	[Flags]
	public enum StraightPathFlags
	{
		Start = 0x01, // The vertex is the start position.
		End = 0x02, // The vertex is the end position.
		OffMeshConnection = 0x04 // The vertex is start of an off-mesh link.
	}
}

public class NavMeshPath2
{
	public Vector3[] corners;
	public NavMeshPathStatus status;
}