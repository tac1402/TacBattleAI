// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tac
{
	public class RegCollection
	{
		private readonly Dictionary<string, Func<object, object>> map = new Dictionary<string, Func<object, object>>();

		public void Register<T>(IAdd<T> factory) where T : IItemDb
		{
			string key = typeof(T).FullName;
			if (!map.ContainsKey(key))
			{
				// Создаём делегат
				Func<object, object> addDelegate = obj => factory.Add((T)obj);
				map.Add(key, addDelegate);
			}
		}

		/// <summary>
		/// Возвращает делегат для добавления объекта указанного типа.
		/// Если фабрика не зарегистрирована, возвращает null.
		/// </summary>
		public Func<object, object> GetAdd(Type type)
		{
			map.TryGetValue(type.FullName, out var del);
			return del;
		}
	}
}
