---
title: SaveCatalog
---

Вместе с классом __SaveCatalog__ в пакете компонента TacSave предоставляется префаб (с тем же названием) для пользовательского интерфейса (UI) обеспечивающий сохранение игр. Внешний вид предоставлен ниже и обеспечивает разделение сохранений по директориям (прохождениям), позволяет сохранить и загрузить соответствующие файлы данных. Вызывается нажатием F5. 

Префаб __SaveCatalog__ должен быть помещен на сцену под элемент с Canvas в скрытом виде. Так же нужно поместить префаб __DialogYouSure__ и перетянуть ссылку на него в класс __SaveCatalog__ в поле __DialogYouSure__. Все остальные поля настроены по умолчанию.

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/SavePanel.jpg)

Для реализации функциональности использует интерфейсы [IDayNight](../../TacSave/IDayNight) и [ISaveManager](../../TacSave/ISaveManager) (см. на диаграмме).

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/SaveCatalog_DayNight_SaveManager.jpg)

Во время инициализации в компоненте __World__ , нужно предоставить эти интерфейсы. Ниже пример кода, который это реализует. 

```csharp
		DayNight = GetComponent<DayNight>();
        ...
		GameObject ui = GameObject.Find("UI");
		if (ui != null)
		{
			SaveCatalog = ui.GetComponentInChildren<SaveCatalog>(true);
			SaveManager saveManager = GetComponent<SaveManager>();
			saveManager.World = this;
			SaveCatalog.ISaveManager = saveManager;
			SaveCatalog.IDayNight = DayNight as IDayNight;
		}
```

Класс [SaveManager](../../) вам нужно реализовать самостоятельно, наследуясь от [SaveManager0](../../TacSave/SaveManage0). Эта реализация позволит вам полностью контролировать сохранение данных и удаление/востановление объектов в сцене.
