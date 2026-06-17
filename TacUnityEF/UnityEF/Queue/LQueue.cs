// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

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
		private Queue<LItem<T>> queue
		{
			get => new Queue<LItem<T>>(items.OrderBy(i => i.Id));
			set
			{
				items = value.ToList();
			}
		}

		public DbLQueue(List<LItem<T>> argItems)
		{
			items = argItems;
		}

		private HashSet<int> removedIds = new HashSet<int>();

		public int Count => items.Count - removedIds.Count;

		public void Enqueue(T item) => queue.Enqueue(new LItem<T>(item));
		public T Peek() => queue.Peek().Item;


		public T Dequeue()
		{
			T ret = default;
			while (items.Count > 0)
			{
				ret = queue.Dequeue().Item;
				if (removedIds.Contains(ret.Id))
				{
					removedIds.Remove(ret.Id);
					continue;
				}
				break;
			}
			return ret;
		}

		public T Remove(int id)
		{
			removedIds.Add(id);

			T ret = default;
			LItem<T>[] items = queue.ToArray();
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
			queue.Clear();
			removedIds.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in queue)
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
			foreach (LItem<T> item in queue)
			{
				if (removedIds.Contains(item.Item.Id) == false)
				{
					newQueue.Enqueue(item);
				}
			}
			queue = newQueue;
			removedIds.Clear();
		}

		public List<T> ToList()
		{
			ClearRemoved();
			return queue.Select(i => i.Item).ToList();
		}

	}
}
