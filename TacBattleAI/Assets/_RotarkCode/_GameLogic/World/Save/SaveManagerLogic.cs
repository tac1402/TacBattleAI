
using System;
using System.Collections.Generic;
using System.Linq;
using Tac.DConvert;
using Tac.Person;

public partial class SaveManager 
{
	private DirectConvert dConvert;

	public World World;


	private void SetProtocolSave()
	{
		dConvert.Clear();

		dConvert.Set(World, ListCreateMode.UseCurrent);

		//dConvert.Set(World.Society.People, ListCreateMode.CreateFromPrefab);

		/*
		dConvert.Set(PersonList, ListCreateMode.CreateFromPrefab);
		dConvert.Set(AgentPointList, ListCreateMode.CreateFromPrefab);
		dConvert.Set(World, ListCreateMode.UseCurrent);
		*/
	}


	private void SaveBin(string argDirName, string argFileName)
	{
		DateTime begin = DateTime.Now;
		SetProtocolSave();
		dConvert.Save(argDirName + "\\" + argFileName + ".bin", ConvertorType.Text);
		double t = (DateTime.Now - begin).TotalMilliseconds;
		//Debug.Log("Save: " + t.ToString() + " ms");
	}


	private bool LoadBin(string argDirName, string argFileName)
	{
		bool IsLoadError = false;


		return IsLoadError;
	}


}
