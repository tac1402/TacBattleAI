---
title: Rules
---

## Основы

`1. Присоединение логики к сущностям (Flow, Spatial) [далее просто Flow, а Spatial это частный случай] выполняется препроцессором если разработчик указал атрибут [TacLogic] с типом логики.

```csharp
	public class Agent : Spatial
	{
        [TacLogic] AgentLogic logic;
	}
```


`2. Flow по умолчанию не дает полный доступ к своей логике (Logic), она доступна только самой сущности и её наследникам.

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


`3. Logic создается через фабрику Flow.CreateLogic() и может быть переопределена в наследниках логикой специфичной для них.

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


`4. По умолчанию TacCompiler строит публичную проекцию интерфейса Logic во Flow. Происходит полное проецирование публичного API Logic непосредственно во Flow, при этом Flow делегирует реализацию проецируемых членов своей Logic. Это можно сравнить с наследованием, но формально это агрегация, с последующим делегированием публичных членов логики наружу от Flow по особым правилам. Таким образом, с точки зрения внешнего потребителя Flow выглядит так, как будто его публичный интерфейс содержит API Logic непосредственно.

```csharp
// Для полей и свойств - доступ только для чтения
public GenderType Gender => logic.Gender;
public bool IsDead => logic.IsDead;
public float Health => logic.Health;
public LDictionary_<string, float> Skills => logic.Skills;
// Для методов - без in, out, ref в параметрах, но допускают кортежи для выхода
public void ApplyDamage(float argDamage) => logic.ApplyDamage(argDamage);
public void SetTarget(int argId) => logic.SetTarget(argId);
public (JobType, int) Contest(Agent argAgent) => logic.Contest(argAgent);
// Для событий - доступ на подписку и отписку
public event Change ChangeHealth
{
    add => logic.ChangeHealth += value;
    remove => logic.ChangeHealth -= value;
}
```

## AllowSet 

__AllowSet__ - это атрибут, который дает явное разрешение нарушить односторонние направление доступа для конкретного состояния. И тут может возникнуть впечатление, что мы вернулись опять к двунаправленной зависимости. Но важно подчеркнуть, что __AllowSet__ меняет это точечно, там где разработчик уверен, что это поле или свойство имеет такую природу, что не может изменяться внутри своей логики (как чистой в Logic, так и контекстной во Flow). Например, возраст, усталость, деньги - не меняются самим персонажем, это ответственность других сущностей, например, общества и конкретных предприятий, в которых персонаж работает и получает зарплату. Отделить такие поля от подавляющего числа характеристик, управляемых самим персонажем достаточно полезно. Но синтаксически, важно чтобы это отличалось от прямого присваивания значения. Поэтому препроцессор, формирует метод типа _SetAge()_ вместо сеттера к Age (сеттер остается доступен только внутри Person). Таким образом, TacCompiler навязывает правильный стиль доступа вместо машинальной самоинкапсуляции полей.

```csharp
// При указании AllowSet в Logic
public class PersonLogic : AgentLogic
{
  [AllowSet]
  public int Age;
}
// во Flow будет добавлен
public class PersonLogic : AgentLogic
{
  //[TacLogic] AgentLogic logic;
#region Generated Logic
  ...
  public int Age => logic.Age;
  public void SetAge(int value)
  {
    logic.Age = value;
  }
#endregion
}
```

## Делегаты


