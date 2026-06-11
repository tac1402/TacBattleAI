using DnaCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using Tac;
using UnityEngine;
using UnityEngine.Rendering;


namespace UnityEF
{
    public class UnityDbContext : DbContext
	{

		private HashSet<Type> customPrimitives = new HashSet<Type>
		{
			typeof(LVector3),
			typeof(LVector2)
		};

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// Указываем путь к файлу SQLite
			optionsBuilder.UseSqlite("Data Source=myapp.db;");
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
					if (ft.IsGenericType && ft.GetGenericTypeDefinition() == typeof(LList<>))
					{
						toAdd.Add(ft); // LList<T>
						var dItemType = typeof(LItem<>).MakeGenericType(ft.GetGenericArguments()[0]);
						toAdd.Add(dItemType); // LItem<T>
					}
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

				if (type.BaseType == null || allTypes.Contains(type.BaseType) == false)
				{
					entityBuilder.HasKey("Id");
				}

				// Если тип - закрытый generic DQueue<T> или DItem<T>, задаём красивое имя таблицы
				if (type.IsGenericType)
				{
					var genericDef = type.GetGenericTypeDefinition();
					if (genericDef == typeof(LList<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LList_{argName}");
					}
					else if (genericDef == typeof(LQueue<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LQueue_{argName}");
					}
					else if (genericDef == typeof(LItem<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LItem_{argName}");
					}
					else if (genericDef == typeof(LDictionary<,>))
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
				else
				{
					// Задаём имя таблицы = имени класса - только для стратегии TPT (Table per Type)
					//entityBuilder.ToTable(type.Name);
				}
			}

			// 3. Теперь для каждой зарегистрированной сущности настраиваем поля и связи
			foreach (Type type in allTypes)
			{
				var entityBuilder = modelBuilder.Entity(type);

				bool isLList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LList<>);
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
					var keyType = type.GetGenericArguments()[0];
					var valueType = type.GetGenericArguments()[1];

					// Если ValueType – простой (включая кастомные), храним как строку
					if (IsSimpleType(valueType))
					{
						entityBuilder.Property(valueType, "Value")
							.HasConversion(GetValueConverter(valueType))
							.HasField("Value"); // предполагаем, что поле называется _value
					}
					else
					{
						// Иначе – связь с сущностью
						entityBuilder.HasOne("Value")
							.WithMany()
							.HasForeignKey("ValueId");
					}

				}
				else if (isLList)
				{
					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LListId"); ;
				}
				else if (isLQueue)
				{
					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LQueueId"); ;
				}
				else if (isLItem)
				{
					var itemType = type.GetGenericArguments()[0]; // T в LItem<T>
					if (IsSimpleType(itemType))
					{
						// T – простой (включая Vector3 и аналоги), храним Item как столбец с конвертером
						entityBuilder.Property(itemType, "Item")
							.HasConversion(GetValueConverter(itemType))
							.HasField("Item");
					}
					else
					{
						// T – сложная сущность – настраиваем связь
						entityBuilder.HasOne("Item")
							.WithMany()
							.HasForeignKey("ItemId");
					}
				}
				else
				{
					//var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
					var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
						.Where(f => (f.IsPublic || (f.IsPrivate && f.IsDefined(typeof(MappedAttribute), false)))
							&& f.IsDefined(typeof(NotMappedAttribute), false) == false);

					foreach (var field in fields)
					{
						var fieldType = field.FieldType;

						// Определяем, является ли тип поля сущностью (уже есть в модели)
						bool isEntity = modelBuilder.Model.FindEntityType(fieldType) != null;
						bool isSimple = IsSimpleType(fieldType);

						// Рекомендуется использовать LList<T> или LQueue<T> для реляционного хранения.
						bool isList = fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>);
						bool isQueue = fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Queue<>);

						if (isList || isQueue)
						{
							// Просто игнорируем такие поля
							entityBuilder.Ignore(field.Name);
							continue;
						}

						if (isEntity)
						{
							// Одиночная ссылка на другую сущность
							var fkName = $"{field.Name}Id";
							entityBuilder.HasOne(fieldType, field.Name)
								.WithMany();
						}
						else if (isSimple && IsDeclaredInMonoBehaviour(field) == false)
						{
							var propertyBuilder = entityBuilder.Property(fieldType, field.Name).HasField(field.Name);

							if (IsCustomPrimitive(fieldType))
							{
								propertyBuilder.HasConversion(GetValueConverter(fieldType));
							}
						}
					}

					bool isRoot = (type.BaseType == null) || allTypes.Contains(type.BaseType) == false;

					BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
					if (isRoot == false)
					{
						flags |= BindingFlags.DeclaredOnly;
					}

					PropertyInfo[] properties = type.GetProperties(flags);
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

		private bool IsCustomPrimitive(Type type)
		{
			// Обработка nullable-типов (int?, DateTime? и т.д.)
			type = Nullable.GetUnderlyingType(type) ?? type;
			return customPrimitives.Contains(type);
		}

		private bool IsSimpleType(Type type)
		{
			// Обработка nullable-типов (int?, DateTime? и т.д.)
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnum) return true;
			TypeCode code = Type.GetTypeCode(type);
			// Считаем простыми все типы, кроме Object
			if (code != TypeCode.Object) return true;
			// Проверяем, не является ли тип известным кастомным
			return IsCustomPrimitive(type);
		}

		public bool IsDeclaredInMonoBehaviour(FieldInfo field)
		{
			Type type = field.DeclaringType;
			return type != null && type == typeof(MonoBehaviour);
		}

		public void DebugModel()
		{
			// Используем стандартный механизм отладки EF Core для получения "длинного" представления
			string modelDebugView = Model.ToDebugString(MetadataDebugStringOptions.ShortDefault);

			// Можно сохранить это в файл или просто вывести в консоль для быстрого анализа
			File.WriteAllText("model_debug.txt", modelDebugView);
		}


		#region Serialize

		// Вспомогательные методы (содержат несколько операторов, но это обычные методы, не лямбды)
		private static LVector3 ParseVector3(string s)
		{
			string[] p = s.Split(';');
			return new LVector3 { x = float.Parse(p[0]), y = float.Parse(p[1]), z = float.Parse(p[2]) };
		}

		private static LVector2 ParseVector2(string s)
		{
			string[] p = s.Split(';');
			return new LVector2 { x = float.Parse(p[0]), y = float.Parse(p[1]) };
		}


		private static readonly ValueConverter<LVector3, string> Vector3_Converter =
			new ValueConverter<LVector3, string>(v => $"{v.x};{v.y};{v.z}", s => ParseVector3(s));

		private static readonly ValueConverter<LVector2, string> Vector2_Converter =
			new ValueConverter<LVector2, string>(v => $"{v.x};{v.y}", s => ParseVector2(s));

		private ValueConverter GetValueConverter(Type type)
		{
			if (type == typeof(LVector3)) return Vector3_Converter;
			if (type == typeof(LVector2)) return Vector2_Converter;
			throw new NotSupportedException($"No converter for type {type}");
		}

		#endregion

	}
}
