using System.Collections.Generic;
using Tac.HealthSystem;

namespace Tac.Agent
{
    public partial class Agent
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

		private HealthState HealthState;
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


		public void ApplyDamage(float argDamage)
		{
			BodyParts bodyPart = (BodyParts)rnd.Next(1, 11);
			ApplyDamage(bodyPart, argDamage);
		}

		public void ApplyDamage(BodyParts argBodyPart, float argDamage)
		{
			HealthState.Body[argBodyPart].State -= argDamage;

			CalcHealth();

			if (IsDead == true)
			{
				agent.enabled = false;
			}
		}

		public void CalcHealth()
		{
			float previousHealth = HealthState.Health;
			float previousStamina = Charge.State;

			HealthState.CalcHealth();

			// Расчитать снижение меткости при изменении здоровья
			Precision.Recalc(Health);

			if (StatusBar != null)
			{
				StatusBar.SetHealth(HealthState.Health, previousHealth);
				StatusBar.SetStamina(Charge.State, previousStamina);
			}
		}

#if OnlyLogic
		public object StatusBar == null;
#endif

	}
}

