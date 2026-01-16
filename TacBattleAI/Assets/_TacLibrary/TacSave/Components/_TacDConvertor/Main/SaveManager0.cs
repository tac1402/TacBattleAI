using System;
using System.Collections;
using System.Collections.Generic;

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
		protected SaveMetaData Meta;


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

			Meta = dConvert.Connect();
		}

		/// <summary>
		/// Зарегистрировать объект с уникальным Id. В отличии от добавления объекта идентификатор выделяется на основании уникальной строки
		/// и сохраняется в мета-данных при сохранении.
		/// </summary>
		public void RegObject(string argObjectId, GameObject argObject)
		{
			int id = Meta.ReserveKey(argObjectId);

			Item locItem = argObject.GetComponent<Item>();
			if (locItem != null)
			{
				locItem.ObjectId = id;
				ILoadGet().AddObject(locItem.ObjectId, argObject);
			}
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
		public event Change LoadEnd;

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
			if (LoadEnd != null)
			{ 
				LoadEnd();
			}
			DayNight.PauseCompleteStop = false;
		}

		protected virtual bool LoadBin(string argDirName, string argFileName) { return false; }

		/// <summary>
		/// Привязанные к логике - перед загрузкой могут не находится на сцене, поэтому во время ResetGame будут удалены, и заново созданы из префабов
		/// </summary>
		protected void LogicBound<T>(List<T> allItem) where T : Item
		{
			if (allItem == null || allItem.Count > 0)
			{
				// Только List можно использовать для предварительной загрузки объектов в сцену, создавая из префабов (ListCreateMode.CreateFromPrefab)
				Debug.Assert(false, "LogicBound: the list must exist but be empty");
				throw new ArgumentException("LogicBound: the list must exist but be empty");
			}
			dConvert.Set(allItem, ListCreateMode.CreateFromPrefab);
		}

		/// <summary>
		/// Привязанный к сцене - перед загрузкой уже находятся на сцене, поэтому их не создаем из префаба, а изменяем только свойства
		/// </summary>
		protected void SceneBound<T>(List<T> allItem) where T : Item
		{
			if (allItem == null || allItem.Count == 0)
			{
				Debug.Assert(false, "SceneBound: the list must exist but not be empty");
				throw new ArgumentException("SceneBound: the list must exist but not be empty");
			}
			for (int i = 0; i < allItem.Count; i++)
			{
				ILoadGet().AddObject(allItem[i].ObjectId, allItem[i].gameObject);
			}
			dConvert.Set(allItem, ListCreateMode.UseCurrent);
		}

		#endregion

	}
}