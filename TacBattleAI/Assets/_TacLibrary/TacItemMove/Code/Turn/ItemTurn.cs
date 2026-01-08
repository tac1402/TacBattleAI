// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using Tac.Camera;
using Tac.ItemMove;
using UnityEngine;

namespace Tac.ItemMove
{
	[Component(typeof(TopCamera))]
	public class ItemTurn : MonoBehaviour
	{
		public KeyCode RotateKey = KeyCode.Mouse1;

		private BuildItem selectedItem;


		void Update()
		{
			if (Input.GetKeyDown(RotateKey))
			{
				if (selectedItem != null && selectedItem.AllowTurn)
				{
					selectedItem.TurnNext();
				}
			}
		}

		public void OnItemTap(BuildItem argItem)
		{
			selectedItem = argItem;
		}
	}
}

namespace Tac
{
	public partial class BuildItem : Item
	{
		public GameObject Pivot;
		public GameObject View;
		public TurnInfo Turn;
		public bool DefaultTurn = true;
		public bool AllowTurn = true;

		public void TurnNext()
		{
			SetTurn(Turn.Turn());
		}

		public void SetTurn()
		{
			SetTurn(Turn.CurrentTurn);
		}

		public virtual void SetTurn(TurnType argRotateIndex)
		{
			if (DefaultTurn == true)
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
					case TurnType.Angle90:
						locAngle = 90;
						break;
					case TurnType.Angle180:
						locAngle = 180;
						break;
					case TurnType.Angle270:
						locAngle = 270;
						break;
				}
				rotatePivot.transform.localEulerAngles = new Vector3(rotatePivot.transform.localEulerAngles.x, locAngle, rotatePivot.transform.localEulerAngles.z);

				if (View != null)
				{
					int tIndex = 0;
					switch (argRotateIndex)
					{
						case TurnType.Angle0:
							tIndex = 0;
							break;
						case TurnType.Angle90:
							tIndex = 1;
							break;
						case TurnType.Angle180:
							tIndex = 2;
							break;
						case TurnType.Angle270:
							tIndex = 3;
							break;
					}
					View.transform.localPosition = new Vector3(Turn.ViewCorrect[tIndex].x, 0, Turn.ViewCorrect[tIndex].y);
				}
			}

			Turn.CurrentTurn = argRotateIndex;
		}
	}
}
