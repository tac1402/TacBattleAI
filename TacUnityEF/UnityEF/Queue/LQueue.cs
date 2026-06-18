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
	public class LQueue<T> : ItemDb where T : class, IItemDb, IId
	{
		private readonly IQueue<T> storage;

		public LQueue()
		{
			storage = CreateStorage();
		}

		private IQueue<T> CreateStorage()
		{
			if (db == null)
			{
				return new MemoryQueue<T>();
			}
			else
			{
				return new DbLQueue<T>(Items);
			}
		}

		public List<LItem<T>> Items { get; set; } = new List<LItem<T>>();

		public void Enqueue(T item) => storage.Enqueue(item);
		public T Peek() => storage.Peek();
		public T Dequeue() => storage.Dequeue();
		public T Remove(int id) => storage.Remove(id);
		public void Clear() => storage.Clear();
		public int Count => storage.Count;
		public List<T> ToList() => storage.ToList();
		public IEnumerator<T> GetEnumerator() => storage.GetEnumerator();
	}

	internal class DbLQueue<T> : IQueue<T> where T : class, IItemDb, IId
	{
		private List<LItem<T>> items;

		public DbLQueue(List<LItem<T>> argItems)
		{
			items = argItems;
		}

		private HashSet<int> removedIds = new HashSet<int>();

		public int Count => items.Count - removedIds.Count;

		public void Enqueue(T item)
		{
			items.Add(new LItem<T>(item));
		}

		// Просмотр первого активного элемента без удаления
		public T Peek()
		{
			foreach (var litem in items)
			{
				if (!removedIds.Contains(litem.Id))
				{
					return litem.Item;
				}
			}
			return default(T);
		}

		public T Dequeue()
		{
			while (items.Count > 0)
			{
				var first = items[0];
				if (removedIds.Contains(first.Id))
				{
					// Удаляем помеченный элемент из списка и очищаем метку
					removedIds.Remove(first.Id);
					items.RemoveAt(0);
					continue;
				}
				// Найден активный элемент – удаляем его и возвращаем
				items.RemoveAt(0);
				return first.Item;
			}
			return default(T);
		}

		public T Remove(int id)
		{
			removedIds.Add(id);

			T ret = default;
			foreach (LItem<T> item in items)
			{
				if (item.Item.Id == id)
				{
					ret = item.Item;
					break;
				}
			}
			return ret;
		}

		public void Clear()
		{
			items.Clear();
			removedIds.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in items)
			{
				if (removedIds.Contains(item.Id) == false)
				{
					yield return item.Item;
				}
			}
		}

		public void ClearRemoved()
		{
			Queue<LItem<T>> newQueue = new Queue<LItem<T>>();
			foreach (LItem<T> item in items)
			{
				if (removedIds.Contains(item.Item.Id) == false)
				{
					newQueue.Enqueue(item);
				}
			}
			items = newQueue.ToList();
			removedIds.Clear();
		}

		public List<T> ToList()
		{
			ClearRemoved();
			return items.Select(i => i.Item).ToList();
		}

	}
}
