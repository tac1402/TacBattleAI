---
title: CrossRef
---

Достаточно редко, но тем не менее в некоторых специальных случаях, появляются т.н. перекрестные ссылки между объектами. Намного чаще это признак плохой архитектуры, но даже в хорошо спланированной архитектуре это имеет место быть, когда перекрестные ссылки встречаются в разных ролях. Рассмотрим специальный случай. 

```csharp
// Агент
public partial class Agent
{
}
// Точка интереса агента
public partial class AgentPoint
{
  // Очередь в порядке прихода агентов в точку
	public Queue<AgentInPoint> Agents = new Queue<AgentInPoint>();
}
// Агент в точке
public partial class AgentInPoint
{
  public Agent Agent; // Агент
  public GameTime EnterTime; // Время когда он пришел в точку
}
public partial class Person : Agent
{
  // Места интереса, в которые ходит конкретно эта персона
	public Dictionary<string, AgentPoint> Places;
}

```


