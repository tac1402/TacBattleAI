// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Tac;

namespace UnityEF
{
	/// <summary>
	/// Локальный список в БД
	/// </summary>
	public class LList<T> : ItemDb, IOrmCollection, IEnumerable<T>
		where T : class, IItemDb
	{
		private readonly IList<T> storage;

		public LList()
		{
			storage = CreateStorage();
		}

		private IList<T> CreateStorage()
		{
			if (db == null)
			{
				return new MemoryList<T>();
			}
			else
			{
				return new DbLList<T>(Items);
			}
		}

		public List<LItem<T>> Items { get; set; } = new List<LItem<T>>();

		public T this[int key] 
		{ 
			get => storage[key]; 
			set => storage[key] = value; 
		}
		public void Add(T item) => storage.Add(item);
		public void RemoveAt(int index) => storage.RemoveAt(index);
		public void Clear() => storage.Clear();
		public int Count => storage.Count;
		public IEnumerator<T> GetEnumerator() => storage.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Find(Predicate<T> match) => storage.Find(match);
		public List<T> FindAll(Predicate<T> match) => storage.FindAll(match);
	}

	internal class DbLList<T> : IList<T> where T : class, IItemDb
	{
		private List<LItem<T>> items;

		public DbLList(List<LItem<T>> argItems)
		{
			items = argItems;
		}

		// Добавляет элемент в конец списка
		public void Add(T item)
		{
			items.Add(new LItem<T>(item));
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			items.RemoveAt(index);
		}

		public void Clear()
		{
			items.Clear();
		}

		public T this[int index]
		{
			get => items[index].Item;
			set => items[index] = new LItem<T>(value);
		}

		public bool Contains(T item)
		{
			if (item == null) return false;
			return items.Any(li => li.Item.item.Id == item.item.Id);
		}

		public int Count => items.Count;
		public IEnumerator<T> GetEnumerator() => items.Select(k => k.Item).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public T Find(Predicate<T> match)
		{
			if (match == null) throw new ArgumentNullException(nameof(match));
			foreach (var li in items)
				if (match(li.Item)) return li.Item;
			return null;
		}

		public List<T> FindAll(Predicate<T> match)
		{
			if (match == null) throw new ArgumentNullException(nameof(match));
			var result = new List<T>();
			foreach (var li in items)
				if (match(li.Item)) result.Add(li.Item);
			return result;
		}
	}
}
