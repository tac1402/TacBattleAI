// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DnaCore;
using System;

namespace Tac
{

	/// <summary>
	/// Управляет игровым временем
	/// </summary>
	public class DayNight: Flow, IDayNight
	{
        //[TacLogic] DayNightLogic logic;
#region Generated Logic
        protected DayNightLogic logic
        {
            get
            {
                return baseLogic as DayNightLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new DayNightLogic();
        }

        public TimeSpan Time => logic.Time;
        public TimeMode timeMode => logic.timeMode;

        public DateTime GetDateTime(int argDay, TimeSpan argTime) => logic.GetDateTime(argDay, argTime);

        public event Tick NextHour { add => logic.NextHour += value; remove => logic.NextHour -= value; }

        public event Tick NextDay { add => logic.NextDay += value; remove => logic.NextDay -= value; }
#endregion

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

		/// <summary>
		/// Пауза полной остановки
		/// </summary>
		public static bool PauseCompleteStop;

		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущие прошедшие реальное время
		/// </summary>
		public Text RealTime;
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущая скорость течения времени
		/// </summary>
		public Text TimeModeTxt;
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущие время
		/// </summary>
		public Text gameTime;
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущий номер суток
		/// </summary>
		public Text gameDays;

		public string GameTime
		{
			get
			{
				if (gameTime != null) { return gameTime.text; } else { return string.Empty; }
			}
			set
			{
				if (gameTime != null) { gameTime.text = value; }
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


		/// <summary>
		/// Текущие время
		/// </summary>
		private float currentTime = 6.0f;
		/// <summary>
		/// Текущие сутки (номер)
		/// </summary>
		private int currentDay = 1;

		/// <summary>
		/// Текущие время
		/// </summary>
		[Mapped]
		public float CurrentTime
		{
			get { return currentTime; }
			set
			{
				currentTime = value;
				ShowTime();
			}
		}

		/// <summary>
		/// Текущие сутки (номер)
		/// </summary>
		[Mapped]
		public int CurrentDay
		{
			get { return currentDay; }
			set
			{
				currentDay = value;
				gameDays.text = "Day # " + currentDay.ToString();
			}
		}


		public TimeMode TimeMode
		{
			get { return logic.timeMode; }
			set
			{
				logic.timeMode = value;
				if (TimeModeTxt != null)
				{
					TimeModeTxt.text = logic.timeMode.ToString();
				}
			}
		}

		public float PlaySpeed = 1;
		public bool Pause = false;


		private void Start()
		{
			TimeMode = TimeMode.Normal;
			logic.DayLength = DayLength;
			logic.NightLength = NightLength;
			logic.DayLength = DayLength;
			StartCoroutine(Tick());
		}

		private void Update()
		{
			if (Keyboard.current[Key.Pause].wasPressedThisFrame && PauseCompleteStop == false)
			{
				PausePress();
			}
		}

		public void PausePress()
		{
			if (Pause == false)
			{
				Pause = true;
				UnityEngine.Time.timeScale = 0;
			}
			else
			{
				Pause = false;
				UnityEngine.Time.timeScale = PlaySpeed;
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				if (Pause == false)
				{
					UnityEngine.Time.timeScale = PlaySpeed;

					(CurrentDay, CurrentTime) = logic.UpdateTime(CurrentDay, CurrentTime);
					UpdateRealTime();
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

		public int rs = 0;
		public int rm = 0;
		void UpdateRealTime()
		{
			rs++;
			if (rs == 600) { rs = 0; rm++; }

			if (RealTime != null)
			{
				RealTime.text = rm.ToString().PadLeft(2, '0') + ":" + ((int)(rs / 10)).ToString().PadLeft(2, '0');
			}
		}

		void ShowTime()
		{
			int hours = (int)Mathf.Floor(CurrentTime);
			int minutes = (int)Mathf.Floor((CurrentTime - hours) * 60.0f);

			gameTime.text = hours.ToString("F0").PadLeft(2, '0') + " : " + minutes.ToString("F0").PadLeft(2, '0') + " ";

			logic.Time = new System.TimeSpan(hours, minutes, 0);
		}

#if OnlyLogic
		public void Tick(int argHourCount)
		{
			for (int i = 0; i < argHourCount; i++)
			{
				CurrentTime++;
				if (CurrentTime > 23)
				{ 
					CurrentTime = 0;
					CurrentDay ++;
				}
				logic.UpdateTime(CurrentDay, CurrentTime);
			}
		}
#endif


	}

}