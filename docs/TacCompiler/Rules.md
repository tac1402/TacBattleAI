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

Эти два метода во Flow, которые можно переопределить. Их вызов обеспечивается при выполнении Unity-метода _Awake()_. Метод _InitData()_ генерируется препроцессором, а метод _InitDataCustom()_ может в ручную использовать разработчик для обеспечения своей дополнительной логики инициализации. Для сгенерированного _InitData()_ есть две основные задачи. 

Первая связана с созданием словарей, списков и прочих коллекций (LDictionary, LList, LQueue), которые должны быть сохранены в базе данных. Такие коллекции намеренно не создаются непосредственно при конструировании Logic. На момент создания объекта ещё неизвестно, будет ли конкретный экземпляр работать с базой данных Entity Framework или только в оперативной памяти. Поэтому создание коллекции откладывается до Awake(), когда контекст среды уже определён. Это позволяет одной и той же Logic работать в двух режимах. При наличии соответствующего контекста коллекция может быть связана с механизмом хранения и ORM. При отсутствии такого контекста она функционирует как обычная коллекция в памяти, аналогичная соответствующему Dictionary, List или Queue.

Вторая задача _InitData()_ связана с инициализацией внутренних делегатов Logic. TacCompiler анализирует объявления делегатов и устанавливает соответствия между требуемыми Logic возможностями и методами Flow. Это позволяет связать уже созданную Logic с конкретной реализацией среды непосредственно в процессе инициализации. (см. раздел "__Использование делегатов__").

_InitDataCustom()_ выполняется после сгенерированной инициализации и предназначен для действий, которые невозможно или нецелесообразно генерировать автоматически. Например, CompanyLogic может объявлять капитал, хранилище продукции и набор требований к профессиям. Создание Storage связано с хранение данных и поэтому попадает в генерируемый _InitData()_. Напротив, создание Capital и заполнение набора требований к профессиям являются предметной настройкой конкретной компании и выполняются разработчиком в _InitDataCustom()_.

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

Поэтому _InitData()_ — это не просто аналог Unity-метода _Start()_ или набор автоматически сгенерированных присваиваний. Это точка перехода Logic из декларативного состояния в состояние, связанное с конкретной средой исполнения.

## InternalAttribute и InitLogic()

Атрибут __[Internal]__, применённый к методу в Logic, указывает TacCompiler, что для данного метода не должна строиться публичная проекция во Flow. При этом сам метод может оставаться public. Такая возможность необходима в тех случаях, когда Logic и Flow находятся в разных пространствах имён или сборках и метод должен быть доступен Flow непосредственно, но при этом не должен становиться частью внешнего API Flow. Таким образом, __[Internal]__ и модификатор __internal__ решают разные задачи. Модификатор __internal__ является стандартным механизмом ограничения доступа C# и используется ППЛ, в частности, для внутренних делегатов (см. раздел "__Использование делегатов__"). __[Internal]__ является инструкцией TacCompiler, изменяющей правила проекции публичного API Logic во Flow.

Типичным примером является служебный метод __InitLogic()__. После создания экземпляра Logic через фабрику _CreateLogic()_ иногда необходимо выполнить дополнительную инициализацию самой Logic до того, как TacCompiler начнёт выполнять InitData(). Это особенно актуально в Unity, где компоненты MonoBehaviour имеют особый жизненный цикл и использование обычных конструкторов для их инициализации не является подходящим механизмом. Поэтому Logic предоставляет специальную точку инициализации InitLogic(). Метод может быть переопределён специализированной Logic, но благодаря [Internal] TacCompiler не проецирует его во Flow как обычный публичный метод Logic. В результате жизненный цикл Flow при выполнении Awake() имеет следующую последовательность: __CreateLogic → Logic.InitLogic → InitData → InitDataCustom__

__Внимание! Не используйте оригинальный Awake в наследника Flow.__ TacCompiler и базовый Flow используют Awake() для выполнения обязательной последовательности инициализации. Собственная реализация Awake() приведет к тому, что эта последовательность не будет выполнена.

```csharp
public abstract class Logic : IItemDb
{
	[Internal]
	public virtual void InitLogic() { }
}
public abstract class Flow : MonoBehaviour
{
	private Logic innerLogic;
	protected Logic baseLogic { get { return innerLogic; } set { innerLogic = value; } }
	protected virtual void CreateLogic() { }
	public virtual void InitData() { }
	public virtual void InitDataCustom() { }

	private void Awake()
	{
		CreateLogic();
		if (baseLogic != null)
		{
			baseLogic.InitLogic();
		}
		InitData(); 
		InitDataCustom();
	}
}
```

