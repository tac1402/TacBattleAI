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
	/// Потоко-управляемая сущность в сцене
	/// </summary>
	public abstract class Flow : MonoBehaviour, IItemDb
	{
		/// <summary>
		/// Уникальный индентификатор объекта в мире
		/// </summary>
		public int Id { get { return itemDb.Id; } set { itemDb.Id = value;  } }

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
			get { return ItemDb.RecoverMode; }
		}


		public static readonly RegCollection Reg = new RegCollection();

		public static void Register<T>(IAdd<T> iadd) where T : IItemDb
			=> Reg.Register(iadd);

	}


}
