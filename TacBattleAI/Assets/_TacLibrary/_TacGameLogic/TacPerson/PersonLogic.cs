// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System.Collections.Generic;

using Tac.Agent_;
using UnityEF;

namespace Tac.Person_
{
	public class PersonLogic : AgentLogic
	{

		#region  Stats & Skills

		/// <summary>
		/// Пол (мужской или женский) персонажа
		/// </summary>
		public GenderType Gender = GenderType.Unknow;

		public void SetGender(GenderType argGender) { Gender = argGender; }

		/// <summary>
		/// Статы (характеристики)
		/// </summary>
		public LDictionary_<string, float> Stats;
		/// <summary>
		/// Типы важности для характеристик
		/// </summary>
		public LDictionary_<string, StatType> StatTypes;

		/// <summary>
		/// Скилы (умения)
		/// </summary>
		public LDictionary_<string, float> Skills;


		/// <summary>
		/// Информация о статах или скилах, которая поддерживается для отображения в UI
		/// </summary>
		private List<NamedValue> Info = new List<NamedValue>();
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

		public void AddStat(string argName, float argValue = 0, StatType argStatType = StatType.Normal, bool argAddInfo = true)
		{
			Stats.Add(argName, argValue);
			StatTypes.Add(argName, argStatType);
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
		public LDictionary<string, AgentPoint> Places;

		/// <summary>
		/// Установить место
		/// </summary>
		public void SetPlace(string argKey, AgentPoint argPlace)
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

	public enum StatType
	{
		Critical,   // Критический (0–100, нельзя допускать падения)
		Normal,     // Обычный (любые значения, не критичен)
		Money       // Валюта
	}

}
