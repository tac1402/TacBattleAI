// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEF
{
	/// <summary>
	/// Список в памяти
	/// </summary>
	internal class MemoryList_<T> : IList<T>
	{
		private readonly List<T> list = new List<T>();

		public T this[int key]
		{
			get => list[key];
			set => list[key] = value;
		}
		public void Add(T item) => list.Add(item);
		public void RemoveAt(int index) => list.RemoveAt(index);
		public void Clear() => list.Clear();
		public bool Contains(T item) => list.Contains(item);
		public int Count => list.Count;
		public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Find(Predicate<T> match) => list.Find(match);
		public List<T> FindAll(Predicate<T> match) => list.FindAll(match);
	}
}
