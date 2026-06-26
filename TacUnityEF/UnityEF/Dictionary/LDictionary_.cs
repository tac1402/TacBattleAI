// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tac;

namespace UnityEF
{

	/// <summary>
	/// Локальный словарь в БД
	/// </summary>
	public class LDictionary_<K, V> : ItemDb, IOrmCollection, IEnumerable<KeyValuePair<K, V>>
	{
		private readonly IDictionary<K, V> storage;

		public LDictionary_()
		{
			storage = CreateStorage();
		}

		private IDictionary<K, V> CreateStorage()
		{
			if (db == null)
			{
				return new MemoryDictionary_<K, V>();
			}
			else
			{
				return new DbLDictionary_<K, V>(Items);
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
		[NotMapped]
		public System.Collections.Generic.IList<V> Values => storage.Values;
		public int Count => storage.Count;
		public IEnumerable<K> Keys => storage.Keys;
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => storage.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerable<KeyValuePair<K, V>> Where(Func<KeyValuePair<K, V>, bool> predicate) => storage.Where(predicate);
		#endregion
	}

	internal class DbLDictionary_<K, V> : IDictionary<K, V>
	{
		private List<LKeyValue<K, V>> items;
		private HashSet<K> nullKeys;         // ключи с null-значением

		public DbLDictionary_(List<LKeyValue<K, V>> argItems)
		{
			items = argItems;
			nullKeys = new HashSet<K>();
		}

		public void Add(K key, V value)
		{
			if (value == null)
			{
				nullKeys.Add(key);
			}
			else
			{
				items.Add(new LKeyValue<K, V> { Key = key, Value = value });
			}
		}

		public bool Remove(K key)
		{
			bool removed = false;
			var item = items.FirstOrDefault(kvp => kvp.Key.Equals(key));

			if (item != null)
			{
				items.Remove(item);
				removed = true;
			}
			if (nullKeys.Remove(key))
			{
				removed = true;
			}
			return removed;
		}

		public bool TryGetValue(K key, out V value)
		{
			if (nullKeys.Contains(key))
			{
				value = default(V);
				return true;
			}
			var kvp = items.FirstOrDefault(kvp => kvp.Key.Equals(key));
			if (kvp != null)
			{
				value = kvp.Value;
				return true;
			}
			value = default(V);
			return false;
		}

		public V this[K key]
		{
			get => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
			set
			{
				if (value == null)
				{
					// Удаляем из items, добавляем в nullKeys
					var existingItem = items.FirstOrDefault(kvp => kvp.Key.Equals(key));
					if (existingItem != null)
					{
						items.Remove(existingItem);
						nullKeys.Add(key);
					}
					else if (nullKeys.Contains(key) == false)
					{
						nullKeys.Add(key);
					}
				}
				else
				{
					nullKeys.Remove(key);
					var existing = items.FirstOrDefault(kvp => kvp.Key.Equals(key));
					if (existing != null)
					{
						existing.Value = value;
					}
					else
					{
						items.Add(new LKeyValue<K, V> { Key = key, Value = value });
					}
				}
			}
		}

		public bool ContainsKey(K key) => nullKeys.Contains(key) || items.Any(kvp => kvp.Key.Equals(key));

		public int Count => items.Count + nullKeys.Count;
		public IEnumerable<K> Keys => items.Select(kvp => kvp.Key);
		public List<V> GetAll() => items.Select(kvp => kvp.Value).ToList();

		public IEnumerable<KeyValuePair<K, V>> Where(Func<KeyValuePair<K, V>, bool> predicate)
		{
			var fromItems = items.Select(item => new KeyValuePair<K, V>(item.Key, item.Value));
			return fromItems.Where(predicate);
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => items.Select(kvp => new KeyValuePair<K, V>(kvp.Key, kvp.Value)).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}


}
