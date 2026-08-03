// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;
using System.Numerics;
using Tac.Agent;
using UnityEF;
using UnityEngine;

namespace Tac.Person
{
	public partial class Person : Agent.Agent
	{
		protected new PersonLogic logic { get { return baseLogic as PersonLogic; } set { baseLogic = value; } }
		protected override void CreateLogic() { baseLogic = new PersonLogic(); }

		public LDictionary<string, AgentPoint> Places => logic.Places;
		public LDictionary_<string, float> Stats => logic.Stats;
		public LDictionary_<string, StatType> StatTypes => logic.StatTypes;
		public AgentPoint WorkPlace { get { return logic.WorkPlace; } set { logic.WorkPlace = value; } }
		public GenderType Gender { get { return logic.Gender; } set { logic.Gender = value; } }
		public string InfoTxt => logic.InfoTxt;

		public override void InitData()
		{
			base.InitData();
			logic.Stats = new LDictionary_<string, float>();
			logic.StatTypes = new LDictionary_<string, StatType>();
			logic.Skills = new LDictionary_<string, float>();
			logic.Places = new LDictionary<string, AgentPoint>();
		}


	}
}