## Использование делегатов

Делегаты используются в Logic для обращения к функциональности Flow, когда необходимое действие не может или не должно быть реализовано непосредственно в Logic. В зависимости от модификатора доступа будет сгенерирован разный код. 

Для public делегата TacCompiler формирует публичный доступ к делегату во Flow. В этом случае Flow не обязан содержать метод с определённым именем. Внешний код может самостоятельно назначить необходимую реализацию делегата. Таким образом, публичный делегат является частью внешнего контракта Flow и может быть подключён любым потребителем, имеющим соответствующий доступ.

Для internal делегата применяется другой механизм. Такой делегат предназначен не для произвольного внешнего назначения, а для автоматического связывания Logic с её собственным Flow. TacCompiler анализирует внутренние делегаты Logic и ищет во Flow метод, соответствующий делегату по имени и сигнатуре. При обнаружении соответствующего метода TacCompiler автоматически формирует инициализацию делегата в InitData(). Таким образом, разработчику не требуется вручную связывать внутренний делегат с методом Flow. Достаточно объявить в Logic требуемую способность и реализовать соответствующий метод в Flow. Если подходящий метод не найден, TacCompiler создаёт диагностическую заготовку — stub — и также устанавливает её при инициализации. Это позволяет сохранить целостность сгенерированного кода и одновременно явно показать разработчику, что требуемая Logic возможность не имеет реализации в соответствующем Flow.

```csharp
public class AgentPointLogic : Logic
{
  public delegate bool IsAgentInEnterDelegate(int agentId);
  internal IsAgentInEnterDelegate IsAgentInEnter;

  private void CheckEnter(GameTime argGameTime, List<Agent> argAllAgent)
  {
    List<Agent> tmpAgents = argAllAgent.FindAll(x => x.TargetId == Id);
	for (int j = 0; j < tmpAgents.Count; j++)
	{
		if (IsAgentInEnter(tmpAgents[j].Id) == true)
		{
			WalkToEnter(argGameTime, tmpAgents[j]);
		}
	}
  }

  public delegate string GetInfoDelegate(int argId);
  public GetInfoDelegate GetInfoHandler;
  public string GetInfo()
  {
    string ret = "";
	if (GetInfoHandler != null)
	{
		ret += "\n" + GetInfoHandler(Id);
	}
	return ret;
  }
}

public class AgentPoint : Spatial
{
  public GetInfoDelegate GetInfoHandler { set => logic.GetInfoHandler = value; }

  public override void InitData()
  {
    base.InitData();
    logic.IsAgentInEnter = IsAgentInEnter;
  }
  /// Находится ли агент на входе
  public bool IsAgentInEnter(int argAgentId)
  {
    bool ret = false;
	Collider[] c = Physics.OverlapBox(Point.transform.position, EnterSize / 2f, Point.transform.rotation, AgentLayer);
	for (int j = 0; j < c.Length; j++)
	{
	  Agent agent = c[j].gameObject.GetComponent<Agent>();
  	  if (agent != null && agent.Id == argAgentId && agent.TargetId == Id)
	  { 
        ret = true; 
		break;
	  }
	}
    return ret;
  }
}

```

Таким образом, использование делегатов - это не просто callback в обычном смысле, это декларация в Logic требующая у своего Flow выполнения действия, находящегося за пределами Logic. А internal и public определяют два разных механизма предоставления этой способности.

__Важно! Для Unity.__ В Unity по умолчанию TacCompiler не получает доступа к internal членам анализируемых сборок. Поэтому, чтобы TacCompiler мог выполнить поиск реализаций внутренних делегатов, необходимо предоставить ему разрешение для каждой динамической библиотеки (.dll), в которой используются такие делегаты. Если используется стандартная сборка Unity (__Assembly-CSharp__), указанное ниже объявление может располагаться в любом месте исходного кода. Если сборка определена через .asmdef, объявление должно находиться в этой же сборке.


```csharp
// AssemblyInfo.cs
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("TacCompiler")]
```

## Разделение проекта и OnlyUnity + OnlyLogic

ППЛ не требует заранее создать две независимые версии проекта. Сначала проект строится как единый объектный мир, затем TacCompiler и архитектурные правила локализуют зависимости, и только после этого условная компиляция позволяет удалить оставшиеся Unity-специфичные фрагменты. То есть переносимость получается не за счёт того, что разработчик с самого начала пишет «кроссплатформенный код», а за счёт того, что граница среды становится видимой и проверяемой.

