---
title: CrossRef
---

Достаточно редко, но тем не менее в некоторых специальных случаях, появляются т.н. перекрестные ссылки между объектами. Намного чаще это признак плохой архитектуры, но даже в хорошо спланированной архитектуре это имеет место быть, когда перекрестные ссылки встречаются в разных ролях. Рассмотрим специальный случай. С одной стороны, у нас есть персона __Person__, которая наследуется от агента __Agent__. С другой стороны, у нас есть некие точки интереса, в которых может находится агент __AgentPoint__. Когда агент приходит в точку интереса отмечается время его прибытия, создавая запись в очереди __AgentPoint.Agents__ с типом __AgentInPoint__, в котором фиксируется ссылка на самого агента __Agent__ и время __EnterTime__. Сама же персона __Person__ имеет полный список мест (точек интереса), которые она может посещать __Person.Places__. И наконец, у нас есть планировщик, которого мы рассматривать не будем, но который использует расписание персоны и его предпочтения __PersonPlan__, в котором хранится конкретный план на несколько дней, в котором указано какой персоне __PersonPlan.Person__ какие места нужно посетить __PersonPlan.DayPlan__.

```csharp
// Агент
public partial class Agent
{
  ...
}
// Точка интереса агента
public partial class AgentPoint
{
  // Очередь в порядке прихода агентов в точку
  public Queue<AgentInPoint> Agents;
}
// Агент в точке
public partial class AgentInPoint
{
  public Agent Agent; // Агент
  public GameTime EnterTime; // Время когда он пришел в точку
}
// Персона
public partial class Person : Agent
{
  // Места интереса, в которые ходит конкретно эта персона
  public Dictionary<string, AgentPoint> Places;
}
// План для персоны
public partial class PersonPlan
{
  public Person Person; // Персона
  public Queue<AgentPoint> DayPlan; // План на день, какие точки интереса нужно посетить
}

```

Теперь мы можем заметить, что если последовательно загружать сохраненные объекты этих классов, то окажется, что мы не можем восстановить их за один проход. Это замкнутый круг: __Person__ ссылается на __AgentPoint__, а __AgentPoint__ (через промежуточный AgentInPoint) ссылается на __Person__.

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/CrossRef.jpg)

Разрешить (_Resolve_) эти ссылки можно путем временной замены ссылок на идентификаторы (постоянно использовать идентификаторы, в данном случае, не рационально) для __Person.Places__. Именно, для этого в [DataSave](../../TacDConvert/DataSave) использовалась следующая конструкция: 

```csharp
namespace Tac.Person
{
	public partial class Person
	{
		public CrossRef<AgentPoint> PlacesRef = new CrossRef<AgentPoint>();
		public Dictionary<string, int> PlacesId
		{
			get { return PlacesRef.GetRef(Places); }
			set { PlacesRef.Ref = value; }
		}
		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);
      ...
			PlacesId = SaveQ(PlacesId, () => PlacesId);
		}
	}
}
```

Здесь создается временная идентификация (свойство) __PlacesId__, которая использует специальный класс CrossRef. Это класс находу при обращении к словарю идентификаторов __PlacesId__, создает их используя постоянную структуру __Places__. Вот как он это делает: 

```csharp
public class CrossRef<T> where T : Item
{
		public Dictionary<string, int> GetRef(Dictionary<string, T> argDictionary)
		{
			if (Ref == null)
			{
				Ref = new Dictionary<string, int>();
				foreach (var item in argDictionary)
				{
					if (item.Value != null)
					{
						Ref.Add(item.Key, item.Value.Id);
					}
				}
			}
			return Ref;
		}
}
```
В первый раз он создает, а затем использует уже созданную. А затем он просто использует тот факт, что любой базоdый класс наследуется от Item и имеет свой идентификатор Id. Он сопоставляет текстовый ключ с идентификатором. И таким образом, сохраняет вместо класса только его идентификатор. 

Наоборот, во время загрузки будет использоваться интерфейс [ILoadManager](../../TacSave/ILoadManager), для которого нужно реализовать метод отвечающий за восстановление игры __RecoverGame()__, и вот здесь важно восстановить (разрешить) ссылки на Gameobject, вам нужно написать код: 

```csharp
public void RecoverGame()
{
	// В случае перекрестных ссылок, например, когда Person.Places ссылается на AgentPoint,
	// а AgentPoint.Agents.Agent опосредованно ссылается на Person, нужно не прямо восстановить ссылки,
	// а при сохранении будут записаны только индексы и когда уже будут восстановленны все объекты,
	// нужно по индексам восстановить ссылки на сами объекты
	p.Places = p.PlacesRef.Resolve(allObject);
}
```

И вот как работает этот метод: 
```csharp
public Dictionary<string, T> Resolve(Dictionary<int, GameObject> allObject)
{
	Dictionary<string, T> ret = new Dictionary<string, T>();
	foreach (var item in Ref)
	{
		if (allObject.ContainsKey(item.Value))
		{
			ret.Add(item.Key, allObject[item.Value].GetComponent<T>());
		}
	}
	return ret;
}
```

Таким образом, только тогда, когда все GameObject будут созданы, ссылки на них можно восстановить. Иначе, может оказаться, что какие то объекты еще вообще не восстановлены из сохранения. 
