// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace UnityEF
{
	public class LQueue<T> : ItemDb where T : ItemDb
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

		public void Enqueue(T item)
		{
			Queue.Enqueue(new LItem<T>(item));
		}

		public T Dequeue()
		{
			return Queue.Dequeue().Item;
		}
	}

	public class LItem<T> : ItemDb where T : ItemDb
	{
		public T Item;

		public LItem() { }

		public LItem(T argItem)
		{
			Item = argItem;
		}
	}

}
