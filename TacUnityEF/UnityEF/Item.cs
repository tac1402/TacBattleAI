// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using DnaCore;
using System.ComponentModel.DataAnnotations;
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


		private void Awake()
		{
			InitData();
		}

		public virtual void InitData() { }
	}
}
