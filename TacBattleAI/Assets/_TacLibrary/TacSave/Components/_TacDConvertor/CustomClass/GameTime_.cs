// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac.DConvert
{
	public class GameTime_ : ICustomConvert<GameTime_, GameTime>
	{
		public int Day;
		public int Hour;

		public GameTime_() { }

		public GameTime_(int argDay, int argHour)
		{
			Day = argDay; Hour = argHour;
		}

		public void ConvertFrom(GameTime v)
		{
			if (v == null)
			{
				Day = 0; Hour = 0;
			}
			else
			{
				Day = v.Day; Hour = v.Hour;
			}
		}

		public GameTime ConvertTo()
		{
			return new GameTime(Day, Hour);
		}
	}
}

