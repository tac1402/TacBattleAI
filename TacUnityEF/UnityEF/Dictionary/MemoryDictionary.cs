// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEF
{
	internal class MemoryDictionary<K, V> : MemoryDictionary_<K, V> where V : class, IItemDb
	{ }
}
