// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2022 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tac.DConvert
{
	public class ConvertData : IDataSave
	{
		public int Id { get { return 0; } set { } }
		public string PrefabName { get { return ""; } set { } }

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
			// Для объектов, на которые не ссылаются, идентификацию писать не нужно
			(this as IDataSave).SaveDataInner(argLoadMode, false);
		}

		protected T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
		{
			return (this as IDataSave).SaveQ(propertyValue, propertyLambda, argTag);
		}
		protected T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, string argTag = null)
		{
			return (this as IDataSave).SaveQ(propertyValue, propertyLambda, argTag);
		}

		protected Queue<K> SaveQQ<K>(Queue<K> propertyValue, Expression<Func<Queue<K>>> propertyLambda, PredefinedTag argTag)
		{
			return SaveQQ(propertyValue, propertyLambda, argTag.ToString());
		}
		protected Queue<K> SaveQQ<K>(Queue<K> propertyValue, Expression<Func<Queue<K>>> propertyLambda, string argTag = null)
		{
			(this as IDataSave).SetMeta(propertyLambda);
			return new Queue<K>((this as IDataSave).SaveD(propertyValue.ToList(), argTag));
		}
	}
}
