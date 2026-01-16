// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

namespace Tac
{
	public partial class GameTime
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
