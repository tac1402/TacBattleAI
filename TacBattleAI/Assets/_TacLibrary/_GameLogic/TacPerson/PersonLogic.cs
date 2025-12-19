// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;
using Tac.Agent;

namespace Tac.Person
{ 
	public partial class Person : Agent.Agent
	{
		/// <summary>
		/// Статы (характеристики)
		/// </summary>
		public Dictionary<string, float> Stats = new Dictionary<string, float>();
		/// <summary>
		/// Скилы (умения)
		/// </summary>
		public Dictionary<string, float> Skills = new Dictionary<string, float>();

		public GenderType Gender = GenderType.Unknow;

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
