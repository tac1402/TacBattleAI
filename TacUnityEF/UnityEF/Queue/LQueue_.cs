// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Tac;

namespace UnityEF
{
	/// <summary>
	/// Локальная очередь
	/// </summary>
	public class LQueue_<T> : ItemDb, ICollection
	{
		private readonly IQueue_<T> storage;

		public LQueue_()
		{
			storage = CreateStorage();
		}

		public LQueue_(IEnumerable<T> collection)
		{
			if (collection == null) 
				throw new ArgumentNullException(nameof(collection));

			Items = collection.Select(item => new LItem<T>(item)).ToList();

			storage = CreateStorage();
		}

		private IQueue_<T> CreateStorage()
		{
			if (db == null)
			{
				return new MemoryQueue_<T>();
			}
			else
			{
				return new DbLQueue_<T>(Items);
			}
		}

		public List<LItem<T>> Items { get; set; } = new List<LItem<T>>();

		public void Enqueue(T item) => storage.Enqueue(item);
		public T Peek() => storage.Peek();
		public T Dequeue() => storage.Dequeue();
		public void Clear() => storage.Clear();
		public int Count => storage.Count;
		public List<T> ToList() => storage.ToList();
		public IEnumerator<T> GetEnumerator() => storage.GetEnumerator();
	}

	internal class DbLQueue_<T> : IQueue_<T>
	{
		private List<LItem<T>> items;

		public DbLQueue_(List<LItem<T>> argItems)
		{
			items = argItems;
		}

		public int Count => items.Count;

		public void Enqueue(T item)
		{
			items.Add(new LItem<T>(item));
		}

		// Просмотр первого активного элемента без удаления
		public T Peek()
		{
			return items.Count > 0 ? items[0].Item : default(T);
		}

		public T Dequeue()
		{
			if (items.Count == 0)
				return default(T);

			var first = items[0];
			items.RemoveAt(0);
			return first.Item;
		}

		public void Clear()
		{
			items.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in items)
			{
				yield return item.Item;
			}
		}

		public List<T> ToList()
		{
			return items.Select(i => i.Item).ToList();
		}

	}
}
