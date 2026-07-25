// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
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
		public virtual K Id { get; set; }

		//public string DebugInfo { get; set; }

		public static DbContext db;

		private static HashSet<object> saving = new HashSet<object>(); // для избежания циклов при сохранении
		private static HashSet<object> loading = new HashSet<object>(); // для избежания циклов при загрузке
		public static bool RecoverMode = false;

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

		/// <summary>
		/// Загружает данные из БД в текущий объект (он уже должен существовать в памяти).
		/// Обновляются все поля, включая навигационные свойства (одиночные ссылки и коллекции).
		/// </summary>
		public void LoadGraph(object root)
		{
			RecoverMode = true;
			// 1. Получаем Id корневого объекта
			var id = (int)root.GetType().GetProperty("Id")?.GetValue(root);
			if (id == 0)
				throw new InvalidOperationException("Root entity has no Id.");

			Dictionary<string, int> foreignKeys = LoadEntity(root, id);

			// 6. Теперь рекурсивно загружаем навигационные свойства (одиночные ссылки и коллекции)
			Load(root, foreignKeys);
			RecoverMode = false;
		}

		private Dictionary<string, int> LoadEntity(object obj, int id)
		{
			// 1. Загружаем сущность из БД (она будет отслеживаться)
			object loaded = db.Find(obj.GetType(), id);
			return RefreshEntity(obj, loaded);
		}

		private Dictionary<string, int> PostLoadEntity(out object obj, object loaded)
		{
			var addDelegate = Flow.Reg.GetAdd(loaded.GetType());
			if (addDelegate != null)
			{
				obj = addDelegate(loaded);
				return RefreshEntity(obj, loaded);
			}
			else
			{
				obj = null;
				return null;
			}
		}

		private Dictionary<string, int> RefreshEntity(object obj, object loaded)
		{
			// 2. Отсоединяем loaded, чтобы избежать дублирования ключей
			EntityEntry loadedEntry = db.Entry(loaded);
			loadedEntry.State = EntityState.Detached;

			// 3. Присоединяем новый к контексту, если он ещё не отслеживается
			EntityEntry objEntry = db.Entry(obj);
			if (objEntry.State == EntityState.Detached)
			{
				db.Attach(obj);
				objEntry = db.Entry(obj);
			}

			// 4. Копируем все скалярные и shadow свойства из loaded в objEntry (без затрагивания навигационных)
			//objEntry.CurrentValues.SetValues(loadedEntry.CurrentValues);

			// 4. Копируем только скалярные и shadow свойства (НЕ навигационные)
			var entityType = db.Model.FindEntityType(obj.GetType());
			Dictionary<string, int> foreignKeys = new Dictionary<string, int>();
			foreach (var prop in entityType.GetProperties())
			{
				var value = loadedEntry.Property(prop.Name).CurrentValue;
				if (prop.IsForeignKey() == true)
				{
					if (value != null && (int)value != 0)
					{
						foreignKeys[prop.Name] = (int)value;
					}
				}
				else
				{
					objEntry.Property(prop.Name).CurrentValue = value;
				}
			}

			return foreignKeys;
		}


		// Рекурсивная загрузка для произвольного объекта
		private void Load(object obj, Dictionary<string, int> argForeignKeys)
		{
			if (obj == null) return;
			if (loading.Contains(obj)) return; // циклическая ссылка
			loading.Add(obj);

			try
			{
				var entry = db.Entry(obj);
				var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

				foreach (var field in fields)
				{
					var fieldType = field.FieldType;
					var fieldValue = field.GetValue(obj);
					bool isCollection = fieldType.GetInterface(typeof(UnityEF.IOrmCollection).FullName) != null;

					// Одиночная ссылка на сущность
					if (typeof(IItemDb).IsAssignableFrom(fieldType) && !isCollection)
					{
						// Получаем значение внешнего ключа (shadow property)
						string fkName = field.Name + "Id";
						int id = argForeignKeys[fkName];

						if (id != 0)
						{
							var currentValue = field.GetValue(obj);
							if (currentValue != null)
							{
								Dictionary<string, int>  foreignKeys = LoadEntity(currentValue, id);

								// Рекурсивно загружаем граф для currentValue
								Load(currentValue, foreignKeys);
							}
							else if (currentValue == null)
							{
								// Если поля нет – присвоить новый объект (но это должно быть редко)
								var loaded = db.Find(fieldType, id);
								if (loaded != null)
								{
									field.SetValue(obj, loaded);
									Load(loaded, null);
								}
							}
						}
					}
					// Коллекция сущностей
					else if (isCollection)
					{
						if (fieldType.IsGenericType &&
								(fieldType.GetGenericTypeDefinition() == typeof(LDictionary_<,>) ||
								 fieldType.GetGenericTypeDefinition() == typeof(LList_<>) ||
								 fieldType.GetGenericTypeDefinition() == typeof(LQueue_<>)))
						{
							// Получаем значение внешнего ключа через shadow property
							string fkName = field.Name + "Id";
							if (argForeignKeys.ContainsKey(fkName))
							{
								int id = argForeignKeys[fkName];
								if (id != 0)
								{
									// Загружаем LDictionary_ по Id
									var child = db.Find(fieldType, id);
									if (child != null)
									{
										// Присваиваем загруженный объект полю
										field.SetValue(obj, child);

										// 3. Загружаем коллекцию Items (LKeyValue)
										var childEntry = db.Entry(child);
										childEntry.Collection("Items").Load();
									}
								}
							}
						}
						else if (fieldType.IsGenericType &&
								(fieldType.GetGenericTypeDefinition() == typeof(LDictionary<,>) ||
								 fieldType.GetGenericTypeDefinition() == typeof(LList<>) ||
								 fieldType.GetGenericTypeDefinition() == typeof(LQueue<>)))
						{
							// Получаем значение внешнего ключа через shadow property
							string fkName = field.Name + "Id";
							if (argForeignKeys.ContainsKey(fkName))
							{
								int id = argForeignKeys[fkName];

								if (id != 0)
								{
									// Загружаем по Id
									var child = db.Find(fieldType, id);
									if (child != null)
									{
										// Присваиваем загруженный объект полю
										field.SetValue(obj, child);

										// 3. Загружаем коллекцию Items (LKeyValue)
										var childEntry = db.Entry(child);

										CollectionEntry collectionEntry = childEntry.Collection("Items");
										collectionEntry.Load();

										var items = collectionEntry.CurrentValue as IEnumerable;
										if (items != null)
										{
											foreach (var item in items)
											{
												// item - это LKeyValue<K,V>, у него есть свойство Value
												var valueProp = item.GetType().GetProperty("Value");
												if (valueProp != null)
												{
													var valueObj = valueProp.GetValue(item);
													/*if (valueObj is IItemDb)
														Load(valueObj); // рекурсивно загружаем граф сущности
														*/
												}
											}
										}
									}
								}
							}
						}
						// Проверяем, является ли коллекция GDictionary<,>
						else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(GDictionary<,>))
						{
							Type valueType = fieldType.GetGenericArguments()[1]; // V

							// ---------- Получение данных из БД с учётом иерархии ----------
							// 1. Получаем метаданные сущности
							var entityType = db.Model.FindEntityType(valueType);
							if (entityType == null) continue;

							// 2. Для TPH берём корневой тип (таблица Agent)
							var rootType = entityType.GetRootType();

							// 3. Получаем DbSet<T> для корневого типа через рефлексию
							//    Вызов db.Set<rootType.ClrType>()
							var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
							var setGeneric = setMethod.MakeGenericMethod(rootType.ClrType);
							IQueryable rootDbSet = setGeneric.Invoke(db, null) as IQueryable; // IQueryable<rootType>

							/*var setMethod2 = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
							var setGeneric2 = setMethod2.MakeGenericMethod(valueType);
							var dbSet = setGeneric2.Invoke(db, null) as IQueryable; // IQueryable<valueType>

							// 4. Применяем OfType через рефлексию для фильтрации по дискриминатору
							var ofTypeMethod = typeof(Queryable).GetMethod("OfType", BindingFlags.Public | BindingFlags.Static);
							var ofTypeGeneric = ofTypeMethod.MakeGenericMethod(valueType);
							var query = ofTypeGeneric.Invoke(db, new object[] { rootDbSet }) as IQueryable;*/

							var toListMethod = typeof(Enumerable)
								.GetMethod("ToList", BindingFlags.Public | BindingFlags.Static)
								.MakeGenericMethod(rootType.ClrType);
							var allEntities = toListMethod.Invoke(null, new[] { rootDbSet }) as System.Collections.IList;


							foreach (var entity in allEntities)
							{
								object component = null;
								Dictionary<string, int> foreignKeys = PostLoadEntity(out component, entity);
								Load(component, foreignKeys);
							}
						}
					}
				}
			}
			finally
			{
				loading.Remove(obj);
			}
		}


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
			Flow locItem = locObject.GetComponent<Flow>();
			return locItem.item.cell as T;
		}

		public static new T Create<T>(T obj, string dllName, string role = "") where T : class, ICell
		{
			return Cell<T>.Create(obj, dllName, role, CreateInstance<T>);
		}

	}
}
