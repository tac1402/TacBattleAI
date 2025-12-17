// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2016 Sergej Jakovlev
// You can use this code for educational purposes only;
// this code or its modifications cannot be used for commercial purposes
// or in proprietary libraries without permission from the author

namespace Tac.HealthSystem
{

	/// <summary>
	/// Жизненно важные системы
	/// </summary>
	public enum VitalSystems
	{
		/// <summary>
		/// Мозговая деятельность
		/// </summary>
		Cerebration = 1,
		/// <summary>
		/// Кровеносная система
		/// </summary>
		Сirculatory = 2,
		/// <summary>
		/// Дыхательная система
		/// </summary>
		Respiratory = 3,
		/// <summary>
		/// Пищевая система
		/// </summary>
		Digestive = 4,
		/// <summary>
		/// Иммунная система
		/// </summary>
		Immune = 5
	}

	/// <summary>
	/// Состояние жизненно важной системы
	/// </summary>
	public class VitalSystemState
	{
		/// <summary>
		/// Физическое состояние
		/// </summary>
		/// <remarks>
		/// от 0 до 100% (0 - полностью не функционирует)
		/// </remarks>
		private float state;

		public float State
		{
			get { return state; }
			set
			{
				state = value;
				if (state < 0)
				{
					state = 0;
				}
			}
		}

		/// <summary>
		/// Скорость деградации (один раз через это кол-во секунд)
		/// </summary>
		public int SpeedDegradation;
		/// <summary>
		/// Скорость регенерации (один раз через это кол-во секунд)
		/// </summary>
		public int SpeedRegeneration;
		/// <summary>
		/// Максимальная сила (интенсивность) деградации
		/// </summary>
		public int MaxForceDegradation;
		/// <summary>
		/// Максимальная сила (интенсивность) восстановления
		/// </summary>
		public int MaxForceRegeneration;

		/// <summary>
		/// Уровень необратимого (без лечения) изменения
		/// </summary>
		public int LevelIrreversibleChange;


		/// <summary>
		/// Генератор случайности
		/// </summary>
		private System.Random rnd;

		public VitalSystemState(int argState, int argSpeedDegradation, int argSpeedRegeneration,
			int argMaxForceDegradation, int argMaxForceRegeneration,
			int argLevelIrreversibleChange)
		{
			State = argState;
			SpeedDegradation = argSpeedDegradation;
			SpeedRegeneration = argSpeedRegeneration;
			MaxForceDegradation = argMaxForceDegradation;
			MaxForceRegeneration = argMaxForceRegeneration;
			LevelIrreversibleChange = argLevelIrreversibleChange;
			SetRandom(new System.Random());
		}

		public void SetRandom(System.Random argRandom)
		{
			rnd = argRandom;
		}

		public void Degradation()
		{
			float locChangeValue = (float)(rnd.NextDouble() * MaxForceDegradation);
			State -= locChangeValue;
		}

		public void AutoRegeneration()
		{
			if (State > LevelIrreversibleChange)
			{
				Regeneration();
			}
		}

		public void Regeneration()
		{
			float locChangeValue = (float)(rnd.NextDouble() * MaxForceDegradation);
			State += locChangeValue;
		}

	}
}