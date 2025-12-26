---
title: HealthState
---

# HealthState

Класс описывающий подробно состояние здоровья. 

## float Health

Итоговое значение здоровья

## Dictionary<BodyParts, BodyPartState> Body

Список всех частей тела и их состояний (см. [BodyParts](../../BodyParts)).

## Dictionary<VitalSystems, VitalSystemState> System 

Список всех жизненно важных систем и их состояний (см. [VitalSystems](../../VitalSystems)).

## SetBodyParts() и SetVitalSystems()

Методы, которые по умолчанию описывают все части тела и жизненно важные органы/системы агента. Могут быть переопределенны в наследнике.

## CalcHealth()

Расчет итогового значения здоровья, основываясь на состояние частей тела и жизненно важных органов/систем. Может быть переопределенно в наследнике.

