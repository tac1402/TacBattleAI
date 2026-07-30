using System.Collections.Generic;
using System.Numerics;
using Tac.HealthSystem;
using UnityEngine;

namespace Tac.Agent
{
    public class AgentLogic : Logic
    {
		/// <summary>
		/// Место назначения (куда идти)
		/// </summary>
		public int TargetId;

		/// <summary>
		/// Занят ли? 
		/// </summary>
		public bool IsBusy = false;

		/// <summary>
		/// Где находится
		/// </summary>
		public int LocatedId = 0;


		public bool UseHealthState = false;

		internal System.Random rnd = new System.Random();

		internal HealthState HealthState;
		internal float health;
		/// <summary>
		/// Здоровье
		/// </summary>
		public float Health
		{
			get
			{
				float ret = 0;
				if (UseHealthState)
				{
					if (HealthState != null) { ret = HealthState.Health; }
				}
				else { ret = health; }
				return ret;
			}
		}

		/// <summary>
		/// Мертв ли
		/// </summary>
		public bool IsDead
		{
			get { return Health == 0; }
		}

		/// <summary>
		/// Заряд
		/// </summary>
		private PhysicalSkill charge = new PhysicalSkill(1, 100);
		/// <summary>
		/// Заряд/Выносливость
		/// </summary>
		public PhysicalSkill Charge { get { return charge; } set { charge = value; } }

		/// <summary>
		/// Меткость
		/// </summary>
		private PhysicalSkill precision = new PhysicalSkill(1, 100);
		/// <summary>
		/// Меткость
		/// </summary>
		public PhysicalSkill Precision { get { return precision; } set { precision = value; } }

		public event Change ChangeHealth;


		public virtual void AddStatsSkills() { }

		public void ApplyDamage(float argDamage)
		{
			BodyParts bodyPart = (BodyParts)rnd.Next(1, 11);
			ApplyDamage(bodyPart, argDamage);
		}

		public void ApplyDamage(BodyParts argBodyPart, float argDamage)
		{
			HealthState.Body[argBodyPart].State -= argDamage;
			CalcHealth();
		}

		internal float previousHealth;
		internal float previousStamina;

		public void CalcHealth()
		{
			previousHealth = HealthState.Health;
			previousStamina = Charge.State;

			HealthState.CalcHealth();

			// Расчитать снижение меткости при изменении здоровья
			Precision.Recalc(Health);

			if (ChangeHealth != null)
			{
				ChangeHealth();
			}
		}
		public void SetTarget(int argId)
		{
			IsBusy = true;
			LocatedId = -1;
			TargetId = argId;
		}

		public void SetLocated(int argId)
		{
			IsBusy = true;
			TargetId = 0;
			LocatedId = Id;
		}
		public void ResetLocated()
		{
			IsBusy = false;
			LocatedId = 0;
			TargetId = 0;
		}
	}
}