Не разделяйте пакет на отдельные физические части: файлы исходного кода в одном пакете, файлы без зависимостей юнити в другом пакете. Складывайте Flow (или Spatial) рядом с Logic в той же директории. Причины этого изложены в разделе "Чистая логика". У вас уже почти всё готово к окончательном разделению: 
1. Если у вас есть объект Logic без парного ему Flow (например, Capital, Product или Deal) соберите их в отдельную директорию в рамках вашего пакета и пометьте его как YouPack.Logic, например, __Society.Logic__
2. У вас могут быть исходные классы вообще вне парадигмы SFL, т.е. полностью специфичные под Unity. Например, свзяанyые с UI, анимацией персонажа, нахождением пути и много другое. Соберите их отдельно и пометьте YouPack.Unity, например, __UI.Logic__ или даже на вернем уровне __Code.Unity__.
3. Там где у вас есть парные Flow+Logic соберите вместе, и не давайте специфичных название, просто оставьте YouPack, например, __Code__ или __Agent__ .
3.1. В Logic у вас просто не может быть зависимости от Юнити (или другой среды).
3.2. __!__ Все внимание теперь направленно на Flow, в нем технически могут быть _public_ и _internal_ члены доступ к которым возможен _"через один этаж"_, например, из SocietyLogic к Person.CancelTarget(). При это метод CancelTarget() средо-специфичный и определён прямо в Person (иначе бы он был бы в PersonLogic).

Именно, в этих пограничных случаях, их сравнительно мало, но они есть, лучше всего применить условную компиляцию с символом препроцессора OnlyUnity. 

```csharp

#if OnlyUnity
using UnityEngine;
using UnityEngine.AI;
#endif

public partial class Agent : Spatial
{
	public void CancelTarget()
	{
#if OnlyUnity // Все свойства пространственно специфичны
		agent.isStopped = true;
		TargetPoint = Vector3_.zero;
		walkDistance = 0;
		currentPathIndex = 0;
		PathPoints.Clear();
		PathStatus = 0;
#endif
	}
}
```

В ряде случаев, требуется не только оставить общую сигнатуру метода, а написать ему замену, чтобы сохранить рабочий код, который можно будет запустить в тесте, например, под WinForms.

```csharp
public class Company : AgentPoint
{
#if OnlyUnity
	/// Если машина на входе - удалить
	public bool RemoveTruckInEnter(int argTruckId)
	{
		bool ret = false;
		Collider[] c = Physics.OverlapBox(Point.transform.position, EnterSize / 2f, Point.transform.rotation, AgentLayer);
		for (int j = 0; j < c.Length; j++)
		{
				Truck truck = c[j].gameObject.GetComponent<Truck>();
				if (truck != null && truck.Id == argTruckId && truck.TargetId == Id)
				{
					Destroy(truck.gameObject);
					ret = true;
					break;
				}
			}
		}
		return ret;
	}
#endif
#if OnlyLogic
	// Если машина на входе - удалить
	public bool RemoveTruckInEnter(int argTruckId)
	{
		return true;
	}
#endif
}
```

__После такой финальной чистки вы сможете скопировать ваш пакет вне Юнити, под управление например, консоли и WinForms и протестировать или просто запустить вашу игру в другом окружении.__

### Организации условной компиляции в сложном случае

Хотя в простых случаях допустимо вставлять блоки #if прямо внутрь методов, при разрастании числа таких вставок навигация по файлу становится затруднительной. Чтобы сохранить читаемость, можно воспользоваться механизмом частичных классов и разнести средозависимые реализации по отдельным файлам, помечая каждый из них директивой на весь файл. Это позволяет отделить Unity‑специфичные методы от логических заглушек, не смешивая их визуально в одном документе.

Основной файл класса остаётся общим и содержит генерируемую TacCompiler «обвязку», а в дополнении к нему создаются файлы с суффиксами .Unity и .Logic. Такой подход даёт чёткую границу: в одном файле гарантированно нет ссылок на UnityEngine, в другом — отсутствуют бизнес‑правила, которые должны оставаться в отдельном классе Logic. Это упрощает ревью кода, снижает риск конфликтов при слиянии веток и облегчает настройку сборки под разные конфигурации.

Вместе с тем переход к частичным классам стоит рассматривать как эволюционный шаг, а не обязательное требование. Если количество средозависимых методов невелико, проще оставить локальные блоки #if — это избавляет от избыточной файловой структуры. К частичному разделению прибегают, когда таких методов становится больше пяти‑семи, или когда требуется полностью альтернативная реализация для другой платформы. В любом случае условная компиляция остаётся лишь вспомогательным инструментом, а не краеугольным камнем архитектуры, и при необходимости её легко заменить на более формальные паттерны вроде адаптера или стратегии.

