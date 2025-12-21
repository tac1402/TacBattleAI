// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2024 Sergej Jakovlev

// You can use this code for educational purposes only;
// this code or its modifications cannot be used for commercial purposes
// or in proprietary libraries without permission from the author

using UnityEngine;

namespace Tac
{
	public static class VectorExt
	{
		public static Vector2 To2(this Vector3_ v)
		{
			return new Vector2(v.x, v.z);
		}
		public static Vector3 To(this Vector3_ v)
		{
			return new Vector3(v.x, v.y, v.z);
		}
		public static Vector3_ To_(this Vector3 v)
		{
			return new Vector3_(v.x, v.y, v.z);
		}

		public static Vector3_ From(this Vector3_ o, Vector3 v)
		{
			return new Vector3_(v.x, v.y, v.z);
		}

	}
}
