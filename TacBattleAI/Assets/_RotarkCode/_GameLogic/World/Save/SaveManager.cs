
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
		dConvert.Set(World.Society.People.Values.ToList());
		dConvert.Set(World.Society.AllAgentPoint);
		dConvert.Set(World);
	}

	protected override void SaveBin(string argDirName, string argFileName)
	{
		DateTime begin = DateTime.Now;
		SetProtocolSave();
		dConvert.Save(argDirName + "\\" + argFileName + ".bin", ConvertorType.Text);
		double t = (DateTime.Now - begin).TotalMilliseconds;
		UnityEngine.Debug.Log("Save: " + t.ToString() + " ms");
	}

	private List<Person> AllAgent = new List<Person>();

	private void ClearMainList()
	{
		AllAgent = null;
	}

	private void SetProtocolLoad()
	{
		dConvert.Clear();
		dConvert.AddExternalAssembly("TacDConvertor");

		LogicBound(AllAgent);
		SceneBound(World.Society.AllAgentPoint);

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
		if (argIndex == 0)
		{
		}
	}
}

