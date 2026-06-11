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
	/// <summary>
	/// Локальный список
	/// </summary>
	public class LList<T> : ItemDb where T : class, IItemDb
	{
		public List<LItem<T>> Items { get; set; } = new List<LItem<T>>();

		// Не сохраняется в БД, используется для удобного доступа к элементам в порядке Id
		[NotMapped]
		public List<LItem<T>> List
		{
			get => Items.OrderBy(i => i.Id).ToList();
			set => Items = value.ToList();
		}

		// Добавляет элемент в конец списка (работает через временную копию — аналогично LQueue)
		public void Add(T item)
		{
			List.Add(new LItem<T>(item));
		}

		// Удаляет первое вхождение элемента (работает через временную копию)
		public bool Remove(T item)
		{
			var lItem = List.FirstOrDefault(i => i.Item == item);
			if (lItem != null)
				return List.Remove(lItem);
			return false;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= List.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			List.RemoveAt(index);             // удаляем элемент по индексу
		}

		public void Clear()
		{
			Items.Clear();
		}

		// Индексатор для доступа по позиции (сортировка по Id)
		public T this[int index]
		{
			get => List[index].Item;
			set => List[index] = new LItem<T>(value);
		}

		// Опционально: получить количество элементов
		[NotMapped]
		public int Count => List.Count;
	}

}
