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
	public class LDictionary<K, V> : ItemDb where V : ItemDb
	{
		public List<LKeyValue<K, V>> Items { get; set; } = new List<LKeyValue<K, V>>();

		// Удобное свойство для работы со словарём (не сохраняется в БД)
		[NotMapped]
		public Dictionary<K, V> Dictionary
		{
			get => Items.ToDictionary(kvp => kvp.Id, kvp => kvp.Value);
			set
			{
				Items = value.Select(kvp => new LKeyValue<K, V>
				{
					Id = kvp.Key,
					Value = kvp.Value
				}).ToList();
			}
		}

		public void Add(K key, V value)
		{
			Items.Add(new LKeyValue<K, V> { Id = key, Value = value });
		}

		public bool Remove(K key)
		{
			var item = Items.FirstOrDefault(kvp => kvp.Id.Equals(key));
			return item != null && Items.Remove(item);
		}

		public bool TryGetValue(K key, out V value)
		{
			var kvp = Items.FirstOrDefault(kvp => kvp.Id.Equals(key));
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
				var existing = Items.FirstOrDefault(kvp => kvp.Id.Equals(key));
				if (existing != null)
					existing.Value = value;
				else
					Items.Add(new LKeyValue<K, V> { Id = key, Value = value });
			}
		}

		public bool ContainsKey(K key) => Items.Any(kvp => kvp.Id.Equals(key));

		[NotMapped]
		public int Count => Items.Count;

		[NotMapped]
		public IEnumerable<K> Keys => Items.Select(kvp => kvp.Id);

		[NotMapped]
		public IEnumerable<V> Values => Items.Select(kvp => kvp.Value);

	}

	/// <summary>
	/// Элемент словаря: связывает ключ со значением и принадлежит одному DDictionary.
	/// </summary>
	public class LKeyValue<K, V> : ItemDb<K> where V : ItemDb
	{
		public V Value { get; set; }
	}
}
