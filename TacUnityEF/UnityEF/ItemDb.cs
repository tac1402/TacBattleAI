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
		public K Id { get; set; }

		public static DbContext db;

		private static HashSet<object> saving = new HashSet<object>(); // для избежания циклов

		public void SaveGraph<T>(T root)
		{
			Save(root);          // рекурсивно присоединяет все объекты
			db.SaveChanges();    // единственный вызов
		}

		private void Save<T>(T obj)
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

					// Проверяем, является ли поле коллекцией элементов, реализующих IItemDb
					bool isCollectionOfEntities = false;
					var enumeratorInterface = fieldType.GetInterface(typeof(IEnumerator<>).FullName);
					if (enumeratorInterface != null)
					{
						var elementType = enumeratorInterface.GetGenericArguments()[0];
						if (typeof(IItemDb).IsAssignableFrom(elementType))
						{
							isCollectionOfEntities = true;
						}
					}


					// Одиночная ссылка на сущность
					if (typeof(IItemDb).IsAssignableFrom(fieldType) && isCollectionOfEntities == false)
					{
						// Приводим к интерфейсу и через свойство item вызываем Save()
						((IItemDb)fieldValue).item.Save(fieldValue);
					}
					// Коллекция сущностей GDictionary, LList, LQueue, LDictionary
					else if (isCollectionOfEntities == true)
					{
						var collection = (System.Collections.IEnumerable)fieldValue;
						foreach (var item in collection)
						{
							if (item != null)
							{
								((IItemDb)item).item.Save(item); // рекурсивный вызов
							}
						}
					}
				}
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
