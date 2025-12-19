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


	}
}

