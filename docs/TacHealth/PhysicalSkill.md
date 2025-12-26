---
title: PhysicalSkill
---

Умение (физическое, связанное с телом агента).

## Конструктор и float MinState/MaxState

При создании объекта можно задать минимальное и максимальное число отражающие величину умения. По умолчанию, MinState = 0 и MaxState = 100.

## float State

Текущий уровень умения

## float DependencyState

Штрафы на умение, например, вызванные здоровьем 

## float ComplexState

Итоговый уровень умения, учитывая штрафы (= State - DependencyState)

## Recalc
_Recalc(float argHealthState)_

Пересчитать штраф в зависимости от состояни здоровья (= (State / (argHealthState / 100f)) - State)



