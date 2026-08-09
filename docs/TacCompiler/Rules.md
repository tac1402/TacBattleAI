---
title: Rules
---

## Основы

1. Присоединение логики к сущностям (Flow, Spatial) [далее просто Flow, а Spatial это частный случай] выполняется препроцессором если разработчик указал атрибут [TacLogic] с типом логики.

```csharp
	public class Agent : Spatial
	{
        [TacLogic] AgentLogic logic;
	}
```


2. Flow по умолчанию не дает полный доступ к своей логике (Logic), она доступна только самой сущности и её наследникам.

```csharp
	public class Agent : Spatial
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
	public class Agent : Spatial
	{
        protected override void CreateLogic() { baseLogic = new AgentLogic(); }
  	}
	public class Person : Agent
	{
        protected override void CreateLogic() { baseLogic = new PersonLogic(); }
  	}
```


4. По умолчанию TacCompiler строит публичную проекцию интерфейса логики во Flow. Происходит полное поднятие публичного API Logic на уровень Flow, но Flow делегирует это Logic. Это можно сравнить с наследованием, но формально это агрегация, с последующим делегированием публичных членов логики наружу от Flow.

```csharp
// Для полей и свойств - доступ только для чтения
public GenderType Gender => logic.Gender;
public bool IsDead => logic.IsDead;
public float Health => logic.Health;
public LDictionary_<string, float> Skills => logic.Skills;
// Для методов
public void ApplyDamage(float argDamage) => logic.ApplyDamage(argDamage);
public void SetTarget(int argId) => logic.SetTarget(argId);
// Для событий - доступ на подписку и отписку
public event Change ChangeHealth
{
    add => logic.ChangeHealth += value;
    remove => logic.ChangeHealth -= value;
}
```
  
	


