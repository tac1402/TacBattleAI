// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

using Tac.DConvert;

namespace Tac
{
	/// <summary>
	/// Расширяется системой сохранения, реализуя IDayNight
	/// </summary>
	public partial class DayNight0 : IDayNight
	{
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
