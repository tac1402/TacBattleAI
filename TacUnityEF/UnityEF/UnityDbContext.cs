// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Tac;
using UnityEngine;


namespace UnityEF
{
    public class UnityDbContext : DbContext
	{
		private readonly StreamWriter logWriter = new StreamWriter(@"EF_log.txt", append: true) { AutoFlush = true };


		private static HashSet<Type> customPrimitives = new HashSet<Type>
		{
			typeof(UnityEngine.Vector3),
			typeof(UnityEngine.Vector2),
			typeof(Vector3_),
			typeof(Vector2_),
			typeof(LVector3),
			typeof(LVector2),
			typeof(GameTime),
			typeof(LGameTime)
		};

		private DebugType debugType;
		private bool loadMode;

		public UnityDbContext()
		{ 
			debugType = DebugType.None;
			loadMode = false;

			var dummy = typeof(TacCompiler.Compiler); // фейковое использование
		}

		public UnityDbContext(DebugType argDebugType, bool argLoadType)
		{
			debugType = argDebugType;
			loadMode = argLoadType;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			SqlTraceInterceptor interceptor = new SqlTraceInterceptor();
			interceptor.LoadMode = loadMode;

			optionsBuilder.UseSqlite("Data Source=rotark.db;Foreign Keys=False;").AddInterceptors(interceptor); ;

			//optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Rotark;Trusted_Connection=True;");


			switch (debugType)
			{ 
				case DebugType.InfoSql:
					optionsBuilder.EnableSensitiveDataLogging().LogTo(logWriter.WriteLine, LogLevel.Information);
					break;
				case DebugType.Trace:
					optionsBuilder.EnableSensitiveDataLogging().LogTo(logWriter.WriteLine, LogLevel.Trace);
					break;
			}
		}

		private readonly List<Type> allTypes = new List<Type>();

