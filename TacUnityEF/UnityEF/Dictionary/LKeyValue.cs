// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using DnaCore;
using System;
using System.Collections.Generic;

namespace UnityEF
{
	/// <summary>
	/// Элемент словаря: связывает ключ со значением и принадлежит одному DDictionary.
	/// </summary>
	public class LKeyValue<K, V> : ItemDb
	{
		public V Value;
		public K Key;
		public int LDictionaryId;
	}
}
