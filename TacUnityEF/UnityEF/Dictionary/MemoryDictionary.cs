// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System.Collections.Generic;
using System.Linq;

namespace UnityEF
{
	internal class MemoryDictionary<K, V> : IDictionary<K, V> where V : class, IItemDb
	{
		private readonly Dictionary<K, V> dictionary = new Dictionary<K, V>();

		public V this[K key]
		{
			get => dictionary[key];
			set => dictionary[key] = value;
		}

		public void Add(K key, V item)
		{
			dictionary[key] = item;
		}

		public bool TryGetValue(K key, out V value) => dictionary.TryGetValue(key, out value);
		public bool Remove(K key) => dictionary.Remove(key);
		public List<V> GetAll() => dictionary.Values.ToList();
		public IEnumerable<K> Keys => dictionary.Keys;
		public int Count => dictionary.Count;

	}
}
