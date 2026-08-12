// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using UnityEF;
using System;

namespace Tac
{
	public interface ISpatialDb : IItemDb
	{
		Vector3_ Position { get; set; }
		Vector3_ Rotation { get; set; }
		Vector3_ Scale { get; set; }
	}
}
