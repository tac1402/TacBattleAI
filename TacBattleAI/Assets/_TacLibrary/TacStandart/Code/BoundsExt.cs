// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tac
{
    public class BoundsExt
    {
		private Bounds bounds;
		private float height;

		public BoundsExt() { }
		public BoundsExt(Bounds argBounds, float argHeight = 0) 
		{ 
			bounds = argBounds; 
			height = argHeight;
			AllBoundsPoint = new List<Vector3>() { backLeft, backRight, frontLeft, frontRight };
		}

		public static Bounds Empty { get { return new Bounds(); } }
		public static List<Bounds> EmptyList { get { return null; } }

		public Vector3 backLeft { get { return new Vector3(bounds.center.x + bounds.extents.x, height, bounds.center.z + bounds.extents.z); } }
		public Vector3 backRight { get { return new Vector3(bounds.center.x + bounds.extents.x, height, bounds.center.z - bounds.extents.z); } }
		public Vector3 frontLeft { get { return new Vector3(bounds.center.x - bounds.extents.x, height, bounds.center.z + bounds.extents.z); } }
		public Vector3 frontRight { get { return new Vector3(bounds.center.x - bounds.extents.x, height, bounds.center.z - bounds.extents.z); } }

		public List<Vector3> AllBoundsPoint;
	}

	public static class BoundsHelper
	{
		public static bool IsEmpty(this Bounds b)
		{
			return b.center == Vector3.zero && b.size == Vector3.zero;
		}
		public static bool IsEmpty(this List<Bounds> b)
		{
			return b == null;
		}
	}

}

