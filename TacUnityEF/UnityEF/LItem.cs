// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEF
{
	public class LItem<T> : ItemDb where T : class, IItemDb
	{
		public T Item;

		public LItem() { }

		public LItem(T argItem)
		{
			Item = argItem;
		}
	}
}
