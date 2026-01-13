---
title: EntityType
---
Описание видов моделей/сущностей в игре, которые разделены на сцене по различным уровням (дочерним gameObject у корневого элемента World).

Базовая функциональность реализована компонентом [ItemCreate](../ItemCreate), а так же используется системой сохранения в через интерфейс [IObject](../../TacStandartU/IObject).

```csharp
namespace Tac
{
	[Serializable]
	public class EntityType
	{
		public string Name; // Название уровня
		public int Id; // Идентификатор уровня (соответствует Item.GroupId)
		public GameObject Level; // Уровень иерархии, как правило дочерний элемента объекта World, в который будут помещаться все объекты этого типа
	}
}
```
