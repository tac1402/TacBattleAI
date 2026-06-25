// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev


using DnaCore;
using System;
using System.Collections.Generic;

namespace UnityEF
{
	internal interface IDictionary<K, V> : IEnumerable<KeyValuePair<K, V>> where V : class, IItemDb
	{
		V this[K key] { get; set; }
		bool TryGetValue(K key, out V value);
		void Add(K key, V item);
		bool Remove(K key);
		List<V> GetAll();
		int Count { get; }
		IEnumerable<K> Keys { get; }

		IEnumerable<KeyValuePair<K, V>> Where(Func<KeyValuePair<K, V>, bool> predicate);

		#region default
		System.Collections.Generic.IList<V> Values => GetAll();
		//IEnumerator<V> GetEnumerator() => GetAll().GetEnumerator();
		bool ContainsKey(K key) => TryGetValue(key, out _);
		#endregion
	}
}
