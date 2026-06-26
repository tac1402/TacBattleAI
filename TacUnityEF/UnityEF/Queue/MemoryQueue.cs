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
	/// Очередь в памяти с возможностью удаления по индексу
	/// </summary>
	internal class MemoryQueue<T> : IQueue<T> where T : class, IItemDb, IId
	{
		private Queue<T> queue = new Queue<T>();

		private HashSet<int> removedIds = new HashSet<int>();

		public int Count => queue.Count - removedIds.Count;


		public T Dequeue()
		{
			T ret = default;
			while (queue.Count > 0)
			{
				ret = queue.Dequeue();
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
			T[] items = queue.ToArray();
			foreach (T item in items)
			{
				if (item.Id == id)
				{
					ret = item;
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
					yield return item;
				}
			}
		}

		public void ClearRemoved()
		{
			Queue<T> newQueue = new Queue<T>();
			foreach (T item in queue)
			{
				if (removedIds.Contains(item.Id) == false)
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
			return queue.ToList();
		}

		public void Enqueue(T item) => queue.Enqueue(item);
		public T Peek() => queue.Peek();
	}

}
