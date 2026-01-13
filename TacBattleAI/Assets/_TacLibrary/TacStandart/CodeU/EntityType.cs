// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tac.ItemCreate
{
	[Serializable]
	public class EntityType
	{
		public string Name;
		public int Id;
		public GameObject Level;

		public static EntityType Get(List<EntityType> argList, int argId) => argList.Find(list => list.Id == argId);
	}
}
