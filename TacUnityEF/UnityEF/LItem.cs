// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEF
{
	public class LItem<T> : ItemDb
	{
		public T Item;

		public LItem() { }

		public LItem(T argItem)
		{
			Item = argItem;
		}

		public static implicit operator LItem<T>(T value)
		{
			return new LItem<T>(value);
		}

		public static implicit operator T(LItem<T> item)
		{
			return item.Item;
		}
	}
}
