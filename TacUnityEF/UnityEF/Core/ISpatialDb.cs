// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tac
{
	public interface ISpatialDb : IItemDb
	{
		Vector3_ Position { get; set; }
		Vector3_ Rotation { get; set; }
		Vector3_ Scale { get; set; }
	}
}
