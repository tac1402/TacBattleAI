// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using System;
using System.Collections.Generic;
using Tac;
using DnaCore;
using System.Numerics;

namespace UnityEF
{
	public class LVector3 : Vector3_, IItemDb
	{
		public int Id { get { return itemDb.Id; } set { itemDb.Id = value; } }

		private ItemDb itemDb = new ItemDb();

		public ItemDb item { get { return itemDb; } }

		public LVector3() { }
		public LVector3(UnityEngine.Vector3 v) : base (v.x, v.y, v.z) { }
		public LVector3(Vector3_ v) : base(v.x, v.y, v.z) { }
	}

	public class LVector2 : Vector2_, IItemDb
	{
		public int Id { get { return itemDb.Id; } set { itemDb.Id = value; } }

		private ItemDb itemDb = new ItemDb();

		public ItemDb item { get { return itemDb; } }
		public LVector2() { }
		public LVector2(UnityEngine.Vector2 v) : base(v.x, v.y) { }
		public LVector2(Vector2_ v) : base(v.x, v.y) { }
	}

	public class LGameTime : GameTime, IItemDb
	{
		public int Id { get { return itemDb.Id; } set { itemDb.Id = value; } }

		private ItemDb itemDb = new ItemDb();

		public ItemDb item { get { return itemDb; } }
		public LGameTime() { }
		public LGameTime(GameTime gt) : base(gt.Day, gt.Hour) { }
	}


}
