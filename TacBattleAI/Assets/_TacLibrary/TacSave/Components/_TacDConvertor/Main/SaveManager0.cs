using System;
using System.Collections;

using System.Reflection;

using UnityEngine;
using UnityEngine.LightTransport;

namespace Tac.DConvert
{
	public abstract class SaveManager0 : MonoBehaviour, ISaveManager
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


		protected DirectConvert dConvert;

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

		protected virtual void SaveBin(string argDirName, string argFileName) { }

		#endregion


		#region Load

		public event Change LoadError;

		public void Load(string argDirName, string argFileName)
		{
			dConvert.ILoad = ILoadGet();
			StartCoroutine(LoadRun(argDirName, argFileName));
		}

		/// <summary>
		/// Получение интерфейса для загрузки. Должен быть обязательно переопределен в наследнике
		/// </summary>
		protected virtual ILoadManager ILoadGet()
		{
			throw new System.NotImplementedException("Must be implemented in a derived class.");
		}


		private IEnumerator LoadRun(string argDirName, string argFileName)
		{
			ILoadGet().ResetGame();
			yield return new WaitForSeconds(1.0f);
			bool isLoadError = LoadBin(argDirName, argFileName);
			if (isLoadError == false)
			{
				ILoadGet().RecoverGame();
			}
			else
			{
				if (LoadError != null)
				{
					LoadError();
				}
			}
			DayNight.PauseCompleteStop = false;
		}

		protected virtual bool LoadBin(string argDirName, string argFileName) { return false; }



		#endregion

	}
}