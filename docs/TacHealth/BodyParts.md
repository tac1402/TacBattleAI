---
title: BodyParts
---

# Части тела

Перечисление частей тела, может использоваться для дифференциации при нанесении урона, лечения и .т.д.

```csharp

	public enum BodyParts
	{
		Head = 1, //  Голова
		Thorax = 2, //  Грудь
		Abdomen = 3, //  Живот
		ThighRight = 4, // Бедро правое
		ThighLeft = 5, // Бедро левое
		ShoulderRight = 6, // Плечо правое
		ShoulderLeft = 7, // Плечо левое
		ShinRight = 8, // Голень правая
		ShinLeft = 9, // Голень левая
		ForearmRight = 10, // Предплечье правое
		ForearmLeft = 11 // Предплечье левое
	}
```

# Dependency

Зависимость одной части тела от другой. 

## Конструктор Dependency
_Dependency(BodyPartState argDependencyPart, float argDependencyKoef)_

Задает коэффициенты влияния (argDependencyKoef) для зависимой части тела (argDependencyPart).

## float GetDependency()

Возвращает величину зависимости = 100 - (float)(Math.Pow(DependencyPart.ComplexState, DependencyKoef) / Math.Pow(100, DependencyKoef - 1))

# Состояние части тела (класс BodyPartState)

## List<Dependency> Dependency

Описывает влияние зависимых частей тела на эту часть тела

## List<VitalSystemState> SystemDependency

Описывает влияние жизненно важных систем на часть тела

## State

Текущие состояние. При значении меньше нуля, начинается автоматическая деградация всех зависимых жизненно важных органов/систем.

## float Koef

Коэфициент влияния на итоговое состояние здоровья. В поведении по умолчанию, считается что 50% здоровья всех частей тела суммарно указывают на повреждения.

## AddBodyDependency()
_AddBodyDependency(BodyPartState argDependencyPart, float argDependencyKoef)_

Добавить к состоянию зависимые части тела с определенными коэффициентами. Например, попадание в бедро, влияет на функциональность голени. 

## AddSystemDependency()
_AddSystemDependency(VitalSystemState argVitalSystem)_

Добавить зависимую жизненно важную систему. Например, попадание в грудь влияет на дыхательную систему.

## float Injury()

Рассчитывает взвешенное значение повреждения, учитывая взаимосвязь систем

