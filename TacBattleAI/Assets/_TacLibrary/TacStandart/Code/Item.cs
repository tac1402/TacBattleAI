// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using UnityEngine;

namespace Tac
{
	/// <summary>
	/// Универсальная сущность второго поколения от Tac 
	/// </summary>
    public abstract class Item : MonoBehaviour
    {
		/// <summary>
		/// Уникальный индентификатор объекта в мире
		/// </summary>
		public int ObjectId;
		/// <summary>
		/// Группа объекта
		/// </summary>
		public int GroupId = -1;
		/// <summary>
		/// Имя префаба
		/// </summary>
		public string ModelName = "";
		/// <summary>
		/// Тип модели
		/// </summary>
		public ModelTypes ModelType = ModelTypes.Model;
	}
}
