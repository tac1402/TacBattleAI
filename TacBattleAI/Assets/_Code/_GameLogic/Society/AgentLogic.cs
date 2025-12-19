
using System.Collections.Generic;

namespace Tac.Agent
{
	public partial class Agent
	{
		/// <summary>
		/// Статы (характеристики)
		/// </summary>
		public Dictionary<string, float> Stats = new Dictionary<string, float>();
		/// <summary>
		/// Скилы (умения)
		/// </summary>
		public Dictionary<string, float> Skills = new Dictionary<string, float>();

		public virtual void AddStatsSkills()
		{
			Stats.Add("Health", 100);
		}

#if OnlyLogic
		public void Init() 
		{ 
			AddStatsSkills();
		}
		public void CheckPosition() { }
		public void Walk(Vector3_ argTarget, float stoppingDistance = 0.1f) { }
#endif
	}
}

