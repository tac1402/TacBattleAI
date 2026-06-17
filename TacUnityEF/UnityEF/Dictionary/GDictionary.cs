// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac;

namespace UnityEF
{
	/// <summary>
	/// Глобальный словарь в БД
	/// </summary>
	public class GDictionary<K, V> where V : class, IItemDb
	{
		private readonly IDictionary<K, V> storage;

		public GDictionary()
		{
			storage = CreateStorage();
		}

		private IDictionary<K, V> CreateStorage()
		{
			if (ItemDb.db == null)
			{
				return new MemoryDictionary<K, V>();
			}
			else
			{
				return new DbGDictionary<K, V>();
			}
		}

		#region default
		public V this[K key]
		{ 
			get =>storage [key];
			set => storage [key] = value;
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


	internal class DbGDictionary<K, V> : IDictionary<K, V> where V : class, IItemDb
	{
		private DbContext Db => ItemDb<K>.db; // статический контекст

		public V this[K key]
		{
			get
			{
				var item = Db.Set<V>().Find(key);
				if (item == null) throw new KeyNotFoundException();
				return item;
			}
			set { Add(key, value); }
		}

		public bool TryGetValue(K key, out V value)
		{
			value = Db.Set<V>().Find(key);
			return value != null;
		}

		public void Add(K key, V item)
		{
			var existing = Db.Set<V>().Find(key);
			if (existing != null)
			{
				Db.Entry(existing).CurrentValues.SetValues(item);
			}
			else
			{
				Db.Set<V>().Add(item);
			}
			//Db.SaveChanges();
		}

		public bool Remove(K key)
		{
			var item = Db.Set<V>().Find(key);
			if (item == null)
				return false;

			Db.Set<V>().Remove(item);
			//Db.SaveChanges();
			return true;
		}

		public List<V> GetAll() => Db.Set<V>().ToList();
		public IEnumerable<K> Keys => Db.Set<V>().Select(item => EF.Property<K>(item, "Id")).ToList();
		public int Count => Db.Set<V>().Count();
	}




}
