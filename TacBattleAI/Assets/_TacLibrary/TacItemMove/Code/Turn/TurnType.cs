using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tac.ItemMove
{

	public static class TurnType_
	{
		public static int GetAngleSign(TurnType argTurnType)
		{
			int d = 1;
			switch (argTurnType)
			{
				case TurnType.Angle0:
				case TurnType.Angle90:
				case TurnType.AngleRev0:
				case TurnType.AngleRev90:
					d = 1;
					break;
				case TurnType.Angle180:
				case TurnType.Angle270:
				case TurnType.AngleRev180:
				case TurnType.AngleRev270:
					d = -1;
					break;
			}
			return d;
		}

		public static Vector2 GetNextPosition(TurnType argTurnType)
		{
			Vector2 ret = Vector2.zero;
			switch (argTurnType)
			{
				case TurnType.Angle0:
				case TurnType.AngleRev0:
					ret = new Vector2(1, 0);
					break;
				case TurnType.Angle90:
				case TurnType.AngleRev90:
					ret = new Vector2(0, 1);
					break;
				case TurnType.Angle180:
				case TurnType.AngleRev180:
					ret = new Vector2(-1, 0);
					break;
				case TurnType.Angle270:
				case TurnType.AngleRev270:
					ret = new Vector2(0, -1);
					break;
			}
			return ret;
		}

		public static Vector2 GetPrevPosition(TurnType argTurnType)
		{
			Vector2 ret = Vector2.zero;
			switch (argTurnType)
			{
				case TurnType.Angle0:
				case TurnType.AngleRev0:
					ret = new Vector2(-1, 0);
					break;
				case TurnType.Angle90:
				case TurnType.AngleRev90:
					ret = new Vector2(0, -1);
					break;
				case TurnType.Angle180:
				case TurnType.AngleRev180:
					ret = new Vector2(1, 0);
					break;
				case TurnType.Angle270:
				case TurnType.AngleRev270:
					ret = new Vector2(0, 1);
					break;
			}
			return ret;
		}
	}

	[Serializable]
	public enum TurnType
	{
		None = 0,
		Angle0 = 1,
		Angle90 = 2,
		Angle180 = 3,
		Angle270 = 4,
		AngleRev0 = 5,
		AngleRev90 = 6,
		AngleRev180 = 7,
		AngleRev270 = 8,
		Angle45 = 9,
		Angle135 = 10,
		Angle225 = 11,
		Angle315 = 12
	}
}