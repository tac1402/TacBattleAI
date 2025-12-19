// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2016 Sergej Jakovlev

// You can use this code for educational purposes only;
// this code or its modifications cannot be used for commercial purposes
// or in proprietary libraries without permission from the author

using System.Collections.Generic;
using System;

namespace Tac.HealthSystem
{
	/// <summary>
	/// Части тела
	/// </summary>
	public enum BodyParts
	{
		/// <summary>
		///  Голова
		/// </summary>
		Head = 1,
		/// <summary>
		///  Грудь
		/// </summary>
		Thorax = 2,
		/// <summary>
		///  Живот
		/// </summary>
		Abdomen = 3,

		/// Бедро правое
		/// </summary>
		ThighRight = 4,
		/// <summary>
		/// Бедро левое
		/// </summary>
		ThighLeft = 5,
		/// <summary>
		/// Плечо правое
		/// </summary>
		ShoulderRight = 6,
		/// <summary>
		/// <summary>
		/// Плечо левое
		/// </summary>
		ShoulderLeft = 7,
		/// <summary>
		/// Голень правая
		/// </summary>
		ShinRight = 8,
		/// <summary>
		/// Голень левая
		/// </summary>
		ShinLeft = 9,
		/// <summary>
		/// Предплечье правое
		/// </summary>
		ForearmRight = 10,
		/// <summary>
		/// Предплечье левое
		/// </summary>
		ForearmLeft = 11
	}

	/// <summary>
	/// Состояние части тела
	/// </summary>
	public class BodyPartState
	{
		/// <summary>
		/// Зависимые части тела
		/// </summary>
		List<Dependency> Dependency = new List<Dependency>();

		List<VitalSystemState> SystemDependency = new List<VitalSystemState>();

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
				if (value < 0)
				{
					int locDegradationForce = (int)(value * -1);
					state = 0;

					for (int i = 0; i < SystemDependency.Count; i++)
					{
						int tmpMaxForceDegradation = SystemDependency[i].MaxForceDegradation;
						SystemDependency[i].MaxForceDegradation = locDegradationForce;
						SystemDependency[i].Degradation();
						SystemDependency[i].MaxForceDegradation = tmpMaxForceDegradation;
					}
				}
				else
				{
					state = value;
				}
			}

		}


		/// <summary>
		/// Снижение функциональности в результате зависимостей от других частей тела
		/// </summary>
		public float DependencyState;

		public float ComplexState
		{
			get
			{
				float locComplexState = State - DependencyState;
				if (locComplexState < 0)
				{
					locComplexState = 0;
				}
				return locComplexState;
			}
		}

		/// <summary>
		/// Коэфициент влияния на здоровье
		/// </summary>
		/// <remarks>
		/// 50% здоровья всех частей тела суммарно указывают на повреждения
		/// </remarks>
		public float Koef;



		public BodyPartState(float argBeginState, float argKoef)
		{
			State = argBeginState;
			Koef = argKoef;
		}

		public void AddBodyDependency(BodyPartState argDependencyPart, float argDependencyKoef)
		{
			Dependency.Add(new Dependency(argDependencyPart, argDependencyKoef));
		}

		public void AddSystemDependency(VitalSystemState argVitalSystem)
		{
			SystemDependency.Add(argVitalSystem);
		}


		public void ClearDependency()
		{
			DependencyState = 0;
		}

		/// <summary>
		/// Взвешенное значение повреждения, учитывая взаимосвязь систем
		/// </summary>
		/// <returns></returns>
		public float Injury()
		{
			for (int i = 0; i < Dependency.Count; i++)
			{
				DependencyState += Dependency[i].GetDependency();
			}

			return Koef * ((100 - ComplexState) / 100);
		}


	}

	/// <summary>
	/// Зависимость одной части тела от другой
	/// </summary>
	public class Dependency
	{
		/// <summary>
		/// Степень влияния на подсистему
		/// </summary>
		/// <remarks>
		/// 1 - линейное влияние
		/// больше 1 - влияние на подсистему больше чем повреждения самой системы
		/// меньше 1 - влияние на подсистему меньше чем повреждения самой системы
		/// </remarks>
		public float DependencyKoef = 1;


		/// <summary>
		/// Ссылка на состояние зависимой части
		/// </summary>
		BodyPartState DependencyPart;

		public Dependency(BodyPartState argDependencyPart, float argDependencyKoef)
		{
			DependencyPart = argDependencyPart;
			DependencyKoef = argDependencyKoef;
		}

		/// <summary>
		/// Расчитывает величину зависимости
		/// </summary>
		public float GetDependency()
		{
			float retDependency = 100 - (float)(Math.Pow(DependencyPart.ComplexState, DependencyKoef) / Math.Pow(100, DependencyKoef - 1));
			return retDependency;
		}

	}
}