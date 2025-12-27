---
title: AgentLogic
---

## int TargetId

Место назначения (куда идти) = идентификатору ObjectId или =0 если не назначено.

## bool IsBusy

Занят ли агент? (устанавливается если находится в какой то точке LocatedId не равен 0).

## int LocatedId

Где находится агент? = идентификатору ObjectId или =0 если в пути.

## bool UseHealthState

По умолчанию = false и используется простое итоговое значение здоровья health. Если = true используется система здоровья [HealthState](../../TacHealth/HealthState).

## bool IsDead

Мертв ли агент, аналогично Health == 0. 

## PhysicalSkill Charge

Заряд/Выносливость 

## PhysicalSkill Precision

Меткость

## ApplyDamage()
_ApplyDamage(float argDamage)_ 

Нанести удар по агенту в случайную часть тела с разрушением =argDamage.

_ApplyDamage(BodyParts argBodyPart, float argDamage)_

Нанести удар по агенту в определенную часть тела =argBodyPart с разрушением =argDamage.




