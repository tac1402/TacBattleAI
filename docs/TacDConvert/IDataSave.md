---
title: IDataSave
---

Интерфейс _IDataSave_ реализуется (и [линкуется](../linking)) для класса [Item](../../TacStandartU/Item) компонентом сохранения __TacSave__ с поведением по умолчанию. Представляет собой управление набором данных, которые нужно сохранить/загрузить. Помещается в очередь _Data_ с помощью реализованных по умолчанию методов SaveQ. Поддерживает сохранение списков (List<T>), словарей (Dictionary<int/string,T>), списков вложенных в словарь (Dictionary<int/string,List<T>>), классов T. В качестве свойств классов поддерживаются только наиболее используемые примитивные типы: string, int, float, bool. 

Для очередей (Queue) в [Item](../../TacStandartU/Item) реализован специальный метод SaveQQ, который во время загрузки востанавливает очередь как список, затем конвертируя в очередь.

Переопределяя метод __SaveData__ наследники [Item](../../TacStandartU/Item) могут управлять записью своих свойств и агрегированных ими свойств.

```csharp

public abstract partial class Item : IDataSave
{
    ... // реализация IDataSave
		public virtual void SaveData(bool argLoadMode) { ... }
		protected T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)  { ... }
		protected T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, string argTag = null)  { ... }
		protected Queue<K> SaveQQ<K>(Queue<K> propertyValue, Expression<Func<Queue<K>>> propertyLambda, string argTag = null)  { ... }
}

public interface IDataSave : IPrefabId
{
    Queue<ObjectInfo> Data { get; }
    string DataTag { get; set; }
    string ClassName { get; set; }
    List<string> PropertyName { get; set; }
		bool IsLoad { get; set; }

    T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
}
public interface IPrefabId : IId { public string PrefabName { get; set; } }
public interface IId { public int Id { get; set; } }

public enum PredefinedTag
{
    OnlyPrefabId = 1, // сохранять только идентификацию
		UseCurrent = 2,   // установить ListCreateMode.UseCurrent для списков вложенных свойств
		UseCurrentPrefabId = 3 // попробовать получить объект по ссылке, и только если он не создан создать из префаба
}
public enum ListCreateMode
{ 
    Recreate = 1,    // Список очистить .Clear(), а объект создать с помощью дефолтного new()
    UseCurrent = 2,  // Новые объекты не создавать, заменить значения свойств
    CreateFromPrefab = 3 // Тоже самое, что Recreate, только добавить как компонент в gameobject, который создать из префаба
}

```


