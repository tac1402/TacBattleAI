// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Tac
{
	public partial class DayNight: MonoBehaviour
	{

		/// <summary>
		/// Длина дня в реальных секундах
		/// </summary>
		public float DayLengthFast = 30;
		/// <summary>
		/// Длина ночи в реальных секундах
		/// </summary>
		public float NightLengthFast = 30;


		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущие время
		/// </summary>
		public Text GameTime;
		/// <summary>
		/// Текстовое поле в UI в котором будет отображаться текущий номер суток
		/// </summary>
		public Text GameDays;
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

				if (GameDays != null)
				{
					GameDays.text = "Day # " + currentDay.ToString();
				}
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
				else if (timeMode == TimeMode.Fast)
				{
					daySpeedMultiplier = GameDayLenght / (float)(DayLengthFast + NightLengthFast);
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

			/*if (CurrentTime <= GameNight)
			{
				AMPM = "NT";
			}
			else
			{
				AMPM = "DT";
				time -= GameNight;
			}*/

			string timeString = Mathf.Floor(time).ToString("F0").PadLeft(2, '0') + " : "
				+ minutes.ToString("F0").PadLeft(2, '0') + " " + AMPM;

			Time = new System.TimeSpan((int)Mathf.Floor(time), (int)Mathf.Floor(minutes), 0);

			if (GameTime != null)
			{
				GameTime.text = timeString;
			}
		}
	}
}