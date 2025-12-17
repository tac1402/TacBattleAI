// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2016 Sergej Jakovlev
// You can use this code for educational purposes only;
// this code or its modifications cannot be used for commercial purposes
// or in proprietary libraries without permission from the author

using System;
using System.Collections.Generic;

namespace Tac.HealthSystem
{
    public class HealthState
    {
        /// <summary>
        /// Здоровье
        /// </summary>
        public float Health = 0;

        /// <summary>
        /// Физическое состояние частей тела
        /// </summary>
        public Dictionary<BodyParts, BodyPartState> Body = new Dictionary<BodyParts, BodyPartState>();

        /// <summary>
        /// Состояние жизненно важных систем
        /// </summary>
        public Dictionary<VitalSystems, VitalSystemState> System = new Dictionary<VitalSystems, VitalSystemState>();

		/// <summary>
		/// Генератор случайности
		/// </summary>
		private System.Random rnd;

		public HealthState(System.Random argRandom)
        {
            rnd = argRandom;
			SetVitalSystems();
			SetBodyParts();
			CalcHealth();
        }

        public void SetBodyParts()
        {
            // Влияние частей тела на здоровье
            Body.Add(BodyParts.Head, new BodyPartState(100, 14));
            Body.Add(BodyParts.Thorax, new BodyPartState(100, 8));
            Body.Add(BodyParts.Abdomen, new BodyPartState(100, 8));

            Body.Add(BodyParts.ThighRight, new BodyPartState(100, 3));
            Body.Add(BodyParts.ShinRight, new BodyPartState(100, 2));
            Body.Add(BodyParts.ShoulderRight, new BodyPartState(100, 3));
            Body.Add(BodyParts.ForearmRight, new BodyPartState(100, 2));

            Body.Add(BodyParts.ThighLeft, new BodyPartState(100, 3));
            Body.Add(BodyParts.ShinLeft, new BodyPartState(100, 2));
            Body.Add(BodyParts.ShoulderLeft, new BodyPartState(100, 3));
            Body.Add(BodyParts.ForearmLeft, new BodyPartState(100, 2));

            // Взаимосвязь влияний между частями тела
            Body[BodyParts.Thorax].AddBodyDependency(Body[BodyParts.Head], 2);
            Body[BodyParts.Abdomen].AddBodyDependency(Body[BodyParts.Thorax], 0.5f);
            Body[BodyParts.ThighRight].AddBodyDependency(Body[BodyParts.Abdomen], 0.7f);
            Body[BodyParts.ThighLeft].AddBodyDependency(Body[BodyParts.Abdomen], 0.7f);
            Body[BodyParts.ShoulderRight].AddBodyDependency(Body[BodyParts.Thorax], 0.7f);
            Body[BodyParts.ShoulderLeft].AddBodyDependency(Body[BodyParts.Thorax], 0.7f);
            Body[BodyParts.ShinRight].AddBodyDependency(Body[BodyParts.ThighRight], 1.5f);
            Body[BodyParts.ShinLeft].AddBodyDependency(Body[BodyParts.ThighLeft], 1.5f);
            Body[BodyParts.ForearmRight].AddBodyDependency(Body[BodyParts.ShoulderRight], 1.5f);
            Body[BodyParts.ForearmLeft].AddBodyDependency(Body[BodyParts.ShoulderLeft], 1.5f);

            // Влияния на жизненно важные системы
            Body[BodyParts.Head].AddSystemDependency(System[VitalSystems.Cerebration]);
			Body[BodyParts.Thorax].AddSystemDependency(System[VitalSystems.Respiratory]);
			Body[BodyParts.Abdomen].AddSystemDependency(System[VitalSystems.Digestive]);

			Body[BodyParts.Head].AddSystemDependency(System[VitalSystems.Сirculatory]);
			Body[BodyParts.Thorax].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.Abdomen].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ThighRight].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ThighLeft].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ShoulderRight].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ShoulderLeft].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ShinRight].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ShinLeft].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ForearmRight].AddSystemDependency(System[VitalSystems.Сirculatory]);
            Body[BodyParts.ForearmLeft].AddSystemDependency(System[VitalSystems.Сirculatory]);
        }

        public void SetVitalSystems()
        {
            int SpeedDefault = 5;

            System.Add(VitalSystems.Cerebration, new VitalSystemState(100, 20, 10, 1, 1, 80));
            System.Add(VitalSystems.Сirculatory, new VitalSystemState(100, SpeedDefault, SpeedDefault, 3, 1, 50));
            System.Add(VitalSystems.Respiratory, new VitalSystemState(100, SpeedDefault, SpeedDefault, 3, 1, 50));
            System.Add(VitalSystems.Digestive, new VitalSystemState(100, SpeedDefault, SpeedDefault, 3, 1, 20));
            System.Add(VitalSystems.Immune, new VitalSystemState(100, SpeedDefault, SpeedDefault, 3, 1, 50));

			foreach (VitalSystemState system in System.Values)
			{
                system.SetRandom(rnd);
			}
		}

		/// <summary>
		/// Рачет показания здоровья
		/// </summary>
		public void CalcHealth()
        {
			// Очистка влияния зависимостей перед пересчетом повреждений при вызове Injury()
			foreach (BodyPartState part in Body.Values)
            {
                part.ClearDependency();
            }
            // Расчет повреждений
            float locInjury = 0;
			foreach (BodyPartState part in Body.Values)
			{
				locInjury += part.Injury();
            }

            Health = 50 - locInjury;

            float locComplexState = System[VitalSystems.Cerebration].State * 0.16f +
                                    System[VitalSystems.Сirculatory].State * 0.10f +
                                    System[VitalSystems.Respiratory].State * 0.10f +
                                    System[VitalSystems.Digestive].State * 0.06f +
                                    System[VitalSystems.Immune].State * 0.06f;
            Health += locComplexState;

            foreach (VitalSystemState system in System.Values)
            {
				if (system.State <= 0)
				{
					Health = 0;
					break;
				}
			}

        }

        public float Injury(float argState, float argKoef)
        {
            return argKoef * ((100 - argState) / 100);
        }

    }



}