using System.Collections;

using Tac;
using Tac.DConvert;
using Tac.Person;
using UnityEngine;

public partial class SaveManager : MonoBehaviour, ISaveManager
{
	/// <summary>
	/// true - если запись происходит в отладочным режиме в редакторе Юнити
	/// </summary>
	public bool InEditor = false;
	/// <summary>
	/// Включен ли отладочный режим сохранения
	/// </summary>
	public bool IsDebugMode = true;

	public string version = "v0.01";
	public string Version { get { return version; } set { version = value; } }

	private string saveRootDir = "";
	public string SaveRootDir { get { return saveRootDir; } }

	private void Start()
	{
		if (InEditor)
		{
			saveRootDir = "Assets\\SaveTmp\\";
		}
		else
		{
			saveRootDir = Application.persistentDataPath + "\\";
		}

		dConvert = new DirectConvert();
		dConvert.IsDebugMode = IsDebugMode;
	}

	#region Save
	/// <summary>
	/// Сохранение (точка входа)
	/// </summary>
	public void Save(string argDirName, string argFileName)
	{
		SaveBin(argDirName, argFileName + "_" + Version);
	}

	#endregion


	#region Load

	public event Change LoadError;

	public void Load(string argDirName, string argFileName)
	{
		StartCoroutine(LoadRun(argDirName, argFileName));
	}

	private IEnumerator LoadRun(string argDirName, string argFileName)
	{
		//IEntity.ResetGame();
		ResetGame();
		yield return new WaitForSeconds(1.0f);
		bool isError = LoadBin(argDirName, argFileName);
		if (isError == false)
		{
			//IEntity.RecoverGame();
		}
		else
		{
			if (LoadError != null)
			{
				LoadError();
			}
		}
	}

	private void ResetGame()
	{ 
	
	}


	#endregion

}
