// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using UnityEngine;
using Tac;

namespace DnaCore
{
	public interface IItemDb
	{
		public ItemDb item { get; }
	}

	public class ItemDb : ItemDb<int>, IItemDb, ICell
	{
		public ItemDb item
		{
			get { return this; }
		}
	}

	public class ItemDb<K> : Cell
	{
		/// <summary>
		/// Уникальный индентификатор объекта в мире
		/// </summary>
		public K Id;

		public static DbContext db;

		private static HashSet<object> saving = new HashSet<object>(); // для избежания циклов

		public void Save<T>(T obj)
		{
			// Если объект уже в процессе сохранения (циклическая ссылка), выходим
			if (saving.Contains(obj)) return;
			saving.Add(obj);

			try
			{
				// Присоединяем текущий объект, если нужно
				var entry = db.Entry(obj);
				if (entry.State == EntityState.Detached)
				{
					if (IsNew())
					{
						db.Add(obj);
					}
					else
					{
						db.Update(obj);
					}
				}

				// Рекурсивно обходим все поля, которые являются сущностями или коллекциями сущностей
				var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var field in fields)
				{
					var fieldType = field.FieldType;
					var fieldValue = field.GetValue(obj);

					if (fieldValue == null) { continue; }

					// Одиночная ссылка на сущность (наследник Item<>)
					if (typeof(ItemDb<>).IsAssignableFrom(fieldType) && fieldType.IsGenericType)
					{
						((dynamic)fieldValue).Save(); // рекурсивный вызов
					}
					// Коллекция сущностей (List<T>, LQueue<T>, LDictionary<K,V> и т.д.)
					else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldType) &&
							 fieldType.IsGenericType)
					{
						var elementType = fieldType.GetGenericArguments()[0];
						if (typeof(ItemDb<>).IsAssignableFrom(elementType) && elementType.IsGenericType)
						{
							var collection = (System.Collections.IEnumerable)fieldValue;
							foreach (var item in collection)
							{
								if (item != null)
								{
									((dynamic)item).Save(); // рекурсивный вызов
								}
							}
						}
					}
				}

				// Сохраняем все изменения в базе (один раз для всего графа)
				db.SaveChanges();
			}
			finally
			{
				saving.Remove(obj);
			}
		}

		// Удалить
		public void Delete()
		{
			var entry = db.Entry(this);
			if (entry.State == EntityState.Detached)
			{
				db.Attach(this);
			}
			db.Remove(this);
			db.SaveChanges();
		}

		// Определяет, является ли объект новым (ещё не сохранённым в БД)
		private bool IsNew()
		{
			// Если Id имеет значение по умолчанию (0 для int, null для string и т.д.), считаем новым
			return EqualityComparer<K>.Default.Equals(Id, default(K));
		}


		#region Model

		/// <summary>
		/// Все модели 
		/// </summary>
		public static List<GameObject> Models = new List<GameObject>();
		/// <summary>
		/// Индексация моделей по имени и типу
		/// </summary>
		public static Dictionary<string, int> IndexList = new Dictionary<string, int>();


		/// <summary>
		/// Получить модель по идентификации
		/// </summary>
		public static GameObject GetModel(string argModelName)
		{
			GameObject retModel = null;
			try
			{
				retModel = Models[IndexList[argModelName]];
			}
			catch (Exception) { }
			return retModel;
		}

		#endregion


		private static T CreateInstance<T>(string name) where T : class, ICell
		{
			GameObject locObject = UnityEngine.Object.Instantiate(GetModel(name));
			Item locItem = locObject.GetComponent<Item>();
			return locItem.item.cell as T;
		}

		public static new T Create<T>(T obj, string dllName, string role = "") where T : class, ICell
		{
			return Cell<T>.Create(obj, dllName, role, CreateInstance<T>);
		}

	}
}
