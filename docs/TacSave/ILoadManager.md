---
title: ILoadManager
---



```csharp
public interface ILoadManager
{
    // Должен предоставить интерфейс IObject, например его реализует компонент ItemCreate
		IObject IObject { get;}

		/// Все объекты в игре с уникальными Id
		Dictionary<int, GameObject> AllObject { get; }

		// Сбрасывает игру до начального состояния перед загрузкой сохранения
    // Как правило реализуется в корне игры классе World
    // Нет реализации по умолчанию
		public void ResetGame();

		/// Восстанавливает производные данные и все процессы после загрузки сохранения
    // Как правило реализуется в корне игры классе World
    // Нет реализации по умолчанию
		public void RecoverGame();

    #region default 
    // Создать объект с идентификатором argObjectId из префаба с названием argModelName     
    // Есть реализация по умолчанию
		public GameObject CreatePrefab(int argObjectId, string argModelName) { ...	}

		// Добавить объект argObject с уникальным идентификатором argObjectId
    // Есть реализация по умолчанию
		public void AddObject(int argObjectId, GameObject argObject) { ... }

		/// Получить объект по уникальному идентификатору argId
    // Есть реализация по умолчанию
		public GameObject GetObject(int argId) { ... }

    // Минимальный сброс текущей игры, например, вызывает DestroyWorld()
    // Есть реализация по умолчанию
		public void ResetGameInner() { ... }

    // Уничтожение текущего мира - удаляет все объект в уровнях мира IObject.WorldLevel
    // Есть реализация по умолчанию 
		public void DestroyWorld() { ... }

    // Сбросить события, т.е. удалить все подписки на событие argEventName у объекта 
    // Есть реализация по умолчанию
		public void ResetEvent(object argObject, string argEventName) { ... }
    #endregion
}
```


