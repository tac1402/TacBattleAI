using DnaCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace UnityEF
{
    public class UnityDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// Указываем путь к файлу SQLite
			optionsBuilder.UseSqlite("Data Source=myapp.db;");
			//optionsBuilder.UseSqlite("DataSource=:memory:;");
		}

		private readonly List<Type> allTypes = new List<Type>();

		public void AddTypes(string assemblyName)
		{
			Type[] types = Assembly.Load(assemblyName).GetTypes();

			// Базовые сущности: все неабстрактные классы в вашем пространстве имён, реализующие ICell
			var baseTypes = types.Where(t => t.IsClass && !t.IsAbstract && typeof(ICell).IsAssignableFrom(t));

			foreach (Type t in baseTypes)
			{
				if (allTypes.Contains(t) == false)
				{
					allTypes.Add(t);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			AddTypes("Assembly-CSharp");


			// 2. находим все закрытые generic-типы DQueue<> и DItem<>, на которые есть ссылки в полях
			var toAdd = new HashSet<Type>();
			foreach (var type in allTypes)
			{
				foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
				{
					var ft = field.FieldType;
					if (ft.IsGenericType && ft.GetGenericTypeDefinition() == typeof(LQueue<>))
					{
						toAdd.Add(ft); // LQueue<T>
						var dItemType = typeof(LItem<>).MakeGenericType(ft.GetGenericArguments()[0]);
						toAdd.Add(dItemType); // LItem<T>
					}
					if (ft.IsGenericType && ft.GetGenericTypeDefinition() == typeof(LDictionary<,>))
					{
						toAdd.Add(ft); // LDictionary<K,V>
						var kvType = typeof(LKeyValue<,>).MakeGenericType(
							ft.GetGenericArguments()[0], // K
							ft.GetGenericArguments()[1]  // V
						);
						toAdd.Add(kvType);
					}
				}
			}
			foreach (var t in toAdd) allTypes.Add(t);

			// 3. Регистрируем все найденные типы как сущности
			foreach (var type in allTypes)
			{
				var entityBuilder = modelBuilder.Entity(type);

				entityBuilder.HasKey("Id");

				// Если тип - закрытый generic DQueue<T> или DItem<T>, задаём красивое имя таблицы
				if (type.IsGenericType)
				{
					var genericDef = type.GetGenericTypeDefinition();
					if (genericDef == typeof(LQueue<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LQueue_{argName}");
					}
					else if (genericDef == typeof(LItem<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LItem_{argName}");
					}
					if (genericDef == typeof(LDictionary<,>))
					{
						var argName = $"{type.GetGenericArguments()[0].Name}_{type.GetGenericArguments()[1].Name}";
						entityBuilder.ToTable($"LDictionary_{argName}");
					}
					else if (genericDef == typeof(LKeyValue<,>))
					{
						var argName = $"{type.GetGenericArguments()[0].Name}_{type.GetGenericArguments()[1].Name}";
						entityBuilder.ToTable($"LKeyValue_{argName}");
					}
				}
			}

			// 3. Теперь для каждой зарегистрированной сущности настраиваем поля и связи
			foreach (Type type in allTypes)
			{
				var entityBuilder = modelBuilder.Entity(type);

				bool isLQueue = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LQueue<>);
				bool isLItem = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LItem<>);
				bool isLDictionary = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LDictionary<,>);
				bool isLKeyValue = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LKeyValue<,>);

				if (isLDictionary)
				{
					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LDictionaryId");
				}
				else if (isLKeyValue)
				{
					// Связь со значением (V)
					entityBuilder.HasOne("Value")
						.WithMany()
						.HasForeignKey("ValueId");
				}
				else if (isLQueue)
				{
					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LQueueId"); ;
				}
				else if (isLItem)
				{
					entityBuilder.HasOne("Item")
						.WithMany()
						.HasForeignKey("ItemId");
				}
				else
				{
					var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
					foreach (var field in fields)
					{
						// Пропускаем помеченные [NotMapped]
						bool notMapped = field.GetCustomAttribute<NotMappedAttribute>() != null;
						if (notMapped) continue;

						var fieldType = field.FieldType;

						// Определяем, является ли тип поля сущностью (уже есть в модели)
						bool isEntity = modelBuilder.Model.FindEntityType(fieldType) != null;
						// Также проверяем, может ли тип быть коллекцией сущностей
						bool isListOfPrimitives = fieldType.IsGenericType &&
							fieldType.GetGenericTypeDefinition() == typeof(List<>) &&
							fieldType.GetGenericArguments()[0].IsPrimitive;

						bool isQueueOfPrimitives = fieldType.IsGenericType &&
							fieldType.GetGenericTypeDefinition() == typeof(Queue<>) &&
							fieldType.GetGenericArguments()[0].IsPrimitive;
						bool isSimple = IsSimpleType(fieldType);

						if (isEntity)
						{
							// Одиночная ссылка на другую сущность
							var fkName = $"{field.Name}Id";
							entityBuilder.HasOne(fieldType, field.Name)
								.WithMany();
							//.HasForeignKey(fkName);
							// Не вызываем Property!
						}
						else if (isListOfPrimitives)
						{
							// Коллекция сущностей
							var elementType = fieldType.GetGenericArguments()[0];
							entityBuilder.HasMany(elementType, field.Name)
								.WithOne();
							//.HasForeignKey($"{field.Name}_{elementType.Name}Id");
						}
						else if (isQueueOfPrimitives)
						{
						}
						else if (isSimple && IsDeclaredInMonoBehaviour(field) == false)
						{
							// Примитив, строка, структура, enum и т.п. — обычный столбец
							entityBuilder.Property(fieldType, field.Name).HasField(field.Name);
						}
					}


					var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
					foreach (var prop in properties)
					{
						if (prop.Name != "Id")
						{
							entityBuilder.Ignore(prop.Name);
						}
					}
				}
			}
		}

		private bool IsSimpleType(Type type)
		{
			// Обработка nullable-типов (int?, DateTime? и т.д.)
			type = Nullable.GetUnderlyingType(type) ?? type;

			// Получаем TypeCode
			TypeCode code = Type.GetTypeCode(type);

			// Считаем простыми все типы, кроме Object
			return code != TypeCode.Object;
		}

		public bool IsDeclaredInMonoBehaviour(FieldInfo field)
		{
			Type type = field.DeclaringType;
			return type != null && type == typeof(MonoBehaviour);
		}

		public void DebugModel()
		{
			// Используем стандартный механизм отладки EF Core для получения "длинного" представления
			string modelDebugView = Model.ToDebugString();

			// Можно сохранить это в файл или просто вывести в консоль для быстрого анализа
			File.WriteAllText("model_debug.txt", modelDebugView);
		}

	}
}
