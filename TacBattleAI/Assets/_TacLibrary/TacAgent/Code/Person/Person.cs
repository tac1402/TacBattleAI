// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;
using System.Numerics;
using Tac.Agent_;
using UnityEF;

namespace Tac.Person_
{
	public partial class Person : Agent
	{
        //[TacLogic] PersonLogic logic;
#region Generated Logic
        protected new PersonLogic logic
        {
            get
            {
                return baseLogic as PersonLogic;
            }

            set
            {
                baseLogic = value;
            }
        }

        protected override void CreateLogic()
        {
            baseLogic = new PersonLogic();
        }

        public GenderType Gender => logic.Gender;

        public void SetGender(GenderType value)
        {
            logic.Gender = value;
        }

        public LDictionary_<string, float> Stats => logic.Stats;
        public LDictionary_<string, StatType> StatTypes => logic.StatTypes;
        public LDictionary_<string, float> Skills => logic.Skills;
        public LDictionary<string, AgentPoint> Places => logic.Places;

        public void AddSkill(string argName, float argValue = 0f, bool argAddInfo = true) => logic.AddSkill(argName, argValue, argAddInfo);
        public void AddStat(string argName, float argValue = 0f, StatType argStatType = StatType.Normal, bool argAddInfo = true) => logic.AddStat(argName, argValue, argStatType, argAddInfo);
        public void Change(string argName, float argValue) => logic.Change(argName, argValue);
        public string InfoTxt => logic.InfoTxt;
        public AgentPoint WorkPlace => logic.WorkPlace;

        public void SetWorkPlace(AgentPoint value)
        {
            logic.WorkPlace = value;
        }

        public AgentPoint ResidencePlace => logic.ResidencePlace;

        public void SetResidencePlace(AgentPoint value)
        {
            logic.ResidencePlace = value;
        }

        public override void InitData()
        {
            base.InitData();
            logic.Stats = new LDictionary_<string, float>();
            logic.StatTypes = new LDictionary_<string, StatType>();
            logic.Skills = new LDictionary_<string, float>();
            logic.Places = new LDictionary<string, AgentPoint>();
        }

        public event Change OnChangeInfo { add => logic.OnChangeInfo += value; remove => logic.OnChangeInfo -= value; }
#endregion


	}
}