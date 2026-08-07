// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Text;

namespace Tac
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class TacLogicAttribute : Attribute
	{
	}
}
