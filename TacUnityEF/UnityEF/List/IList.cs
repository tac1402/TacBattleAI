// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev


using DnaCore;
using System.Collections.Generic;

namespace UnityEF
{
	internal interface IList<T>
	{
		T this[int key] { get; set; }
		void Add(T item);
		void RemoveAt(int index);
		void Clear();
		int Count { get; }
		public IEnumerator<T> GetEnumerator();
	}
}
