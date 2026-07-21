// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using DnaCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UnityEngine;

namespace Tac
{
	/// <summary>
	/// Универсальная сущность от Tac 
	/// </summary>
	public abstract class Item : MonoBehaviour, IItemDb
	{
		/// <summary>
		/// Уникальный индентификатор объекта в мире
		/// </summary>
		public int Id { get { return itemDb.Id; } set { itemDb.Id = value;  } }

		[NotMapped]
		public Vector3_ Position
		{
			get { if (IsValidUnityComponent) { return transform.position.To_(); } else { return Vector3_.zero; } }
			set { if (IsValidUnityComponent) { transform.position = value.To(); } }
		}
		[NotMapped]
		public Vector3_ Rotation
		{
			get { if (IsValidUnityComponent) { return transform.localEulerAngles.To_(); } else { return Vector3_.zero; } }
			set { if (IsValidUnityComponent) { transform.localEulerAngles = value.To(); } }
		}
		[NotMapped]
		public Vector3_ Scale
		{
			get { if (IsValidUnityComponent) { return transform.localScale.To_(); } else { return Vector3_.zero; } }
			set { if (IsValidUnityComponent) { transform.localScale = value.To(); } }
		}

		public bool IsValidUnityComponent
		{
			get
			{
				try
				{
					// Попытка обратиться к transform — у валидного компонента это работает,
					// у созданного через new — выбрасывается исключение (или возвращается null).
					var t = transform;
					return t != null; // Если transform вернул null — тоже считаем невалидным
				}
				catch
				{
					// Если выброшено исключение — объект невалидный
					return false;
				}
			}
		}


		/// <summary>
		/// Группа объекта
		/// </summary>
		public int GroupId = -1;
		/// <summary>
		/// Имя префаба
		/// </summary>
		public string ModelName = "";

		private ItemDb itemDb = new ItemDb();

		public ItemDb item { get { return itemDb; } }


		private bool initData = false;
		private void Awake()
		{
			if (initData == false) { InitData(); }
		}

		public virtual void InitData() { initData = true; }

		[NotMapped]
		public bool RecoverMode
		{
			get { if (Id == 0) { return false; } else { return true; } }
		}
		

		public static readonly RegCollection Reg = new RegCollection();

		public static void Register<T>(IAdd<T> iadd) where T : Item
			=> Reg.Register(iadd);

	}

	public class RegCollection
	{
		private readonly Dictionary<string, Func<object, object>> map = new Dictionary<string, Func<object, object>>();

		public void Register<T>(IAdd<T> factory) where T : Item
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
