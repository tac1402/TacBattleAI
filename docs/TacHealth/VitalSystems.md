---
title: VitalSystems
---

# Жизненно важные системы

```csharp
	public enum VitalSystems
	{
		Cerebration = 1, // Мозговая деятельность
		Сirculatory = 2, // Кровеносная система
		Respiratory = 3, // Дыхательная система
		Digestive = 4, // Пищевая система
		Immune = 5 // Иммунная система
	}
```

# Состояние жизненно важной системы (класс VitalSystemState)

## float State

Текущие состояние системы от 0 до 100% (0 - полностью не функционирует).

## Конструктор 
_VitalSystemState(int argState, int argSpeedDegradation, int argSpeedRegeneration, int argMaxForceDegradation, int argMaxForceRegeneration, int argLevelIrreversibleChange)_

При создании нужно задать: 

1. argState - текущие состояние

2. argSpeedDegradation - Скорость деградации - количество секунд, через которое будет происходить уменьшение состояния при повреждении

3. argSpeedRegeneration - Скорость регенерации - количество секунд, через которое будет происходить восстановление состояния при повреждении

4. argMaxForceDegradation - Максимальная величина деградации

5. argMaxForceRegeneration - Максимальная величина восстановления

6. argLevelIrreversibleChange - Уровень необратимого (без лечения) изменения

## Degradation()

Один шаг деградации - случайным образом выбирается величина до MaxForceDegradation и уменьшает состояние

## Regeneration()

Один шаг регенерации - случайным образом выбирается величина до MaxForceRegeneration и увеличивает состояние

## AutoRegeneration()

Один шаг автоматической регенерации, происходит если состояние системы больше уровня необратимого изменения (LevelIrreversibleChange)

