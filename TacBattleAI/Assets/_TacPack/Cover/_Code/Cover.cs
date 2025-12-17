using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tac.ItemMove;

namespace Tac
{
	public class Cover : Item2
	{
		public override void SetTurn(TurnType argRotateIndex)
		{
			GameObject rotatePivot = Pivot;
			if (rotatePivot == null)
			{
				rotatePivot = gameObject;
			}

			float locAngle = 0;
			switch (argRotateIndex)
			{
				case TurnType.Angle0:
					locAngle = 0;
					break;
				case TurnType.Angle45:
					locAngle = 45;
					break;
				case TurnType.Angle90:
					locAngle = 90;
					break;
				case TurnType.Angle135:
					locAngle = 135;
					break;
				case TurnType.Angle180:
					locAngle = 180;
					break;
				case TurnType.Angle225:
					locAngle = 225;
					break;
				case TurnType.Angle270:
					locAngle = 270;
					break;
				case TurnType.Angle315:
					locAngle = 315;
					break;
			}
			rotatePivot.transform.localEulerAngles = new Vector3(rotatePivot.transform.localEulerAngles.x, locAngle, rotatePivot.transform.localEulerAngles.z);

			base.SetTurn(argRotateIndex);
		}
	}
}
