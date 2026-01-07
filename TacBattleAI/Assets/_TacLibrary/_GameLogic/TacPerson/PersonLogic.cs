// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;

using Tac.Agent;
using Tac.Organization;

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


		public List<NamedValue> Info = new List<NamedValue>();
		public string InfoTxt
		{
			get
			{
				string ret = "";
				foreach (var item in Stats)
				{
					ret += item.Key + "\t = " + item.Value.ToString("F2") + "\n";
				}
				foreach (var item in Skills)
				{
					ret += item.Key + "\t = " + item.Value.ToString("F2") + "\n";
				}
				//ret += "Education" + "\t = " + Education.ToString("F1") + "\n";
				//ret += "WorkExperience" + "\t = " + WorkExperience.ToString("F0") + "\n";
				return ret;
			}
		}




		public virtual void AddStatsSkills()
		{
			Stats.Add("Health", 100);
		}

		#region Places

		/// <summary>
		/// Место работы
		/// </summary>
		public Business WorkPlace
		{
			get { return (GetPlace("Work") as Business); }
			set { SetPlace("Work", value); }
		}

		/// <summary>
		/// Место жительства
		/// </summary>
		public AgentPoint ResidencePlace
		{
			get { return GetPlace("Residence"); }
			set { SetPlace("Residence", value); }
		}
		/// <summary>
		/// Место для обучения
		/// </summary>
		public AgentPoint LearningPlace
		{
			get { return GetPlace("Learning"); }
			set { SetPlace("Learning", value); }
		}
		/// <summary>
		/// Место для прогулок
		/// </summary>
		public AgentPoint CenterPlace
		{
			get { return GetPlace("Center"); }
			set { SetPlace("Center", value); }
		}

		/// <summary>
		/// Места интереса
		/// </summary>
		public Dictionary<string, AgentPoint> Places = new Dictionary<string, AgentPoint>();

		private void SetPlace(string argKey, AgentPoint argPlace)
		{
			if (Places.ContainsKey(argKey))
			{
				Places[argKey] = argPlace;
			}
			else
			{
				Places.Add(argKey, argPlace);
			}
		}
		private AgentPoint GetPlace(string argKey)
		{
			AgentPoint ret = null;
			if (Places.ContainsKey(argKey))
			{
				ret = Places[argKey];
			}
			return ret;
		}

		#endregion



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
