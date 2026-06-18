// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev


using System;
using System.Collections.Generic;

namespace Tac
{
	public class GameTime
	{
		public int Day;
		public int Hour;

		public string Time
		{
			get { return Day.ToString() + "-" + Hour.ToString(); }
		}

		public GameTime() { }

		public GameTime(int argDay, int argHour)
		{
			Day = argDay;
			Hour = argHour;
		}
	}
}
