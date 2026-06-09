// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace DnaCore
{
	public interface ICell
	{
		public Cell cell { get; }
	}

	public abstract class Cell : ICell
	{
		public Cell cell { get { return this; } }

		// Локальная копия загруженной сборки (геном)
		internal Assembly genomeAssembly;

		internal string role;
		public string Role
		{
			get { return role; }
		}

		public static T Create<T, C>(T obj, C coll, Expression<Func<C>> lambda) where T : ICell where C : class
		{
			string role = Cell<T>.GetRole(lambda);
			Cell<T>.RegisterCollection(role, coll);
			return Create<T>(obj, role);
		}

		public static T Create<T>(T obj, string dllName, string commonRole = "") where T : ICell
		{
			return Cell<T>.Create(obj, dllName, commonRole);
		}

		protected T Restore<T>(string restoreRole) where T : Cell
		{
			if (Cell<T>.cells.ContainsKey(restoreRole))
			{
				Cell existingCell = Cell<T>.cells[restoreRole].cell;
				if (existingCell is T typedCell)
					return typedCell;
			}

			// Создаём нового соседа через геном
			Console.WriteLine($"[{GetType().Name}] {typeof(T).Name} (роль '{role}') восстанавливаем. ");
			T newCell = (T)genomeAssembly.CreateInstance(typeof(T).FullName, true);

			return newCell;
		}

		// Метод для восстановления коллекции соседей (если нужно – пересоздаёт всех)
		protected List<T> RestoreList<T>(List<T> c, string restoreRole) where T : Cell
		{
			return Cell<T>.GetCollection<List<T>>(restoreRole);
		}
		protected Queue<T> RestoreQueue<T>(Queue<T> c, string restoreRole) where T : Cell
		{
			return Cell<T>.GetCollection<Queue<T>>(restoreRole);
		}
		protected Dictionary<TKey, T> RestoreDictionary<TKey, T>(Dictionary<TKey, T> dict, string restoreRole)
			where T : Cell
		{
			return Cell<T>.GetCollection<Dictionary<TKey, T>>(restoreRole);
		}
		protected T[] RestoreArray<T>(T[] array, string restoreRole) where T : Cell
		{
			return Cell<T>.GetCollection<T[]>(restoreRole);
		}

	}


	public class Cell<T> where T : ICell
	{
		// Локальная копия загруженной сборки (геном)
		private static Assembly genomeAssembly;

		internal static readonly Dictionary<string, ICell> cells = new Dictionary<string, ICell>();

		internal static Dictionary<string, object> collections = new Dictionary<string, object>();

		public T Result;

		public Cell(T obj, string dllName, string role)
		{
			Result = Cell<T>.Create(obj, dllName, role);
		}

		public static void RegisterCollection(string role, object collection)
		{
			if (collections.ContainsKey(role) == false)
			{
				collections[role] = collection;
			}
		}
		public static C GetCollection<C>(string role) where C : class
		{
			return collections[role] as C;
		}

		private static T CreateInstance(string name)
		{ 
			return (T)genomeAssembly.CreateInstance(name, true);
		}

		public static T Create(T obj, string dllName, string role = "")
		{
			return Create(obj, dllName, role, CreateInstance);
		}


		public static T Create(T obj, string dllName, string role = "", Func<string, T> funcCreate = null)
		{
			if (genomeAssembly == null)
			{
				//genomeAssembly = Assembly.LoadFrom(dllName);
				genomeAssembly = Assembly.GetAssembly(obj.GetType());
				Console.WriteLine($"[{typeof(T).FullName}] Загружен геном из {dllName}");
			}

			string name = typeof(T).FullName;
			T cell = default(T);
			if (name != null)
			{
				if (obj == null)
				{
					cell = funcCreate(name);
				}
				else
				{
					cell = obj;
				}
				if (cell != null)
				{
					cell.cell.genomeAssembly = genomeAssembly;
					if (role != "")
					{
						cell.cell.role = role;

						if (collections.ContainsKey(cell.cell.role))
						{
							// Коллективная роль уже зарегистрированна, на будущие тут можно анализировать её члены
						}
						else if (cells.ContainsKey(cell.cell.role))
						{
							// Одиночная роль уже создана
						}
						else
						{
							cells[cell.cell.role] = cell;
						}
					}
					Console.WriteLine($"Создан [{typeof(T).FullName}]");
				}
			}

			return cell;
		}

		internal static string GetRole<T>(Expression<Func<T>> expr)
		{
			MemberExpression me = expr.Body as MemberExpression;
			string propertyName = me.Member.Name;
			return propertyName;
		}
	}
}
