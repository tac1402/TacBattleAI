// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEF
{
	internal interface IQueue<T> where T : class, IItemDb
	{
		void Enqueue(T item);
		T Peek();
		T Dequeue();
		T Remove(int id);
		void Clear();
		int Count { get; }
		List<T> ToList();
		IEnumerator<T> GetEnumerator();
	}
}
