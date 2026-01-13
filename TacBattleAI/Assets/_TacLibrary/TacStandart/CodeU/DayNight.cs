// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Tac
{
	public partial class DayNight: MonoBehaviour
	{
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущие время
		/// </summary>
		public Text gameTime;
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущий номер суток
		/// </summary>
		public Text gameDays;

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
		/// Текущие время
		/// </summary>
		public float currentTime = 6.0f;
		/// <summary>
		/// Текущие сутки (номер)
		/// </summary>
		public int currentDay = 1;

		/// <summary>
		/// Текущие время
		/// </summary>
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
		public int CurrentDay
		{
			get { return currentDay; }
			set
			{
				currentDay = value;
				GameDays = "Day # " + currentDay.ToString();
			}
		}



		public TimeMode TimeMode
		{
			get { return timeMode; }
			set
			{
				timeMode = value;

				if (timeMode == TimeMode.Normal)
				{
					daySpeedMultiplier = GameDayLenght / (float)(DayLength + NightLength);
				}

				if (TimeModeTxt != null)
				{
					TimeModeTxt.text = timeMode.ToString();
				}
			}
		}

		public float PlaySpeed = 1;
		public bool Pause = false;

		private void Awake()
		{
			TimeMode = TimeMode.Normal;
			StartCoroutine(Tick());
		}

		private void Update()
		{
			if (Keyboard.current[Key.Pause].wasPressedThisFrame)
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

					UpdateTime();
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
			string AMPM = "";
			float minutes = ((CurrentTime) - (Mathf.Floor(CurrentTime))) * 60.0f;
			float GameNight = (GameDayLenght / (DayLength + NightLength)) * NightLength;

			float time = CurrentTime;

			GameTime = Mathf.Floor(time).ToString("F0").PadLeft(2, '0') + " : " + minutes.ToString("F0").PadLeft(2, '0') + " " + AMPM;

			Time = new System.TimeSpan((int)Mathf.Floor(time), (int)Mathf.Floor(minutes), 0);
		}
	}
}