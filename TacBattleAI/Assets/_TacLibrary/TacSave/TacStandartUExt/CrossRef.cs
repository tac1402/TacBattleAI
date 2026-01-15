// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev


using System.Collections.Generic;
using UnityEngine;

namespace Tac
{
	/// <summary>
	/// Перекрестные ссылки
	/// </summary>
	public class CrossRef<T> where T : Item
	{
		/// <summary>
		/// Индексы ссылок
		/// </summary>
		public Dictionary<string, int> Ref;

		public CrossRef() { }

		public CrossRef(Dictionary<string, T> argDictionary) 
		{
			Ref = GetRef(argDictionary);
		}

		public void ClearRef() { Ref.Clear(); Ref = null; }

		/// <summary>
		/// Получить из объектов уникальные индексы, используя Item.Id
		/// </summary>
		public Dictionary<string, int> GetRef(Dictionary<string, T> argDictionary)
		{
			if (Ref == null)
			{
				Ref = new Dictionary<string, int>();
				foreach (var item in argDictionary)
				{
					if (item.Value != null)
					{
						Ref.Add(item.Key, item.Value.Id);
					}
				}
			}
			return Ref;
		}

		/// <summary>
		/// Разрешить ссылки (резолвинг) - по индексам восстановить объекты (ссылки на них)
		/// </summary>
		public Dictionary<string, T> Resolve(Dictionary<int, GameObject> allObject)
		{
			Dictionary<string, T> ret = new Dictionary<string, T>();
			foreach (var item in Ref)
			{
				if (allObject.ContainsKey(item.Value))
				{
					ret.Add(item.Key, allObject[item.Value].GetComponent<T>());
				}
			}
			return ret;
		}

	}


}