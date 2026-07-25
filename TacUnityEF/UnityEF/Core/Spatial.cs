// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev


using DnaCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using UnityEngine;

namespace Tac
{
	/// <summary>
	/// Пространственная сущность в сцене
	/// </summary>
	public abstract class Spatial : Flow, ISpatialDb
	{
		private Vector3_ fakePosition;
		public Vector3_ Position
		{
			get { if (IsValidUnityComponent) { return transform.position.To_(); } else { return fakePosition; } }
			set { if (IsValidUnityComponent) { transform.position = value.To(); } else { fakePosition = value; } }
		}
		private Vector3_ fakeRotation;
		public Vector3_ Rotation
		{
			get { if (IsValidUnityComponent) { return transform.localEulerAngles.To_(); } else { return fakeRotation; } }
			set { if (IsValidUnityComponent) { transform.localEulerAngles = value.To(); } else { fakeRotation = value; } }
		}
		private Vector3_ fakeScale;
		public Vector3_ Scale
		{
			get { if (IsValidUnityComponent) { return transform.localScale.To_(); } else { return fakeScale; } }
			set { if (IsValidUnityComponent) { transform.localScale = value.To(); } else { fakeScale = value; } }
		}
	}
}
