---
title: Item
---

Любой объект в сцене, который визуально представлен отдельным префабом.

```csharp
namespace Tac
{
    public abstract class Item : MonoBehaviour
    {
		public int ObjectId; // Уникальный индентификатор объекта в мире
		public int GroupId = -1; // Группа объекта
		public string ModelName = ""; // Имя префаба
		public ModelTypes ModelType = ModelTypes.Model; // Тип модели
	}
}
```
