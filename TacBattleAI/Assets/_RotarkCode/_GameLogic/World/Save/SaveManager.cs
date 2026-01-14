
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.Agent;
using Tac.DConvert;
using Tac.Person;

public class SaveManager : SaveManager0
{
	public World World;


	private void SetProtocolSave()
	{
		dConvert.Clear();

		AllAgent = World.Society.People.Values.ToList();
		dConvert.Set(AllAgent, ListCreateMode.CreateFromPrefab);
		AllAgentPoint = World.Society.AllAgentPoint;
		dConvert.Set(AllAgentPoint, ListCreateMode.CreateFromPrefab);


		dConvert.Set(World, ListCreateMode.UseCurrent);
	}


	protected override void SaveBin(string argDirName, string argFileName)
	{
		DateTime begin = DateTime.Now;
		SetProtocolSave();
		dConvert.Save(argDirName + "\\" + argFileName + ".bin", ConvertorType.Text);
		double t = (DateTime.Now - begin).TotalMilliseconds;
		UnityEngine.Debug.Log("Save: " + t.ToString() + " ms");
	}

	/// <summary>
	/// Только List можно использовать для предварительной загрузки объектов в сцену, создавая из префабов (ListCreateMode.CreateFromPrefab)
	/// (может быть пустым, но не null)
	/// </summary>
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
		dConvert.AddExternalAssembly("TacDConvertor");

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

	protected override ILoadManager ILoadGet()
	{
		return World as ILoadManager;
	}

	protected override bool LoadBin(string argDirName, string argFileName)
	{
		bool isLoadError = false;

		DateTime begin = DateTime.Now;
		SetProtocolLoad(); // Важно вызвать до загрузки метаданных (Meta.Init)
		dConvert.OnLoadStep += DConvert_OnLoadStep;

		try
		{
			dConvert.Load(argDirName + "\\" + argFileName + ".bin", ConvertorType.Text);
			double t = (DateTime.Now - begin).TotalMilliseconds;
			UnityEngine.Debug.Log("Load: " + t.ToString() + " ms");
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Load Error: " + ex.Message);
			string ErrorLast = dConvert.ErrorLoadInList;
			isLoadError = true;
		}

		ClearMainList();

		return isLoadError;
	}

	private void DConvert_OnLoadStep(int argIndex)
	{
		/*if (argIndex == 0)
		{
		}*/
	}





}
