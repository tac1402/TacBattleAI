// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tac.ItemCreate
{
	public class ItemCreate : MonoBehaviour
	{
		public List<EntityType> WorldLevel;

		/// <summary>
		/// Все модели 
		/// </summary>
		public List<GameObject> Models = new List<GameObject>();
		/// <summary>
		/// Индексация моделей по имени и типу
		/// </summary>
		private Dictionary<string, int> IndexList = new Dictionary<string, int>();

		//public event ItemInfo OnItemCreate;

		public void Init()
		{
			List<GameObject> tmpList = Models.ToList();
			Models.Clear();
			IndexList.Clear();
			AddPack(tmpList);
			ResetObject();
		}

		#region Create

		public int ObjectIdBegin = 100; // 1-100 для объектов уже размещенных на сцене

		/// <summary>
		/// Счетчик максимального идентификатора объектов
		/// </summary>
		public int ObjectIdCounter = 0;
		public void ResetObject()
		{
			ObjectIdCounter = ObjectIdBegin;
		}

		public int GetNewId()
		{
			ObjectIdCounter++;
			return ObjectIdCounter;
		}

		public int PredeffinedObjectId;


		/// <summary>
		/// Создать объект на карте (разрешаются объекты только с компонентом Item2)
		/// </summary>
		/// <param name="argModelName">Имя модели в ресурсах, на основании которой будет создаваться объект</param>
		/// <param name="argX">Позиция по X</param>
		/// <param name="argY">Позиция по Y</param>
		/// <returns>Созданый объект или null если отказано</returns>
		public GameObject CreateObject(string argModelName, float argX, float argY, float? Height, ModelTypes argModelType = ModelTypes.Model, GameObject argParent = null)
		{
			GameObject locObject = null;

			// Загрузим модель и проверим, что у неё есть компонент Item2
			GameObject locModel = GetModel(argModelName, argModelType);

			Item locModelItem = null;
			if (locModel != null) 
			{
				locModelItem = locModel.GetComponent<Item>();
			}
			if (locModelItem != null)
			{
				locObject = Instantiate(locModel);
				Item locItem = locObject.GetComponent<Item>();

				if (PredeffinedObjectId == 0)
				{
					ObjectIdCounter++;
					locItem.ObjectId = ObjectIdCounter;
				}
				else
				{
					locItem.ObjectId = PredeffinedObjectId;
					PredeffinedObjectId = 0;
				}

				// Выставим ему требуемую позицию
				if (Height == null)
				{
					locObject.transform.position = new Vector3(argX, locModel.transform.position.y, argY);
				}
				else
				{
					locObject.transform.position = new Vector3(argX, Height ?? 0, argY);
				}

				if (argParent != null)
				{
					locObject.gameObject.transform.SetParent(argParent.transform);
				}
				else if (locItem.GroupId != -1)
				{
					EntityType locLevel = EntityType.Get(WorldLevel, locItem.GroupId);
					if (locLevel != null)
					{
						locObject.gameObject.transform.SetParent(locLevel.Level.transform);
					}
				}

				/*if (OnItemCreate != null)
				{
					OnItemCreate(locItem);
				}*/
			}
			return locObject;
		}

		public GameObject CreateObject(string argModelName, float argX, float argY, float? Height, GameObject argParent)
		{
			return CreateObject(argModelName, argX, argY, Height, ModelTypes.Model, argParent);
		}

		public GameObject CreateObject(string argModelName)
		{
			return CreateObject(argModelName, 0, 0, null, 0);
		}

		public event Change DelayDelete;

		private IEnumerator DelayDeleteTask()
		{
			yield return new WaitForSeconds(0.3f);
			if (DelayDelete != null)
			{
				DelayDelete();
				DelayDelete = null;
			}
		}

		/// <summary>
		/// Удалить объект из мира
		/// </summary>
		/// <param name="argObject">Удаляемый объект</param>
		public void DestroyObject(GameObject argObject)
		{
			if (argObject != null)
			{
				Destroy(argObject);
				StartDelayDelete();
			}
		}

		public void StartDelayDelete()
		{
			if (DelayDelete != null)
			{
				StartCoroutine(DelayDeleteTask());
			}
		}

		#endregion

		#region Model

		/// <summary>
		/// Получить модель по идентификации
		/// </summary>
		public GameObject GetModel(string argModelName, ModelTypes argModelType = ModelTypes.Model)
		{
			GameObject retModel = null;
			try
			{
				string locNameKey = argModelName + "-" + argModelType.ToString();
				retModel = Models[IndexList[locNameKey]];
			}
			catch (Exception) { }
			return retModel;
		}


		public void AddModel(GameObject argModel)
		{
			if (argModel != null)
			{
				Item locModelItem = argModel.GetComponent<Item>();
				if (locModelItem != null)
				{
					string locNameKey = locModelItem.ModelName + "-" + locModelItem.ModelType.ToString();

					if (IndexList.ContainsKey(locNameKey))
					{
						int index = IndexList[locNameKey];
						Models[index] = argModel;
					}
					else
					{
						Models.Add(argModel);
						IndexList.Add(locNameKey, IndexList.Count);
					}
				}
			}
		}


		public void AddPack(List<GameObject> argModelList)
		{
			foreach (GameObject item in argModelList)
			{ 
				AddModel(item);
			}
		}
		#endregion

	}
}


