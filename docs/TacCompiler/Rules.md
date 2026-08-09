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

__AllowSet__ - это атрибут, который дает явное разрешение нарушить односторонние направление доступа для конкретного состояния. И тут может возникнуть впечатление, что мы вернулись опять к двунаправленной зависимости. Но важно подчеркнуть, что __AllowSet__ меняет это точечно, там где разработчик уверен, что это поле или свойство имеет такую природу, что не может изменяться внутри своей логики (как чистой в Logic, так и контекстной во Flow). Например, возраст, усталость, деньги - не меняются самим персонажем, это ответственность других сущностей, например, общества и конкретных предприятий, в которых персонаж работает и получает зарплату. Отделить такие поля от подавляющего числа характеристик, управляемых самим персонажем достаточно полезно. Но синтаксически, важно чтобы это отличалось от прямого присваивания значения. Поэтому препроцессор, формирует метод типа _SetAge()_ вместо сеттера к Age (сеттер остается доступен только внутри Person). Таким образом, TacCompiler навязывает семантически осмысленный стиль доступа вместо машинальной самоинкапсуляции полей.

```csharp
// При указании AllowSet в Logic
public class PersonLogic : AgentLogic
{
  [AllowSet]
  public int Age;
}
// во Flow будет добавлен
public class Person : Agent
{
  //[TacLogic] PersonLogic logic;
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

Таким образом, Flow при работе с Logic имеет как бы шлюз с двумя дверьми внутренними и внешними. Не все то, что может изменить Flow в Logic (_внутренняя дверь_) нужно предоставлять другим Flow (_внешняя дверь_). А если нужно, то в самом Logic нужно решить исходя из природы этой характеристики (свойства, поля) и пометить атрибутом __AllowSet__ . 

Причем замети, что в основном нужно работать прямо с полями, никакая самоинкапсуляция тут не нужна - она в любом случае будет избыточна, т.к. настоящую инкапсуляцию выполняет этот шлюз. Работать со свойствами нужно в более сложных случаях, когда установка значения порождает немедленные вычисления, уведомление через событие или вызов через делегат обновления UI. В таких случаях, сама природа характеристики такова, что она является частью вычислений, а связанную систему понятий, от которых зависит эта характеристика, нужно синхронизировать в актуальное состояние. Например, как ниже в примере с установкой на основе усталости рекомендаций ко сну.

```csharp
private int sleepTime = 0;
[AllowSet]
public int SleepTime
{
	get { return sleepTime; }
	set
	{
        // При установке 0 сначала засчитывается последний час сна
		if (value == 0 && sleepTime > 0) { Sleep(); }
        // Значения > 4 начинают новый 4-часовой цикл
		if (value > 4) { value = 1; }
		sleepTime = value;
		if (sleepTime > 0) { Sleep(); }
	}
}
private void Sleep()
{
	switch (sleepTime)
	{
		case 1: Fatigue -= 1; break;
		case 2: Fatigue -= 2; break;
		case 3: Fatigue -= 2; break;
		case 4: Fatigue -= 1; break;
	}
}
```

## InitData() и InitDataCustom()

Эти два метода во Flow, которые можно переопределить. Их вызов обеспечивается при выполнении Unity-метода _Awake()_. Метод InitData() генерируется препроцессором, а метод InitDataCustom() может в ручную использовать разработчик для обеспечения своей дополнительной логики инициализации. Для сгенерированного InitData() есть две цели. Первая связана с созданием словарей, списков и прочих коллекций (LDictionary, LList, LQueue), которые должны быть сохранены в базе данных. Такие коллекции нельзя инициализировать раньше _Awake()_, т.к. контекст к базе данных EntityFramefork`а еще не знает, нужно ли его использования. Если нет коллекции ведут себя буквально аналогично соответствующим им (Dictionary, List, Queue) в памяти. Вторая цель связана с инициализацией делегатов (см. раздел "Использование делегатов").

```csharp
public class CompanyLogic : AgentPointLogic
{
	public Capital Capital;
	public LDictionary<string, Product> Storage;

	public void AddAllJobRequirements()
	{
	  AddJobRequirements(JobType.Worker, new NamedValue("Mathematics", 1), new NamedValue("Logics", 1));
	  AddJobRequirements(JobType.Agronomist, new NamedValue("Biology", 2), new NamedValue("Chemistry", 1));
      ...
	}
    private void AddJobRequirements(JobType argJobType, params NamedValue[] argSkill)
	{
		JobRequirements requirements = new JobRequirements();
		requirements.JobType = argJobType;
		for (int i = 0; i < argSkill.Length; i++)
		{
			requirements.AddRequirements(argSkill[i].Name, argSkill[i].Value);
		}
		JobRequirements.Add(requirements.JobType, requirements);
	}
}
public class Company : AgentPoint
{
  // Генерируется
  public override void InitData()
  {
    base.InitData();
    logic.Storage = new LDictionary<string, Product>();
  }
  // Пишет разработчик в ручную
  public override void InitDataCustom()
  {
    base.InitDataCustom();
	logic.Capital = new Capital();
	logic.AddAllJobRequirements();
  }
}
```

## Использование делегатов

Делегаты используются в логике (Logic), чтобы управлять выполнением методов во Flow. В зависимости от модификатора доступа будет сгенерирован разный код. Для _public_ делегата, так же как с полями будет просто предоставлен доступ во вне Flow, чтобы кто угодно мог бы потом назначить нужный метод. Для _internal_ делегата будет выполнен соответствующий поиск подходящего метода (совпадает сигнатура и название) во Flow. И если такой метод будет создан, он будет проинициализирован в методе _InitData()_ (). Если же метод не будет найден, будет создана заготовка-пустышка (stub), и так же проинициализирована в методе _InitData()_. 

```csharp

```



