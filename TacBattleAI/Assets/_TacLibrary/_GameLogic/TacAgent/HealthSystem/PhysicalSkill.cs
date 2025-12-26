// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2016 Sergej Jakovlev

// You can use this code for educational purposes only;
// this code or its modifications cannot be used for commercial purposes
// or in proprietary libraries without permission from the author

namespace Tac.HealthSystem
{
	public class PhysicalSkill
	{
		/// <summary>
		/// Величина текущего умения
		/// </summary>
		private float state = 1;

		public float State
		{
			get { return state; }
			set
			{
				state = value;
				if (state < MinState)
				{
					state = MinState;
				}
				if (state > MaxState)
				{
					state = MaxState;
				}
			}
		}

		/// <summary>
		/// Штрафы на умение, например, вызванные здоровьем 
		/// </summary>
		public float DependencyState;

		/// <summary>
		/// Итоговый уровень умения, учитывая штрафы
		/// </summary>
		public float ComplexState
		{
			get
			{
				float locComplexState = State - DependencyState;
				if (locComplexState < 1)
				{
					locComplexState = 1;
				}
				return locComplexState;
			}
		}

		/// <summary>
		/// Минимальный уровень для этого умения
		/// </summary>
		public float MinState;

		/// <summary>
		/// Максимальный уровень для этого умения
		/// </summary>
		public float MaxState;

		public PhysicalSkill(float argMinState = 0, float argMaxState = 100)
		{
			MinState = argMinState;
			MaxState = argMaxState;
		}

		public void Recalc(float argHealthState)
		{
			DependencyState = (State / (argHealthState / 100f)) - State;
		}

	}
}
