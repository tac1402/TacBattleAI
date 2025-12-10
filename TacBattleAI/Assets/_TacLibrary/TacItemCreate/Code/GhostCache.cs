using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tac.ItemCreate
{
	[Component(typeof(GhostCache), typeof(ItemCreate))]
	public class GhostCache : MonoBehaviour
	{
		public ItemCreate ItemCreate;

		public GameObject GhostCurrent;
		public List<Item> Cache = new List<Item>();

		private bool CreateMode = false;

		private void Add(string argModelName, ModelTypes argModelType)
		{
			ItemCreate.PredeffinedObjectId = -1;
			GameObject tmpGhostObj = ItemCreate.CreateObject(argModelName, 0, 0, 0, argModelType);
			ItemCreate.PredeffinedObjectId = 0;

			if (tmpGhostObj != null)
			{
				tmpGhostObj.transform.SetParent(this.gameObject.transform);
				Item tmpItem = tmpGhostObj.GetComponent<Item>();

				Cache.Add(tmpItem);
			}
		}

		/// <summary>
		/// Взять из кэша
		/// </summary>
		public Item TakeFromCache(string argModelName, ModelTypes argModelType, Vector3 argPosition)
		{
			Item ret = null;
			int index = -1;
			if (CreateMode == false)
			{
				for (int i = 0; i < Cache.Count; i++)
				{
					if (Cache[i].ModelName == argModelName && Cache[i].ModelType == argModelType)
					{
						index = i;
						break;
					}
				}
			}

			if (index == -1 || Cache[index] == null)
			{
				Add(argModelName, argModelType);
				index = Cache.Count - 1;
			}

			ret = Cache[index];
			ret.transform.SetParent(GhostCurrent.transform);
			ret.transform.position = argPosition;

			Cache.RemoveAt(index);

			return ret;
		}

		/// <summary>
		/// Вернуть в кэш
		/// </summary>
		public void ReturnToCache(Item argItem)
		{
			if (CreateMode == false)
			{
				argItem.transform.SetParent(this.gameObject.transform);
				Cache.Add(argItem);
				ItemCreate.StartDelayDelete();
			}
			else
			{
				ItemCreate.DestroyObject(argItem.gameObject);
			}
		}


	}
}