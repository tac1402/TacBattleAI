// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
	/// Универсальная сущность от Tac (дополняется реализацией IPrefabId и IDataSave для сохранения)
	/// </summary>
	public abstract partial class Item: IPrefabId, IDataSave
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
			(this as IDataSave).SaveData(argLoadMode);
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
}