// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac
{
	/// <summary>
	/// Логика игрового времени
	/// </summary>
	public class DayNightLogic : Logic
	{
		public TimeSpan Time;

		/// <summary>
		/// Длина дня в реальных секундах
		/// </summary>
		private const float DayLength = 120;
		/// <summary>
		/// Длина ночи в реальных секундах
		/// </summary>
		private const float NightLength = 120;
		/// <summary>
		/// Длина суток в игровых часах
		/// </summary>
		private const float GameDayLenght = 24;

		/// <summary>
		/// Прошел час
		/// </summary>
		public event Tick NextHour;
		/// <summary>
		/// Прошли сутки
		/// </summary>
		public event Tick NextDay;

		public TimeMode timeMode;

		private float daySpeedMultiplier;
		public float DaySpeedMultiplier
		{
			get
			{
				if (timeMode == TimeMode.Normal)
				{
					daySpeedMultiplier = GameDayLenght / (float)(DayLength + NightLength);
				}
				return daySpeedMultiplier;
			}
		}


		public (int, float) UpdateTime(int argCurrentDay,float argCurrentTime)
		{
			int currentDay = argCurrentDay;
			float currentTime = argCurrentTime;
			int oldHour = (int)Math.Floor(argCurrentTime);
			int oldMinutes = (int)((argCurrentTime - Math.Floor(argCurrentTime)) * 60.0f);

			currentTime += DaySpeedMultiplier / 10f;
			if (argCurrentTime >= GameDayLenght)
			{
				argCurrentTime = 0;
				currentDay++;
				if (NextDay != null)
				{
					NextDay(new GameTime(argCurrentDay, 0));
				}
			}

			int currentHour = (int)Math.Floor(currentTime);

			if (oldHour != currentHour)
			{
				if (NextHour != null)
				{
					NextHour(new GameTime(argCurrentDay, currentHour));
				}
			}
			return (currentDay, currentTime);
		}

		public DateTime GetDateTime(int argDay, TimeSpan argTime)
		{
			DateTime dt = new DateTime(2025, 1, 1, 0, 0, 0);
			dt = dt.AddDays(argDay);
			dt = dt.AddHours(argTime.Hours);
			dt = dt.AddMinutes(argTime.Minutes);
			return dt;
		}
	}

	public enum TimeMode
	{
		Normal = 0,
		Fast = 1,
	}

	public delegate void Tick(GameTime argGameTime);

}
