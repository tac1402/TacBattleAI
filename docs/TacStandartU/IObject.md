---
title: IObject
---

Реализация этого интерфейса позволяет управлять созданием/удалением в сцене юнити gameObject на основании префабов (моделей). Полноценно реализовано компонентом [ItemCreate](../../TacItemCreate/ItemCreate). Также используется системой сохранений. 

```csharp
	public interface IObject
	{
		int ObjectIdCounter { get; set; } // Счетчик идентификатов объектов
		int PredeffinedObjectId { get; set; } // CreateObject должен поддерживать создание объекта с наперед заданным идентификатором. Если он =0, используется приращение счетчика ObjectIdCounter
		
		List<EntityType> WorldLevel { get; } // Описаниие уровней в мире см. [EntityType](../../TacStandartU/EntityType)

		GameObject CreateObject(string argModelName); // Создать и возвратит GameObject по названию модели argModelName
		void DestroyObject(GameObject argObject); // Удалить GameObject со сцены
	}
```
