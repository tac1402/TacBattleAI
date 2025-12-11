using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tac.ItemMove
{
	[Serializable]
	public class TurnInfo
	{
		public List<TurnType> TurnSeq;
		public List<Vector2> ViewCorrect;
		public Vector2 TurnSize = Vector2.one;
		public TurnType CurrentTurn;

		/// <summary>
		/// Повернуть конструкцию
		/// </summary>
		public TurnType Turn()
		{
			TurnType retIndex = TurnType.None;
			for (int i = 0; i < TurnSeq.Count; i++)
			{
				if (TurnSeq[i] == CurrentTurn)
				{
					if (i == TurnSeq.Count - 1)
					{
						retIndex = TurnSeq[0];
					}
					else
					{
						retIndex = TurnSeq[i + 1];
					}
					break;
				}
			}
			return retIndex;
		}
	}
}
