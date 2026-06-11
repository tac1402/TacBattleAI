// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using System;
using System.Collections.Generic;

namespace UnityEF
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class MappedAttribute : Attribute
	{
	}
}
