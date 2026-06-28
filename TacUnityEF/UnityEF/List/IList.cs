// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev


using DnaCore;
using System;
using System.Collections.Generic;

namespace UnityEF
{
	internal interface IList<T> : IEnumerable<T>
	{
		T this[int key] { get; set; }
		void Add(T item);
		void RemoveAt(int index);
		void Clear();
		bool Contains(T item);
		int Count { get; }

		T Find(Predicate<T> match);
		List<T> FindAll(Predicate<T> match);
	}
}