		public void AddTypes(string assemblyName)
		{
			Type[] types = Assembly.Load(assemblyName).GetTypes();

			// Базовые сущности: все неабстрактные классы в вашем пространстве имён, реализующие IItemDb
			var baseTypes = types.Where(t => t.IsClass && !t.IsAbstract && typeof(IItemDb).IsAssignableFrom(t));

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
					Type ft = field.FieldType;
					if (ft.IsGenericType)
					{
						Type gt = ft.GetGenericTypeDefinition();
						if (gt == typeof(LList<>) || gt == typeof(LList_<>))
						{
							toAdd.Add(ft); // LList<T>
							var dItemType = typeof(LItem<>).MakeGenericType(ft.GetGenericArguments()[0]);
							toAdd.Add(dItemType); // LItem<T>
						}
						if (gt == typeof(LQueue<>) || gt == typeof(LQueue_<>))
						{
							toAdd.Add(ft); // LQueue<T>
							var dItemType = typeof(LItem<>).MakeGenericType(ft.GetGenericArguments()[0]);
							toAdd.Add(dItemType); // LItem<T>
						}
						if (gt == typeof(LDictionary<,>) || gt == typeof(LDictionary_<,>))
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
			}
			foreach (var t in toAdd) allTypes.Add(t);

			//allTypes.Add(typeof(Logic));
			//allTypes.Add(typeof(Flow));
			//allTypes.Add(typeof(Spatial));

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
					var gt = type.GetGenericTypeDefinition();
					if (gt == typeof(LList<>) || gt == typeof(LList_<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LList_{argName}");
					}
					else if (gt == typeof(LQueue<>) || gt == typeof(LQueue_<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LQueue_{argName}");
					}
					else if (gt == typeof(LItem<>))
					{
						var argName = type.GetGenericArguments()[0].Name;
						entityBuilder.ToTable($"LItem_{argName}");
					}
					else if (gt == typeof(LDictionary<,>) || gt == typeof(LDictionary_<,>))
					{
						var keyArg = type.GetGenericArguments()[0];
						var valueArg = type.GetGenericArguments()[1];

						// Если значение — LItem<T>, извлекаем T
						string valueName;
						if (valueArg.IsGenericType && valueArg.GetGenericTypeDefinition() == typeof(LItem<>))
						{
							var inner = valueArg.GetGenericArguments()[0];
							valueName = $"LItem_{inner.Name}";
						}
						else
						{
							valueName = valueArg.Name;
						}
						entityBuilder.ToTable($"LDictionary_{keyArg.Name}_{valueName}");
					}
					else if (gt == typeof(LKeyValue<,>))
					{
						var keyArg = type.GetGenericArguments()[0];
						var valueArg = type.GetGenericArguments()[1];
						string valueName;
						if (valueArg.IsGenericType && valueArg.GetGenericTypeDefinition() == typeof(LItem<>))
						{
							var inner = valueArg.GetGenericArguments()[0];
							valueName = $"LItem_{inner.Name}";
						}
						else
						{
							valueName = valueArg.Name;
						}
						entityBuilder.ToTable($"LKeyValue_{keyArg.Name}_{valueName}");
					}
				}
				/*else
				{
					// Для обычных классов задаём имя таблицы по имени класса
					entityBuilder.ToTable(type.Name);
				}*/

				/*var entityTypes = modelBuilder.Model.GetEntityTypes();
				foreach (var et in entityTypes)
				{
					if (et.ClrType == typeof(Vector3_))
					{
						Console.WriteLine("Vector3_ зарегистрирован как сущность!");
					}
				}*/
			}

			// 3. Теперь для каждой зарегистрированной сущности настраиваем поля и связи
			foreach (Type type in allTypes)
			{
				var entityBuilder = modelBuilder.Entity(type);

				bool isLList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LList<>);
				bool isLList_ = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LList_<>);
				bool isLQueue = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LQueue<>);
				bool isLQueue_ = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LQueue_<>);
				bool isLItem = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LItem<>);
				bool isLDictionary = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LDictionary<,>);
				bool isLDictionary_ = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LDictionary_<,>);
				bool isLKeyValue = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(LKeyValue<,>);

				if (isLDictionary || isLDictionary_)
				{
					entityBuilder.Ignore("Values");

					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LDictionaryId");
				}
				else if (isLKeyValue)
				{
					var keyType = type.GetGenericArguments()[0];
					var valueType = type.GetGenericArguments()[1];

					if (IsSimpleType(valueType))
					{
						var prop = entityBuilder.Property(valueType, "Value").HasField("Value");
						if (IsCustomPrimitive(valueType))
						{
							prop.HasConversion(GetValueConverter(valueType));
						}
					}
					else
					{
						// Иначе – связь с сущностью
						entityBuilder.HasOne("Value")
							.WithMany()
							.HasForeignKey("ValueId");
					}

					entityBuilder.Property("Id").ValueGeneratedOnAdd();

					entityBuilder.HasIndex("LDictionaryId", "Key").IsUnique();
				}
				else if (isLList || isLList_)
				{
					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LListId");
				}
				else if (isLQueue || isLQueue_)
				{
					entityBuilder.HasMany("Items")
						.WithOne()
						.HasForeignKey("LQueueId");
				}
				else if (isLItem)
				{
					var itemType = type.GetGenericArguments()[0]; // T в LItem<T>
					if (IsSimpleType(itemType))
					{
						var prop = entityBuilder.Property(itemType, "Item").HasField("Item");
						if (IsCustomPrimitive(itemType))
						{
							prop.HasConversion(GetValueConverter(itemType));
						}
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
					var logicProps = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance)
										 .Where(p => p.Name.StartsWith("logic"))
										 .ToList();
					if (logicProps.Count > 1)
					{
						throw new InvalidOperationException($"Multiple properties starting with 'logic' found in type {type.FullName}.");
					}
					else if (logicProps.Count == 1)
					{
						var logicProp = logicProps[0];
						var logicPropType = logicProp.PropertyType;

						bool isEntity = modelBuilder.Model.FindEntityType(logicPropType) != null;
						if (isEntity)
						{
							var existingNavigation = entityBuilder.Metadata.FindNavigation(logicProp.Name);
							if (existingNavigation == null)
							{
								// Это навигационное свойство – настраиваем связь
								entityBuilder.HasOne(logicPropType, logicProp.Name)
										 .WithMany();
							}
						}
					}

					///////////////////////////////////////////////////////////////////////////////////////////////////
					var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
						.Where(f => (f.IsPublic || f.IsDefined(typeof(MappedAttribute), false))
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

						if (isEntity && isSimple == false)
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

							// Добавляем для bool
							if (fieldType == typeof(bool))
							{
								propertyBuilder.HasConversion<int>();
							}
						}
					}

					if (typeof(ISpatialDb).IsAssignableFrom(type))
					{
						entityBuilder.Property<Vector3_>("Position")
							.HasConversion(GetValueConverter(typeof(Vector3_)));
						entityBuilder.Property<Vector3_>("Rotation")
							.HasConversion(GetValueConverter(typeof(Vector3_)));
						entityBuilder.Property<Vector3_>("Scale")
							.HasConversion(GetValueConverter(typeof(Vector3_)));
					}

					bool isRoot = (type.BaseType == null) || allTypes.Contains(type.BaseType) == false;

					BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
					if (isRoot == false)
					{
						flags |= BindingFlags.DeclaredOnly;
					}

					List<PropertyInfo> properties = type.GetProperties(flags).ToList();

					foreach (var prop in properties)
					{
						if (prop.Name == "Id") { continue; }
						if ((typeof(ISpatialDb).IsAssignableFrom(type)) &&
							(prop.Name == "Position" || prop.Name == "Rotation" || prop.Name == "Scale"))
						{
							continue;
						}

						if (prop.IsDefined(typeof(MappedAttribute), false))
						{
							// Настраиваем свойство как маппируемое
							var propertyBuilder = entityBuilder.Property(prop.PropertyType, prop.Name);
							if (IsCustomPrimitive(prop.PropertyType))
							{
								propertyBuilder.HasConversion(GetValueConverter(prop.PropertyType));
							}
							// Явно указываем, что нужно использовать свойство, а не поле
							propertyBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
						}
						else
						{
							// Все остальные свойства игнорируем
							entityBuilder.Ignore(prop.Name);
						}
					}
				}
			}
		}

		public static bool IsCustomPrimitive(Type type)
		{
			// Обработка nullable-типов (int?, DateTime? и т.д.)
			type = Nullable.GetUnderlyingType(type) ?? type;
			return customPrimitives.Contains(type);
		}

		public static bool IsSimpleType(Type type)
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
			if (debugType == DebugType.OnlyShema)
			{
				// Используем стандартный механизм отладки EF Core для получения "длинного" представления
				string modelDebugView = Model.ToDebugString(MetadataDebugStringOptions.ShortDefault); // 5.0.17

				// Можно сохранить это в файл или просто вывести в консоль для быстрого анализа
				File.WriteAllText("model_debug.txt", modelDebugView);
			}
		}


		#region Serialize

		// Вспомогательные методы (содержат несколько операторов, но это обычные методы, не лямбды)
		private static Vector3 ParseVector3(string s)
		{
			string[] p = s.Split(';');
			return new Vector3 {x = float.Parse(p[0], CultureInfo.InvariantCulture), y = float.Parse(p[1], CultureInfo.InvariantCulture), z = float.Parse(p[2], CultureInfo.InvariantCulture) };
		}
		private static Vector2 ParseVector2(string s)
		{
			string[] p = s.Split(';');
			return new Vector2 {x = float.Parse(p[0], CultureInfo.InvariantCulture), y = float.Parse(p[1], CultureInfo.InvariantCulture) };
		}
		private static Vector3_ ParseVector3_(string s)
		{
			string[] p = s.Split(';');
			return new Vector3_ {x = float.Parse(p[0], CultureInfo.InvariantCulture), y = float.Parse(p[1], CultureInfo.InvariantCulture), z = float.Parse(p[2], CultureInfo.InvariantCulture) };
		}
		private static Vector2_ ParseVector2_(string s)
		{
			string[] p = s.Split(';');
			return new Vector2_ { x = float.Parse(p[0], CultureInfo.InvariantCulture), y = float.Parse(p[1], CultureInfo.InvariantCulture) };
		}
		private static LVector3 ParseLVector3(string s)
		{
			string[] p = s.Split(';');
			return new LVector3 {x = float.Parse(p[0], CultureInfo.InvariantCulture), y = float.Parse(p[1], CultureInfo.InvariantCulture), z = float.Parse(p[2], CultureInfo.InvariantCulture) };
		}
		private static LVector2 ParseLVector2(string s)
		{
			string[] p = s.Split(';');
			return new LVector2 {x = float.Parse(p[0], CultureInfo.InvariantCulture), y = float.Parse(p[1], CultureInfo.InvariantCulture) };
		}

		private static GameTime ParseGameTime(string s)
		{
			string[] p = s.Split(';');
			return new GameTime { Day = int.Parse(p[0]), Hour = int.Parse(p[1]) };
		}
		private static LGameTime ParseLGameTime(string s)
		{
			string[] p = s.Split(';');
			return new LGameTime { Day = int.Parse(p[0]), Hour = int.Parse(p[1]) };
		}



		private static readonly ValueConverter<Vector3, string> Vector3Converter =
			new ValueConverter<Vector3, string>(v => $"{v.x};{v.y};{v.z}", s => ParseVector3(s));

		private static readonly ValueConverter<Vector2, string> Vector2Converter =
			new ValueConverter<Vector2, string>(v => $"{v.x};{v.y}", s => ParseVector2(s));

		private static readonly ValueConverter<Vector3_, string> Vector3_Converter =
			new ValueConverter<Vector3_, string>(v => $"{v.x};{v.y};{v.z}", s => ParseVector3_(s));

		private static readonly ValueConverter<Vector2_, string> Vector2_Converter =
			new ValueConverter<Vector2_, string>(v => $"{v.x};{v.y}", s => ParseVector2_(s));

		private static readonly ValueConverter<LVector3, string> LVector3Converter =
			new ValueConverter<LVector3, string>(v => $"{v.x};{v.y};{v.z}", s => ParseLVector3(s));

		private static readonly ValueConverter<LVector2, string> LVector2Converter =
			new ValueConverter<LVector2, string>(v => $"{v.x};{v.y}", s => ParseLVector2(s));

		private static readonly ValueConverter<GameTime, string> GameTimeConverter =
			new ValueConverter<GameTime, string>(v => $"{v.Day};{v.Hour}", s => ParseGameTime(s));
		private static readonly ValueConverter<LGameTime, string> LGameTimeConverter =
			new ValueConverter<LGameTime, string>(v => $"{v.Day};{v.Hour}", s => ParseLGameTime(s));


		private ValueConverter GetValueConverter(Type type)
		{
			if (type == typeof(Vector3)) return Vector3Converter;
			if (type == typeof(Vector3_)) return Vector3_Converter;
			if (type == typeof(LVector3)) return LVector3Converter;

			if (type == typeof(Vector2)) return Vector2Converter;
			if (type == typeof(Vector2_)) return Vector2_Converter;
			if (type == typeof(LVector2)) return LVector2Converter;

			if (type == typeof(GameTime)) return GameTimeConverter;
			if (type == typeof(LGameTime)) return LGameTimeConverter;

			throw new NotSupportedException($"No converter for type {type}");
		}

		#endregion

	}


	public class LItemConverter<T> : ValueConverter<LItem<T>, T>
	{
		public LItemConverter(): base(
				item => item.Item,                 // LItem<T> → T
				value => new LItem<T>(value)       // T → LItem<T>
			)
		{ }
	}
}
