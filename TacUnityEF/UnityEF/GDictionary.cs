// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEF
{
	public class GDictionary<K, V> where V : class, IItemDb
	{

		public GDictionary()
		{
		}

		private DbContext Db => ItemDb<K>.db; // статический контекст

		public V this[K key]
		{
			get
			{
				var item = Db.Set<V>().Find(key);
				if (item == null) throw new KeyNotFoundException();
				return item;
			}
		}

		public bool ContainsKey(K key) => Db.Set<V>().Find(key) != null;

		public bool TryGetValue(K key, out V value)
		{
			value = Db.Set<V>().Find(key);
			return value != null;
		}

		// Добавление – сразу сохраняет (можно и без SaveChanges, если полагаться на внешний вызов)
		public void Add(K key, V item)
		{
			// Проверяем, существует ли запись с таким ключом
			var existing = Db.Set<V>().Find(key);

			if (existing != null)
			{
				// Обновляем существующую запись значениями из item
				Db.Entry(existing).CurrentValues.SetValues(item);
			}
			else
			{
				// Добавляем новую запись
				Db.Set<V>().Add(item);
			}

			Db.SaveChanges();
		}

		public void Remove(V item)
		{
			Db.Set<V>().Remove(item);
			Db.SaveChanges();
		}

		public void RemoveByKey(K key)
		{
			if (TryGetValue(key, out var item))
				Remove(item);
		}

		public List<V> GetAll() => Db.Set<V>().ToList();

		public IList<V> Values => GetAll();

		// Для поддержки foreach
		public IEnumerator<V> GetEnumerator() => GetAll().GetEnumerator();
	}
}
