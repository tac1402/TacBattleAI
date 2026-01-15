---
title: ILoadManager
---

Для обработки процесса загрузки сохранения игры [SaveManager0](../../TacSave/SaveManager0) требует реализацию в наследнике [SaveManager](../../TacSave/SaveManager) этого интерфейса _ILoadManager_, но не прямо, а посредством реализации его в классе World. Ниже показан сам интерфейс, и пример его реализации в World.

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

```csharp
public partial class World : Item, ILoadManager
{
	#region ILoadManager
	// Здесь будут хранится все объекты добавленные в процессе загрузки из файла сохранения
    // и этот словарь будет использоваться для обмена ссылками на соответтвующие объекты по универсальному идентификатору (ключу словаря)
	private Dictionary<int, GameObject> allObject = new Dictionary<int, GameObject>();
	public Dictionary<int, GameObject> AllObject { get { return allObject; } }
    // Как правило World изначально использует компонент ItemCreate, поэтому реализацию интерфейса IObject можно предоставить ему
	public IObject IObject { get { return ItemCreate; } }

    // Будет вызвано перед загрузкой из файла сохранеия, для сброса текущей игры,
    // используйте реализацию по умолчанию ResetGameInner() чтобы удалить объекты из уровней игры, расположенных в сцене и сбросьте
    // все события с помощью ResetEvent, на которые могли быть подписаны объекты прошлой игры, которые не пересоздаются, например, DayNight
	public void ResetGame()
	{
		(this as ILoadManager).ResetGameInner();
		(this as ILoadManager).ResetEvent(DayNight, "NextDay");
		(this as ILoadManager).ResetEvent(DayNight, "NextHour");
        ...
	}

    // Будет вызвано после загрузки из файла сохранения, для востановления записанной игры с прерванного момента,
    // Используйте для повторной инициализации объектов, которые этого требует (их метод Start после загрузки вызван не будет),
    // переподпишитесь на нужные события, обновите NavMesh в соответствии с изменениями в сцене, перезапустите, если требуется короутины,
	public void RecoverGame()
	{
		RunPanel.Init(DayNight, Society);
		DayNight.NextHour += AgentWalkEmulation;

		UpdateSurface();
        ...
	}
	#endregion
}
```




