// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using UnityEngine.UI;

namespace Tac
{
	public abstract partial class DayNight0 : IDayNight
	{
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущие время
		/// </summary>
		public Text gameTime;
		public string GameTime
		{
			get
			{
				if (gameTime != null) { return gameTime.text; } else { return string.Empty; }
			}
			set
			{
				if (gameTime != null) gameTime.text = value;
			}
		}

		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущий номер суток
		/// </summary>
		public Text gameDays;
		public string GameDays
		{
			get
			{
				if (gameDays != null) { return gameDays.text; } else { return string.Empty; }
			}
			set
			{
				if (gameDays != null) { gameDays.text = value; }
			}
		}
	}
}
