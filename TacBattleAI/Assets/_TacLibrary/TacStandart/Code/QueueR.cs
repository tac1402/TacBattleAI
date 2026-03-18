// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tac
{
    public class QueueR<T> where T : IId
	{
		public Queue<T> Queue = new Queue<T>();
		private HashSet<int> removedIds = new HashSet<int>();

		public int Count => Queue.Count - removedIds.Count;

		public void Enqueue(T item) => Queue.Enqueue(item);
		public T Peek() => Queue.Peek();
		

		public T Dequeue()
		{
			T ret = default;
			while (Queue.Count > 0)
			{
				ret = Queue.Dequeue();
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
			T[] items = Queue.ToArray();
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
			Queue.Clear();
			removedIds.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in Queue)
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
			foreach (T item in Queue)
			{
				if (removedIds.Contains(item.Id) == false)
				{
					newQueue.Enqueue(item);
				}
			}
			Queue = newQueue;
			removedIds.Clear();
		}

		public List<T> ToList()
		{ 
			ClearRemoved();
			return Queue.ToList();
		}

	}
}
