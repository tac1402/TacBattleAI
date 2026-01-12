// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using UnityEngine.UI;

using Tac.DConvert;

namespace Tac.DConvert
{
	public interface IPrefabId : IId
	{
		public string PrefabName { get; set; }
	}

	public interface IId
	{
		public int Id { get; set; }
	}
}

namespace Tac
{
	/// <summary>
	/// ”ниверсальна€ сущность от Tac (дополн€етс€ реализацией IPrefabId и IDataSave дл€ сохранени€)
	/// </summary>
	public abstract partial class Item: IDataSave, IPrefabId
	{
		public int Id { get { return ObjectId; } set { ObjectId = value; } }
		public string PrefabName { get { return ModelName; } set { ModelName = value; } }

		#region IDataSave
		private Queue<ObjectInfo> data = new Queue<ObjectInfo>();
		public Queue<ObjectInfo> Data { get { return data; } }
		protected string dataTag;
		public string DataTag { get { return dataTag; } set { dataTag = value; } }

		private string className = "";
		public string ClassName { get { return className; } set { className = value; } }

		private List<string> propertyName;
		public List<string> PropertyName { get { return propertyName; } set { propertyName = value; } }

		private bool isLoad = false;
		public bool IsLoad { get { return isLoad; } set { isLoad = value; } }

		#endregion

		public virtual void SaveData(bool argLoadMode) 
		{
			(this as IDataSave).SaveDataInner(argLoadMode);
		}

		protected T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
		{ 
			return (this as IDataSave).SaveQ(propertyValue, propertyLambda, argTag);
		}

		protected T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, string argTag = null)
		{ 
			return (this as IDataSave).SaveQ(propertyValue, propertyLambda, argTag);
		}
	}


	public abstract partial class DayNight0 : IDayNight
	{
		/// <summary>
		/// “екстовое поле в UI в котором будет отображатьс€ текущие врем€
		/// </summary>
		public Text gameTime;
		public Text GameTime => gameTime;

		/// <summary>
		/// “екстовое поле в UI в котором будет отображатьс€ текущий номер суток
		/// </summary>
		public Text gameDays;
		public Text GameDays => gameDays;
	}
}