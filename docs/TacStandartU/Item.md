---
title: Item
---

Любой объект в сцене, который визуально представлен отдельным префабом.

```csharp
public abstract class Item : MonoBehaviour
{
	public int ObjectId; // Уникальный индентификатор объекта в мире
	public int GroupId = -1; // Группа объекта
	public string ModelName = ""; // Имя префаба
	public ModelTypes ModelType = ModelTypes.Model; // Тип модели
}
```

Перечисление видов моделей, настройте под свой проект: только первые два стандарты остальные удалите или добавьте.

```csharp
public enum ModelTypes
{
	None = -1, // Не имеет визуального представления
	Model = 0, // Обычная модель
}
```
