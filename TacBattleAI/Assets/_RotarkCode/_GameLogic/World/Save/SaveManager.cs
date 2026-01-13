
using System;
using System.Collections.Generic;
using System.Linq;
using Tac;
using Tac.DConvert;
using Tac.Person;
using Tac.Society;

public class SaveManager : SaveManager0
{
	public World World;


	private void SetProtocolSave()
	{
		dConvert.Clear();

		People = World.Society.People.Values.ToList();
		dConvert.Set(People, ListCreateMode.CreateFromPrefab);

		dConvert.Set(World, ListCreateMode.UseCurrent);

		//dConvert.Set(World.Society.People, ListCreateMode.CreateFromPrefab);

		/*
		dConvert.Set(PersonList, ListCreateMode.CreateFromPrefab);
		dConvert.Set(AgentPointList, ListCreateMode.CreateFromPrefab);
		dConvert.Set(World, ListCreateMode.UseCurrent);
		*/
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
	private List<Person> People = new List<Person>();

	private void ClearMainList()
	{
		People = null;
	}

	private void SetProtocolLoad()
	{
		dConvert.Clear();
		dConvert.AddExternalAssembly("TacDConvertor");

		dConvert.Set(People, ListCreateMode.CreateFromPrefab);

		//dConvert.Set(AgentPointList, ListCreateMode.CreateFromPrefab);

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
