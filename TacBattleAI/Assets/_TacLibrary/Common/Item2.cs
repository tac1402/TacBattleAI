using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tac
{
	public partial class Item2 : Item
	{
		/// <summary>
		/// Идентификатор, еще не построенного/ не существующего объекта в мире
		/// </summary>
		public int GhostId;

		public void SetPosition(Vector3 position)
		{
			transform.localPosition = position;
		}

		public Vector3 GetPosition()
		{
			return transform.localPosition;
		}

		public static Item2 GetItem(GameObject go)
		{
			Item2 retItem = null;
			if (go != null)
			{
				retItem = go.GetComponent<Item2>();
				if (retItem == null)
				{
					BuildPart part = go.GetComponent<BuildPart>();
					if (part != null)
					{
						retItem = part.Main;
					}
				}
			}
			return retItem;
		}

		public static GameObject GetMain(GameObject go)
		{
			GameObject retItem = null;
			if (go != null)
			{
				BuildPart part = go.GetComponent<BuildPart>();
				if (part != null)
				{
					retItem = part.MainObj;
				}
			}
			return retItem;
		}
	}
}