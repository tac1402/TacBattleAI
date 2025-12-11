// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

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

	}
}