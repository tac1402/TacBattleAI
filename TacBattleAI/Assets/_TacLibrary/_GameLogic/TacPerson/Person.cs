// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;

using Tac.Agent;

namespace Tac.Person
{ 
	public partial class Person : Agent.Agent
	{

		#region  Stats & Skills

		/// <summary>
		/// Пол (мужской или женский) персонажа
		/// </summary>
		public GenderType Gender = GenderType.Unknow;

		/// <summary>
		/// Статы (характеристики)
		/// </summary>
		public Dictionary<string, float> Stats = new Dictionary<string, float>();
		/// <summary>
		/// Скилы (умения)
		/// </summary>
		public Dictionary<string, float> Skills = new Dictionary<string, float>();


		/// <summary>
		/// Информация о статах или скилах, которая поддерживается для отображения в UI
		/// </summary>
		public List<NamedValue> Info = new List<NamedValue>();
		/// <summary>
		/// Полная информация о статах и скилах, разбитая на строки для UI
		/// </summary>
		public string InfoTxt
		{
			get
			{
				string ret = "";
				foreach (var item in Info)
				{
					ret += item.Name + "\t = " + item.Value.ToString("F2") + "\n";
				}
				return ret;
			}
		}

		/// <summary>
		/// Изменилась информация о статах или скилах
		/// </summary>
		public event Change OnChangeInfo;

		public void AddSkill(string argName, float argValue = 0, bool argAddInfo = true)
		{
			Skills.Add(argName, argValue);
			if (argAddInfo == true)
			{
				Info.Add(new NamedValue(argName, argValue));
			}
		}

		public void AddStat(string argName, float argValue = 0, bool argAddInfo = true)
		{
			Stats.Add(argName, argValue);
			if (argAddInfo == true)
			{
				Info.Add(new NamedValue(argName, argValue));
			}
		}

		/// <summary>
		/// Сменить значение стата или скила 
		/// </summary>
		public void Change(string argName, float argValue)
		{
			if (argValue < 0) { argValue = 0; }

			if (Stats.ContainsKey(argName))
			{
				Stats[argName] = argValue;
			}
			if (Skills.ContainsKey(argName))
			{
				Skills[argName] = argValue;
			}
			ChangeInfo(argName, argValue);
		}

		private void ChangeInfo(string argName, float argValue)
		{
			for (int i = 0; i < Info.Count; i++)
			{
				if (Info[i].Name == argName)
				{
					Info[i].Value = argValue;

					if (OnChangeInfo != null)
					{
						OnChangeInfo();
					}
					break;
				}
			}
		}


		#endregion


		#region Places

		/// <summary>
		/// Место работы
		/// </summary>
		public AgentPoint WorkPlace
		{
			get { return (GetPlace("Work")); }
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
		/// Места интереса
		/// </summary>
		public Dictionary<string, AgentPoint> Places = new Dictionary<string, AgentPoint>();

		/// <summary>
		/// Установить место
		/// </summary>
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
		/// <summary>
		/// Получить место
		/// </summary>
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


	}
}
