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
	/// Локальный словарь в БД
	/// </summary>
	public class LDictionary<K, V> : ItemDb where V : class, IItemDb
	{
		private readonly IDictionary<K, V> storage;

		public LDictionary()
		{
			storage = CreateStorage();
		}

		private IDictionary<K, V> CreateStorage()
		{
			if (ItemDb<K>.db == null)
			{
				return new MemoryDictionary<K, V>();
			}
			else
			{
				return new DbLDictionary<K, V>(Items);
			}
		}

		public List<LKeyValue<K, V>> Items { get; set; } = new List<LKeyValue<K, V>>();

		#region default
		public V this[K key]
		{
			get => storage[key];
			set => storage[key] = value;
		}
		public bool ContainsKey(K key) => storage.ContainsKey(key);
		public bool TryGetValue(K key, out V value) => storage.TryGetValue(key, out value);
		public void Add(K key, V item) => storage.Add(key, item);
		public void Remove(K key) => storage.Remove(key);
		public List<V> GetAll() => storage.GetAll();
		public System.Collections.Generic.IList<V> Values => storage.Values;
		public int Count => storage.Count;
		public IEnumerable<K> Keys => storage.Keys;
		public IEnumerator<V> GetEnumerator() => storage.GetEnumerator();
		#endregion
	}

	/// <summary>
	/// Элемент словаря: связывает ключ со значением и принадлежит одному DDictionary.
	/// </summary>
	public class LKeyValue<K, V> : ItemDb<K> where V : class, IItemDb
	{
		public V Value { get; set; }
	}

	internal class DbLDictionary<K, V> : IDictionary<K, V> where V : class, IItemDb
	{
		private List<LKeyValue<K, V>> items;

		public DbLDictionary(List<LKeyValue<K, V>> argItems)
		{
			items = argItems;
		}

		public void Add(K key, V value)
		{
			items.Add(new LKeyValue<K, V> { Id = key, Value = value });
		}

		public bool Remove(K key)
		{
			var item = items.FirstOrDefault(kvp => kvp.Id.Equals(key));
			return item != null && items.Remove(item);
		}

		public bool TryGetValue(K key, out V value)
		{
			var kvp = items.FirstOrDefault(kvp => kvp.Id.Equals(key));
			if (kvp != null)
			{
				value = kvp.Value;
				return true;
			}
			value = null;
			return false;
		}

		public V this[K key]
		{
			get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
			set
			{
				var existing = items.FirstOrDefault(kvp => kvp.Id.Equals(key));
				if (existing != null)
					existing.Value = value;
				else
					items.Add(new LKeyValue<K, V> { Id = key, Value = value });
			}
		}

		public bool ContainsKey(K key) => items.Any(kvp => kvp.Id.Equals(key));

		public int Count => items.Count;
		public IEnumerable<K> Keys => items.Select(kvp => kvp.Id);
		public IEnumerable<V> Values => items.Select(kvp => kvp.Value);
		public List<V> GetAll() => items.Select(kvp => kvp.Value).ToList();
	}

}
