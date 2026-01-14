---
title: SaveManager
---

Ниже приведен полный пример реализации для тестовой сцены города Ротарк. Важно понимать, что сохранение и загрузка, это сложный процесс, который не ограничивается управлением записи/загрузки классом SaveManager, но это корень этого управления. В нем вы описываете какие данные вы будете записывать/загружать и в каком порядке (см. SetProtocolSave()/SetProtocolLoad()). Ссылку на себя предосавляет World во время инициализации (см. пример в [SaveCatalog](../../TacSave/SaveCatalog)). Он же реализует интерфейс _ILoadManager_, который здесь предоставляется через _ILoadGet()_ (см. [ILoadManager](../../TacSave/ILoadManager)). Методы SaveBin() и LoadBin() запускают непосредственно сохранение или загрузку через прямые конвертации (систему DirectConvert).

```csharp
public class SaveManager : SaveManager0
{
	public World World; 

	protected override ILoadManager ILoadGet()
	{
		return World as ILoadManager;
	}

	private void SetProtocolSave()
	{
		dConvert.Clear();

		AllAgent = World.Society.People.Values.ToList();
		dConvert.Set(AllAgent);
		AllAgentPoint = World.Society.AllAgentPoint;
		dConvert.Set(AllAgentPoint);

		dConvert.Set(World);
	}

	protected override void SaveBin(string argDirName, string argFileName)
	{
		SetProtocolSave();
        // Выполнение сохранения системой DCovert
		dConvert.Save(argDirName + "\\" + argFileName + ".bin", ConvertorType.Text);
	}

	private List<Person> AllAgent = new List<Person>();
	private List<AgentPoint> AllAgentPoint = new List<AgentPoint>();
	private void ClearMainList()
	{
		AllAgent = null;
		AllAgentPoint = null;
	}

	private void SetProtocolLoad()
	{
		dConvert.Clear();

		//Сами агенты могу не находится на сцене, поэтому во время ResetGame все агенты будут удалены, и заново созданы из префабов 
		dConvert.Set(AllAgent, ListCreateMode.CreateFromPrefab);

		//Точки агентов уже находятся на сцене, поэтому их не создаем из префаба, а заполняем измененные свойства
		AllAgentPoint = World.Society.AllAgentPoint;
		for (int i = 0; i < AllAgentPoint.Count; i++)
		{
			ILoadGet().AddObject(AllAgentPoint[i].ObjectId, AllAgentPoint[i].gameObject);
		}
		dConvert.Set(AllAgentPoint, ListCreateMode.UseCurrent);

		dConvert.Set(World, ListCreateMode.UseCurrent);
	}

	protected override bool LoadBin(string argDirName, string argFileName)
	{
		bool isLoadError = false;
		SetProtocolLoad();
		try
		{
            // Выполнение загрузки системой DConvert
			dConvert.Load(argDirName + "\\" + argFileName + ".bin", ConvertorType.Text);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Load Error: " + ex.Message);
			isLoadError = true;
		}
		ClearMainList();
		return isLoadError;
	}
}
```

Списки всех агентов (AllAgent, см. [Person](../../TacPerson/Person)) и всех зданий, где они бывают (точек агентов) (AllAgentPoint, см. [AgentPoint](../../TacAgent/AgentPoint)) это типовой пример массовых игровых концепций, которые непосредственно находятся на сцене в виде префабов имея визуальное представление в игровой сцене. Рекоммендуется все такого рода игровые концепты находящиеся непосредственно на сцене, записывать предварительно списками. Тогда в производных классах, которые вам нужно будет сохранить, будут только ссылки на них. Поэтому уже имеющейся словарь World.Society.People (преобразовав в список) и список World.Society.AllAgentPoint вначале записываем с полным содержимым (см. код выше _SetProtocolSave()_). А потом, записывая объект World, свойства содержащие эти списки будут записаны используя только уникальные идентификаторы.



