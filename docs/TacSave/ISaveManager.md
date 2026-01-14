---
title: ISaveManager
---

Используется UI управления сохранениями игр (см. [SaveCatalog](../../TacSave/SaveCatalog)) для непосредственного сохранения (создания файла данных с состоянием игры на определенное игровое время) и загрузки (чтение файла данных и восстановления игры в сцене Unity 3D). Теоретически можно реализовать любую реализацию сохранений, но компонент TacSave предоставляет класс-заготовку (см. [SaveManager0](../../TacSave/SaveManager0)), наследуясь от которой будет проще программного управлять ходом сохранения и подключить систему "прямых конвертаций" ([DirectConvert](../../TacDConvert/DConvert)). Подробнее см. [SaveManager](../../TacSave/TacManager), его роль среди других классов см. ниже на диаграмме выделен зеленным цветом.

```csharp
  public interface ISaveManager
	{
		public string Version { get; set; } // Версия способа сохранения
		public string SaveRootDir { get; } // Корневая директория для сохранений
		public event Change LoadError; // Событие возникающие, если во время сохранения/загрузки произошла ошибка

		public void Save(string argDirName, string argFileName); // Сохранить в поддериктории argDirName с именем файла данных argFileName
		public void Load(string argDirName, string argFileName); // Загрузить из поддериктории argDirName с именем файла данных argFileName
	}
```

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/SaveCatalog_DayNight_SaveManager.jpg)

