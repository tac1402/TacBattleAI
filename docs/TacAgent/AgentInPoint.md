---
title: AgentInPoint
---

Отметка времени (tstamp) прихода агента в точку. Попадает в очередь класса AgentPoint.Agents.

```csharp
	public class AgentInPoint
	{
		public Agent Agent; // Агент
		public GameTime EnterTime; // Игровое время добавления/прихода агента в очередь/точку.
	}
```
