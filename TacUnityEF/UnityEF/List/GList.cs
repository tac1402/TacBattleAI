// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Tac;

namespace UnityEF
{
	/// <summary>
	/// Глобальный список в БД
	/// </summary>
	public class GList<T> : IOrmCollection, IEnumerable<T> 
		where T : class, IItemDb
	{
		private readonly IList<T> storage;

		public GList()
		{
			storage = CreateStorage();
		}

		private IList<T> CreateStorage()
		{
			if (ItemDb.db == null)
			{
				return new MemoryList<T>();
			}
			else
			{
				return new DbGList<T>();
			}
		}

		public T this[int key]
		{
			get => storage[key];
			set => storage[key] = value;
		}
		public void Add(T item) => storage.Add(item);
		public void RemoveAt(int index) => storage.RemoveAt(index);
		public void Clear() => storage.Clear();
		public bool Contains(T item) => storage.Contains(item);
		public int Count => storage.Count;
		public IEnumerator<T> GetEnumerator() => storage.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Find(Predicate<T> match) => storage.Find(match);
		public List<T> FindAll(Predicate<T> match) => storage.FindAll(match);
	}

	internal class DbGList<T> : IList<T> where T : class, IItemDb
	{
		private DbContext Db => ItemDb.db;
		private DbSet<T> ItemsDb => Db.Set<T>();


		// Индексатор – запрос по индексу через Skip/Take
		public T this[int index]
		{
			get
			{
				var li = ItemsDb.OrderBy(li => GetId(li)).Skip(index).FirstOrDefault();
				if (li == null) throw new ArgumentOutOfRangeException(nameof(index));
				return li;
			}
			set 
			{
				var li = ItemsDb.OrderBy(li => GetId(li)).Skip(index).FirstOrDefault();
				if (li == null) throw new ArgumentOutOfRangeException(nameof(index));
				Db.Entry(li).CurrentValues.SetValues(value);
			}
		}

		public void Add(T item)
		{
			// Ищем существующую обёртку, содержащую элемент с таким же Id
			var existing = ItemsDb.Find(item.item.Id);
			if (existing != null)
			{
				// Обновляем значение элемента
				Db.Entry(existing).CurrentValues.SetValues(item);
			}
			else
			{
				// Добавляем новую запись
				ItemsDb.Add(item);
			}
		}


		public int Count => ItemsDb.Count();

		public void Clear()
		{
			ItemsDb.RemoveRange(Db.Set<T>());
		}

		// Проверка наличия – по Id
		public bool Contains(T item)
		{
			return ItemsDb.Any(li => GetId(li) == item.item.Id);
		}

		// Удаление по объекту – по Id
		public bool Remove(T item)
		{
			var li = ItemsDb.FirstOrDefault(li => GetId(li) == item.item.Id);
			if (li == null) return false;
			ItemsDb.Remove(li);
			return true;
		}

		// Удаление по индексу – через Skip/Take
		public void RemoveAt(int index)
		{
			var li = ItemsDb.OrderBy(li => GetId(li)).Skip(index).FirstOrDefault();
			if (li == null) throw new ArgumentOutOfRangeException(nameof(index));
			ItemsDb.Remove(li);
		}

		// Вспомогательный метод получения Id (как в GDictionary)
		private int GetId(T item) => EF.Property<int>(item, "Id");

		public IEnumerator<T> GetEnumerator() => ItemsDb.Select(item => item).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Find(Predicate<T> match)
		{
			if (match == null) throw new ArgumentNullException(nameof(match));
			foreach (var li in ItemsDb.ToList()) // загружаем всё в память
				if (match(li)) return li;
			return null;
		}

		public List<T> FindAll(Predicate<T> match)
		{
			if (match == null) throw new ArgumentNullException(nameof(match));
			var result = new List<T>();
			foreach (var li in ItemsDb.ToList())
				if (match(li)) result.Add(li);
			return result;
		}

	}


}
