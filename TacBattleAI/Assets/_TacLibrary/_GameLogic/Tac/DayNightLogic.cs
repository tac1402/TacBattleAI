// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tac
{
	public partial class DayNight
	{
		public TimeSpan Time;


		/// <summary>
		/// Длина дня в реальных секундах
		/// </summary>
		public float DayLength = 120;
		/// <summary>
		/// Длина ночи в реальных секундах
		/// </summary>
		public float NightLength = 120;
		/// <summary>
		/// Длина суток в игровых часах
		/// </summary>
		public float GameDayLenght = 24;

		private TimeMode timeMode;



		/// <summary>
		/// Прошел час
		/// </summary>
		public event Tick NextHour;
		/// <summary>
		/// Прошли сутки
		/// </summary>
		public event Tick NextDay;


		private float daySpeedMultiplier;


		void UpdateTime()
		{
			int oldHour = (int)Math.Floor(CurrentTime);
			int oldMinutes = (int)((CurrentTime - Math.Floor(CurrentTime)) * 60.0f);

			CurrentTime += daySpeedMultiplier / 10f;
			if (CurrentTime >= GameDayLenght)
			{
				CurrentTime = 0;
				CurrentDay++;
				if (NextDay != null)
				{
					NextDay(new GameTime(CurrentDay, 0));
				}
			}

			int currentHour = (int)Math.Floor(CurrentTime);

			if (oldHour != currentHour)
			{
				if (NextHour != null)
				{
					NextHour(new GameTime(CurrentDay, currentHour));
				}
			}

		}

		public DateTime GetDateTime(int argDay, TimeSpan argTime)
		{
			DateTime dt = new DateTime(2025, 1, 1, 0, 0, 0);
			dt = dt.AddDays(argDay);
			dt = dt.AddHours(argTime.Hours);
			dt = dt.AddMinutes(argTime.Minutes);
			return dt;
		}

		public DateTime DateTimeNow
		{
			get
			{
				return GetDateTime(CurrentDay, Time);
			}
		}


#if OnlyLogic

		/// <summary>
		/// Текущие время
		/// </summary>
		public float currentTime = 6.0f;
		/// <summary>
		/// Текущие сутки (номер)
		/// </summary>
		public int currentDay = 1;

		public void Tick(int argTickCount)
		{
			daySpeedMultiplier = GameDayLenght / (float)(DayLength + NightLength);
			for (int i = 0; i < argTickCount; i++)
			{
				UpdateTime();
			}
		}

		public TimeMode TimeMode
		{
			get { return timeMode; }
			set { timeMode = value; }
		}

		/// <summary>
		/// Текущие время
		/// </summary>
		public float CurrentTime
		{
			get { return currentTime; }
			set
			{
				currentTime = value;

				float minutes = (currentTime - (float)Math.Floor(currentTime)) * 60.0f;
				Time = new System.TimeSpan((int)Math.Floor(currentTime), (int)Math.Floor(minutes), 0);
			}
		}

		/// <summary>
		/// Текущие сутки (номер)
		/// </summary>
		public int CurrentDay
		{
			get { return currentDay; }
			set { currentDay = value; }
		}
#endif


	}

	public enum TimeMode
	{
		Normal = 0,
		Fast = 1,
	}

	public delegate void Tick(GameTime argGameTime);

}
