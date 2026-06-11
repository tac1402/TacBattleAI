// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
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
		public List<LItem<T>> Items { get; set; } = new List<LItem<T>>();

		// Для удобства — очередь (не сохраняется в БД)
		[NotMapped]
		public Queue<LItem<T>> Queue
		{
			get => new Queue<LItem<T>>(Items.OrderBy(i => i.Id));
			set
			{
				Items = value.ToList();
			}
		}

		private HashSet<int> removedIds = new HashSet<int>();

		public int Count => Queue.Count - removedIds.Count;

		public void Enqueue(T item) => Queue.Enqueue(new LItem<T>(item));
		public T Peek() => Queue.Peek().Item;


		public T Dequeue()
		{
			T ret = default;
			while (Queue.Count > 0)
			{
				ret = Queue.Dequeue().Item;
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
			LItem<T>[] items = Queue.ToArray();
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
			Queue.Clear();
			removedIds.Clear();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in Queue)
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
			foreach (LItem<T> item in Queue)
			{
				if (removedIds.Contains(item.Item.Id) == false)
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
			return Queue.Select(i => i.Item).ToList();
		}
	}

}
