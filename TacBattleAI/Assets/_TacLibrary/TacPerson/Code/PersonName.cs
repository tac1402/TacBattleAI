// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using UnityEngine;

namespace Tac.Person
{
	public partial class PersonName
	{
		public void LoadName(System.Random argRnd)
		{
			TextAsset locFileMen = Resources.Load("Data\\Person\\Men") as TextAsset;
			TextAsset locFileWomen = Resources.Load("Data\\Person\\Women") as TextAsset;
			LoadName(argRnd, locFileMen.text, locFileWomen.text);
		}
	}
}
