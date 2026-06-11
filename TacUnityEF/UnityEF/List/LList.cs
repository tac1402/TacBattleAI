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
	/// Локальный список в БД
	/// </summary>
	public class LList<T> : ItemDb where T : class, IItemDb
	{
		private readonly IList<T> storage;

		public LList()
		{
			storage = CreateStorage();
		}

		private IList<T> CreateStorage()
		{
			if (ItemDb<T>.db == null)
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
	}

	internal class DbLList<T> : IList<T> where T : class, IItemDb
	{
		private List<LItem<T>> items;

		public DbLList(List<LItem<T>> argItems)
		{
			items = argItems;
		}

		// Добавляет элемент в конец списка (работает через временную копию — аналогично LQueue)
		public void Add(T item)
		{
			items.Add(new LItem<T>(item));
		}

		// Удаляет первое вхождение элемента (работает через временную копию)
		public bool Remove(T item)
		{
			var lItem = items.FirstOrDefault(i => i.Item == item);
			if (lItem != null)
				return items.Remove(lItem);
			return false;
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

		public int Count => items.Count;
		public IEnumerator<T> GetEnumerator() => items.Select(k => k.Item).GetEnumerator();
	}

}
