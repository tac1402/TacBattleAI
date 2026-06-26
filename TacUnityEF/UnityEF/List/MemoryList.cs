// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;

namespace UnityEF
{
	/// <summary>
	/// Список в памяти
	/// </summary>
	internal class MemoryList<T> : MemoryList_<T> where T : class, IItemDb
	{ }
}
