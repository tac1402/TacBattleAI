---
title: EntityType
---
Компонент [ItemCreate](ItemCreate) требует заполнение списка видов сущностей, чтобы разделить их в сцене на соответствующие уровни.

```csharp
namespace Tac.ItemCreate
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
