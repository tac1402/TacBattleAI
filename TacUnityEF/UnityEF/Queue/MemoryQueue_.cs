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
	internal class MemoryQueue_<T> : IQueue_<T>
	{
		private Queue<T> queue = new Queue<T>();

		public int Count => queue.Count;

		public T Dequeue() => queue.Dequeue();


		public void Clear()
		{
			queue.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in queue)
			{
				yield return item;
			}
		}

		public List<T> ToList()
		{
			return queue.ToList();
		}

		public void Enqueue(T item) => queue.Enqueue(item);
		public T Peek() => queue.Peek();
	}
}
