---
title: PersonLogic
---

Более высокая абстракция "Персонажа", наследуемая от агента [Agent](../../TacAgent/Agent). Отличается от агента наличием таких свойств как статы, скилы, точки/места интереса. Здесь эти свойства обезличины, но в своем проекте рекоммендуется разименовать их, например так:  

```csharp
		public float Money
		{
			get { return Stats["Money"]; }
			set { Change("Money", value); }
		}
		public float Experience
		{
			get { return Skills["Experience"]; }
			set { Change("Experience", value); }
		}
		public AgentPoint WorkPlace
		{
			get { return (GetPlace("Work")); }
			set { SetPlace("Work", value); }
		}
```

Так же смотрите [как слинковать](../linking) моего персонажа с TacLibrary.

## Gender
_GenderType Gender_

Пол персонажа.

```csharp
public enum GenderType
{
	Unknow = 0, // Не известен
	Men = 1, // Мужской
	Women = 2, // Женский
}
```

## Stats
_Dictionary<string, float\> Stats_

Т.н. статы персонажа, любые характеристики персонажа, значения которых можно выразить числом типа float. Каждая характеристика имеет свое уникальное текстовое (string) наименование. 

## Skills
_Dictionary<string, float\> Skills_

Т.н. скилы персонажа, любые умения персонажа, значения которых можно выразить числом типа float. Каждое умение имеет свое уникальное текстовое (string) наименование. 

_Различие между статами и скилами скорее просто стилистическое, но они разделены. Если стат отвечает на вопрос "Что это дает персонажу?" (существительное), то скил отвечает на вопрос "Что персонаж умеет?" (глагол)._

## AddStat
_AddStat(string argName, float argValue = 0, bool argAddInfo = true)_

Добавить стат с именем argName и значением argValue (по умолчанию =0). По умолчанию (argAddInfo = true) добавляется в список для отображения в UI.

## AddSkill
_AddSkill(string argName, float argValue = 0, bool argAddInfo = true)_

Добавить скилл с именем argName и значением argValue (по умолчанию =0). По умолчанию (argAddInfo = true) добавляется в список для отображения в UI.

## Info
_List<NamedValue\> Info_

Проименованные значения статов, скилов и других значений для отображения в UI

## InfoTxt
_string InfoTxt_ (Read Only)

Полная информация о статах, скилах и других значений из списка _Info_ , разбитая на строки для UI

## Change()
_Change(string argName, float argValue)_

Заменить значение стата или скила с именем _argName_ новым значением _argValue_ с обновлением об этом информации _Info_

## OnChangeInfo
_event Change OnChangeInfo;_

Событие на которое подписываются панели UI для синхронного отображения изменений статов или скилов персонажа.

## Places
_Dictionary<string, AgentPoint\> Places_

Места интереса персонажа. Как правило те места, которые он может посещать и из которых по той или иной логике строится его [маршрут/план](../../TacPerson/PersonPlan). Каждое место/точка интереса имеет свое уникальное текстовое (string) наименование. 

## SetPlace()
_SetPlace(string argKey, AgentPoint argPlace)_

Установить место с определенным наименованием. Если наименование встречается впервые будет добавленно новое место в _Places_, иначе будет заменено. 

## GetPlace()
_AgentPoint GetPlace(string argKey)_

Получить место по наименованию.

## WorkPlace
Предустановленное место для работы с наименованием _Work_ в списке _Places_.

## ResidencePlace
Предустановленное место жительства с наименованием _Residence_ в списке _Places_.
 






