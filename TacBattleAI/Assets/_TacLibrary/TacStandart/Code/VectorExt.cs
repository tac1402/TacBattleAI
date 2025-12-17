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
		public static Vector2 To2(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}
		public static Vector3 To3(this Vector2 v)
		{
			return new Vector3(v.x, 0, v.y);
		}
		public static Vector3 SetX(this Vector3 v, float argX)
		{
			return new Vector3(argX, v.y, v.z);
		}
		public static Vector3 SetY(this Vector3 v, float argY)
		{
			return new Vector3(v.x, argY, v.z);
		}
		public static Vector3 SetZ(this Vector3 v, float argZ)
		{
			return new Vector3(v.x, v.y, argZ);
		}
	}
}
