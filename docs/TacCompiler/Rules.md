---
title: Rules
---

## Основы

1. Присоединение логики к сущностям (Flow, Spatial) [далее просто Flow, а Spatial это частный случай] выполняется препроцессором если разработчик указал атрибут [TacLogic] с типом логики.
   ```csharp
	public partial class Agent : Spatial
	{
        [TacLogic] AgentLogic logic;
	}
```


2. Flow по умолчанию не дает полный доступ к своей логике (Logic), она доступна только самой сущности и её наследникам.
   ```csharp
	public partial class Agent : Spatial
	{
        //[TacLogic] AgentLogic logic;
#region Generated Logic
        protected AgentLogic logic 
        {
            get { return baseLogic as AgentLogic; }
            set { baseLogic = value; }
        }
#endregion
	}
```


3. Logic создается через фабрику Flow.CreateLogic() и может быть переопределена в наследниках логикой специфичной для них.
   ```csharp
	public partial class Agent : Spatial
	{
        protected override void CreateLogic() { baseLogic = new AgentLogic(); }
  	}
	public partial class Person : Agent
	{
        protected override void CreateLogic() { baseLogic = new PersonLogic(); }
  	}
```


4.
	


