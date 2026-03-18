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
			var openPlaces = Person.Places.Where(pair => pair.Value.IsOpen(argGameTime.Hour)).ToDictionary(pair => pair.Key, pair => pair.Value);

			// 1. Вычисляем базовый приоритет для каждого стата, который есть у агента
			Dictionary<string, float> statPriority = new Dictionary<string, float>();
			foreach (var stat in Person.Stats)
			{
				string statName = stat.Key;
				float statValue = stat.Value;
				StatType statType = Person.StatTypes[statName];

				switch (statType)
				{
					case StatType.Critical:
						// Критический стат: приоритет растёт квадратично при приближении к нулю
						float deficit = 100 - statValue;
						statPriority[statName] = deficit * deficit; // квадрат дефицита
						break;

					case StatType.Normal:
					case StatType.Money:
					default:
						// Для обычных статов и денег пока базовый приоритет = 1
						statPriority[statName] = 1f;
						break;
				}
			}

			// 2. Собираем все ресурсы, которые предлагаются хотя бы в одной точке
			HashSet<string> availableStats = new HashSet<string>();
			foreach (var point in openPlaces)
			{
				foreach (var payment in point.Value.BaseWorkPayment)
				{
					availableStats.Add(payment.Name);
				}
			}

			// 3. Вычисляем виртуальный спрос на деньги за счёт недоступных критических статов
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

			// 4. Расчёт полезности (score) для каждой точки
			var scores = new Dictionary<string, float>();
			foreach (var point in openPlaces)
			{
				float score = 0;
				foreach (var payment in point.Value.BaseWorkPayment)
				{
					// Определяем приоритет для данного ресурса (если он есть у агента — используем его,
					// иначе считаем приоритет = 1 (Normal) )
					float priority = statPriority.TryGetValue(payment.Name, out float p) ? p : 1f;
					score += payment.Value * priority;
				}
				scores[point.Key] = score;
			}

			// 6. Сортировка по убыванию score и формирование очереди
			List<string> sortedIds = scores.OrderByDescending(x => x.Value)
								  .Select(x => x.Key)
								  .ToList();

			CurrentPlan = new Queue<string>(sortedIds);

		}

		public AgentPoint GetActual()
		{ 
			string key = CurrentPlan.Dequeue();
			return Person.Places[key];
		}

	}
}