// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tac.Camera
{
	public partial class TopCamera
	{
		public LayerMask TerrainLayer;
		public LayerMask BuildingsLayer;
		public EventSystem EventSystem;

		public float MaxHeight = 1000;


		public bool IsUsingUI()
		{
			return (EventSystem.IsPointerOverGameObject() || EventSystem.IsPointerOverGameObject(0));
		}

		public GameObject GetBuilding(Vector2 touch)
		{
			Ray ray = camera.ScreenPointToRay(touch);
			return GetRaycast(ray, BuildingsLayer).Item2;
		}

		public (Vector3, GameObject) GetTerrain(Vector2 touch)
		{
			Ray ray = camera.ScreenPointToRay(touch);
			return GetRaycast(ray, TerrainLayer);
		}

		public (Vector3, GameObject) GetAllowBuildPoint(Vector2 touch, List<Bounds> argBounds)
		{
			Ray ray = camera.ScreenPointToRay(touch);
			(Vector3 retPoint, GameObject retObject) = GetRaycast(ray, TerrainLayer);

			if (retObject != null && argBounds != null)
			{
				for (int i = 0; i < argBounds.Count; i++)
				{
					if (argBounds[i].IsEmpty() == false)
					{
						Vector3 locDirection = new Vector3(0, -1, 0);
						debugPoint = retPoint;
						Bounds bounds = new Bounds(retPoint, argBounds[i].size);
						BoundsExt boundsExt = new BoundsExt(bounds, ray.origin.y);
						debugRays = new List<Ray>();
						foreach (Vector3 point in boundsExt.AllBoundsPoint)
						{
							Ray ray2 = new Ray(point, locDirection);
							debugRays.Add(ray2);

							RaycastHit hit;
							Building building = null;
							if (Physics.Raycast(ray2, out hit, MaxHeight, BuildingsLayer.value))
							{
								building = hit.collider.gameObject.GetComponent<Building>();
							}

							if (building == null || building.AllowBuild == false)
							{
								retObject = null;
								break;
							}
						}
					}
					if (retObject == null) { break; }
				}

			}
			return (retPoint, retObject);
		}


		private (Vector3, GameObject) GetRaycast(Ray argRay, LayerMask target)
		{
			RaycastHit hit;
			Vector3 retPoint = Vector3.zero;
			GameObject retObject = null;

			if (Physics.Raycast(argRay, out hit, MaxHeight, target.value))
			{
				retPoint = hit.point;
				retObject = hit.collider.gameObject;
			}

			return (retPoint, retObject);
		}

		List<Ray> debugRays;
		Vector3 debugPoint;

#if UNITY_EDITOR
		void OnDrawGizmos2()
		{
			Gizmos.color = Color.green;
			if (debugRays != null)
			{
				for (int i = 0; i < debugRays.Count; i++)
				{
					Gizmos.DrawLine(debugRays[i].origin, debugRays[i].origin + debugRays[i].direction * 100);
				}
			}
			Gizmos.DrawSphere(debugPoint, 0.1f);
		}
#endif

	}
}