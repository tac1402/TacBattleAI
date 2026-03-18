using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tac.Agent;

namespace Tac.Person
{
	public partial class PersonPlan
	{
		public Person Person;
		public Queue<string> CurrentPlan;

		public PersonPlan(Person argPerson)
		{
			Person = argPerson;
		}

		public void CalculateActual(GameTime argGameTime)
		{
			if (Person.Places == null || Person.Places.Count == 0) { return; }
			// +1 час на дорогу
			var openPlaces = Person.Places.Where(pair => pair.Value != null && pair.Value.IsOpen(argGameTime.Hour + 1))
								.ToDictionary(pair => pair.Key, pair => pair.Value);
			if (openPlaces.Count == 0) { return; }

			// 1. ¬ычисл€ем базовый приоритет дл€ каждого стата, который есть у агента
			Dictionary<string, float> statPriority = new Dictionary<string, float>();
			foreach (var stat in Person.Stats)
			{
				string statName = stat.Key;
				float statValue = stat.Value;
				StatType statType = Person.StatTypes[statName];

				statPriority.Add(statName, 0);
				switch (statType)
				{
					case StatType.Critical:
						//  ритический стат: приоритет растЄт 
						if (statValue > 10) // 10 - означает, что нехватает на 10 часов
						{
							statPriority[statName] = 10f;
						}
						else if (statValue > 5)
						{
							statPriority[statName] = 1f;
						}
						break;

					case StatType.Normal:
					case StatType.Money:
					default:
						// ƒл€ обычных статов и денег пока базовый приоритет = 1
						statPriority[statName] = 1f;
						break;
				}
			}

			// 2. —обираем все ресурсы, которые предлагаютс€ хот€ бы в одной точке
			HashSet<string> availableStats = new HashSet<string>();
			foreach (var point in openPlaces)
			{
				foreach (var payment in point.Value.BaseWorkPayment)
				{
					availableStats.Add(payment.Name);
				}
			}

			// 3. ¬ычисл€ем виртуальный спрос на деньги за счЄт недоступных критических статов
			float virtualMoneyDemand = 0;
			foreach (var stat in Person.Stats)
			{
				string statName = stat.Key;
				StatType statType = Person.StatTypes[statName];

				if (statType == StatType.Critical && availableStats.Contains(statName) == false)
				{
					virtualMoneyDemand += statPriority[statName];
				}
			}

			string moneyStatName = Person.StatTypes.FirstOrDefault(x => x.Value == StatType.Money).Key;
			statPriority[moneyStatName] = 1f + virtualMoneyDemand;

			// 4. –асчЄт полезности (score) дл€ каждой точки
			var scores = new Dictionary<string, float>();
			foreach (var point in openPlaces)
			{
				float score = 0;
				foreach (var payment in point.Value.BaseWorkPayment)
				{
					// ќпредел€ем приоритет дл€ данного ресурса (если он есть у агента Ч используем его,
					// иначе считаем приоритет = 1 (Normal) )
					float priority = statPriority.TryGetValue(payment.Name, out float p) ? p : 1f;
					score += payment.Value * priority;
				}
				scores[point.Key] = score;
			}

			// 6. —ортировка по убыванию score и формирование очереди
			List<string> sortedIds = scores.OrderByDescending(x => x.Value)
								  .Select(x => x.Key)
								  .ToList();

			CurrentPlan = new Queue<string>(sortedIds);
		}

		public AgentPoint GetActual()
		{
			if (CurrentPlan != null && CurrentPlan.Count > 0)
			{
				string key = CurrentPlan.Dequeue();
				return Person.Places[key];
			}
			return null;
		}

	}
}