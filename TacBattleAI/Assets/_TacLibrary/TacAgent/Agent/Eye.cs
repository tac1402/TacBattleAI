// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2018-24 Sergej Jakovlev
// You can use this code for educational purposes only;
// this code or its modifications cannot be used for commercial purposes
// or in proprietary libraries without permission from the author

using UnityEngine;

namespace Tac.Agent
{
	public class Eye : MonoBehaviour
	{
		/// <summary>
		/// ћаска определ€юща€ опасности 
		/// </summary>
		public LayerMask DangerLayerMask;
		/// <summary>
		/// ћаска определ€юща€ преп€тстви€ на пути
		/// </summary>
		public LayerMask ObstacleLayerMask;

		/// <summary>
		/// ≈сть ли преп€тствие на пути взгл€да
		/// </summary>
		public int PathOfSight(Transform argTarget)
		{
			return PathOfSight(argTarget, new Vector3(0, 1, 0), new Vector3(0, 1, 0));
		}

		/// <summary>
		/// ≈сть ли преп€тствие на пути взгл€да
		/// </summary>
		public int PathOfSight(Transform argTarget, Vector3 argOffset)
		{
			return PathOfSight(argTarget, argOffset, new Vector3(0, 1, 0));
		}


		private Vector3 from;
		private Vector3 to;

		/// <summary>
		/// ≈сть ли преп€тствие на пути взгл€да
		/// </summary>
		public int PathOfSight(Transform argTarget, Vector3 argOffset, Vector3 argTargetOffset)
		{
			RaycastHit hit;
			//from = transform.TransformPoint(argOffset);
			//to = argTarget.TransformPoint(argTargetOffset);
			from = transform.position + argOffset;
			to = argTarget.position + argTargetOffset;

			// ≈сли на пути преп€тствие
			if (Physics.Linecast(from, to, out hit, ObstacleLayerMask))
			{
				return 1; // на пути цели есть преп€тствие
			}
			// ƒолжен пересечьс€ с целью или еЄ частью, иначе есть преп€тствие
			if (Physics.Linecast(from, to, out hit, DangerLayerMask))
			{
				if (ContainsTransform(hit.transform, argTarget))
				{
					return 0; // цель видна
				}
			}
			return -1; // цель не видна
		}

		// true - если target часть parent
		private bool ContainsTransform(Transform target, Transform parent)
		{
			if (target == null)
			{
				return false;
			}
			if (target.Equals(parent))
			{
				return true;
			}
			return ContainsTransform(target.parent, parent);
		}

/*
#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			DrawSightLine();
		}

		public void DrawSightLine()
		{
			Color color = Color.yellow;

			UnityEditor.Handles.color = color;

			UnityEditor.Handles.DrawLine(from, to);

		}
#endif
*/

	}
}
