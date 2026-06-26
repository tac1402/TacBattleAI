// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Tac;
using UnityEF;
using UnityEngine;

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

		//public string DebugInfo { get; set; }

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
					var enumeratorInterface = fieldType.GetInterface(typeof(UnityEF.IOrmCollection).FullName);
					if (enumeratorInterface != null)
					{
						isCollectionOfEntities = true;
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
						//SetDiscriminator(obj, field, fieldValue);
						var collection = GetCollection(fieldValue);
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

		/*private void SetDiscriminator(object owner, FieldInfo field, object fieldValue)
		{
			if (fieldValue is ItemDb container && container.DebugInfo == null)
			{
				container.DebugInfo = $"{owner.GetType().Name}.{field.Name}";
			}
		}*/


		public IEnumerable<object> GetCollection(object storage)
		{
			if (storage == null)
				throw new ArgumentNullException(nameof(storage));

			Type storageType = storage.GetType();

			// Получаем все обобщённые аргументы типа
			Type[] genericArgs = storageType.GetGenericArguments();

			// Проверяем, есть ли среди них примитив
			bool hasPrimitiveArg = false;
			foreach (Type arg in genericArgs)
			{
				if (UnityDbContext.IsSimpleType(arg))
				{
					hasPrimitiveArg = true;
					break;
				}
			}

			if (hasPrimitiveArg == true)
			{
				yield break; 
			}

			// Ищем публичный метод GetEnumerator без параметров
			var method = storageType.GetMethod("GetEnumerator", Type.EmptyTypes);
			if (method != null)
			{
				// Вызываем метод и получаем IEnumerator (он может быть обобщённым, но мы используем IEnumerator)
				var enumerator = method.Invoke(storage, null) as System.Collections.IEnumerator;
				if (enumerator != null)
				{
					// Перебираем все элементы
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}
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
			//db.SaveChanges();
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
