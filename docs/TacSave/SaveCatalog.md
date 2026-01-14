---
title: SaveCatalog
---

Вместе с классом __SaveCatalog__ в пакете компонента TacSave предоставляется префаб (с тем же названием) для пользовательского интерфейса (UI) обеспечивающий сохранение игр. Внешний вид предоставлен ниже и обеспечивает разделение сохранений по директориям (прохождениям), позволяет сохранить и загрузить соответствующие файлы данных. Вызывается нажатием F5.

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/SavePanel.jpg)

Для реализации функциональности использует интерфейсы [IDayNight](../../TacSave/IDayNight) и [ISaveManager](../../TacSave/ISaveManager) (см. на диаграмме).

![alt](https://tac1402.github.io/TacBattleAI/Diagramm/SaveCatalog_DayNight_SaveManager.jpg)


