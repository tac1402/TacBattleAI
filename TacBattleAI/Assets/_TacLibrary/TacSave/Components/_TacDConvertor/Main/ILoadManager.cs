using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Tac.DConvert
{

	public interface ILoadManager
	{
		IObject IObject { get;}

		/// <summary>
		/// Все объекты в игре с уникальными Id
		/// </summary>
		Dictionary<int, GameObject> AllObject { get; }

		/// <summary>
		/// Сбрасывает игру до начального состояния перед загрузкой сохранения
		/// </summary>
		public void ResetGame();

		/// <summary>
		/// Восстанавливает производные данные и все процессы после загрузки сохранения
		/// </summary>
		public void RecoverGame();

		public GameObject CreatePrefab(int argObjectId, string argModelName)
		{
			GameObject g = null;
			if (argModelName != "")
			{
				IObject.PredeffinedObjectId = argObjectId;
				g = IObject.CreateObject(argModelName);

				AddObject(argObjectId, g);
			}
			else
			{
				int a = 1;
			}
			return g;
		}

		/// <summary>
		/// Добавить объект с уникальным Id
		/// </summary>
		public void AddObject(int argObjectId, GameObject argObject)
		{
			if (AllObject.ContainsKey(argObjectId))
			{
				AllObject[argObjectId] = argObject;
			}
			else
			{
				AllObject.Add(argObjectId, argObject);
				if (IObject.ObjectIdCounter < argObjectId)
				{
					IObject.ObjectIdCounter = argObjectId;
				}
			}
		}

		/// <summary>
		/// Получить объект по уникальному Id
		/// </summary>
		public GameObject GetObject(int argId)
		{
			if (AllObject.ContainsKey(argId))
			{
				return AllObject[argId];
			}
			else
			{
				return null;
			}
		}


		public void ResetGameInner()
		{
			DestroyWorld();
			IObject.ObjectIdCounter = 0;
		}

		public void DestroyWorld()
		{
			for (int i = 0; i < IObject.WorldLevel.Count; i++)
			{
				Transform t = IObject.WorldLevel[i].Level.transform;
				for (int j = t.childCount - 1; j >= 0; j--)
				{
					IObject.DestroyObject(t.GetChild(j).gameObject);
				}
			}
		}

		public void ResetEvent(object argObject, string argEventName)
		{
			EventInfo eventInfo = argObject.GetType().GetEvent(argEventName);

			var fieldInfo = argObject.GetType().GetField(eventInfo.Name,
				BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

			if (fieldInfo.GetValue(argObject) is Delegate eventHandler)
			{
				foreach (var invocatedDelegate in eventHandler.GetInvocationList())
				{
					eventInfo.GetRemoveMethod(fieldInfo.IsPrivate).Invoke(
						argObject, new object[] { invocatedDelegate });
				}
			}
		}
	}

	public interface IPrefab
	{
		public GameObject GetObject(int argObjectId);

		public GameObject CreatePrefab(int argObjectId, string argModelName);
	}



}
