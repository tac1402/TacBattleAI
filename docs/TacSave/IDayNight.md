---
title: IDayNight
---

Используется UI управления сохранениями игр (см. [SaveCatalog](../../TacSave/SaveCatalog)) для формирования отметки игрового времени в метаданных файла сохранения.

```csharp
public interface IDayNight
{
	public string GameTime { get; set; } // Текстовое отображение текущие время
	public string GameDays { get; set; } // Текстовое отображение текущий номер суток
}
```

В простом случае, когда у Вас слабо выраженное течение времени в игре, можно вообще не использовать реализацию класса DayNight, а в самом классе World реализовать IDayNight. Например: 

```csharp
public class World : MonoBehaviour, IDayNight
{
	public int year = 0;
	public int quarter = 0;

	public string GameTime
	{
		get { return year.ToString(); }
		set { year = int.Parse(value); }
	}
	public string GameDays
	{
		get { return quarter.ToString(); }
		set { quarter = int.Parse(value); }
	}
}
```

Но, а в более сложном случае, рекомендуется использовать реализацию класса DayNight. 

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/SaveCatalog_DayNight_SaveManager.jpg)

На диаграмме видно, что сущность [DayNight](../../TasStandartU/DayNight) технически разделена на несколько классов. С одной стороны это сделано, чтобы обойти ограничения Юнити, с другой чтобы была возможность расширить класс Tac.DayNigh из системы сохарений Tac.DConvertor используя т.н. [линковку](../linking). Ограничение Юнити состоит в том, что на сцену в геймобъект нельзя добавить компонент, если он реализует интерфейс. Для этого выделяется родитель с техническим именем __DayNight0__. Тогда линкуясь к родителю __DayNight0__ из компонента Tac.DConvertor можно реализовать интерфейс IDayNight (выделенно ярко желтым).
