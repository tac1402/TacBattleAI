// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using Tac.DConvert;
using UnityEngine;

namespace Tac
{
	/// <summary>
	/// Универсальная сущность от Tac (дополняется реализацией IPrefabId и IDataSave для сохранения)
	/// </summary>
	public abstract partial class Item : IPrefabId
	{
		public int Id { get { return ObjectId; } set { ObjectId = value; } }
		public string PrefabName { get { return ModelName; } set { ModelName = value; } }

		public Transform Transform
		{
			get { return transform; }
			set
			{
				transform.position = value.position;
				transform.localEulerAngles = value.localEulerAngles;
				transform.localScale = value.localScale;
			}
		}
		public bool IsActive
		{
			get { return gameObject.activeSelf; }
			set { gameObject.SetActive(value); }
		}
	}
}
