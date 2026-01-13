// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using System.Collections.Generic;
using UnityEngine;

namespace Tac
{
	public interface IObject
	{
		int ObjectIdCounter { get; set; }
		int PredeffinedObjectId { get; set; }
		
		List<EntityType> WorldLevel { get; }

		GameObject CreateObject(string argModelName);
		void DestroyObject(GameObject argObject);
	}
}
