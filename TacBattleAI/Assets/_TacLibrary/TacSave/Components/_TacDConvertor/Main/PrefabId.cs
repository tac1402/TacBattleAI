using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Tac.DConvert
{
    public class PrefabId : Item, IDataSave, IPrefabId
	{
		#region IDataSave
		private Queue<ObjectInfo> data = new Queue<ObjectInfo>();
		public Queue<ObjectInfo> Data
		{
			get { return data; }
		}
		protected string dataTag;
		public string DataTag
		{
			get { return dataTag; }
			set { dataTag = value; }
		}

		private string className = "";
		private List<string> propertyName;
		public string ClassName { get { return className; } }
		public List<string> PropertyName { get { return propertyName; } }

		public void ClearQ()
		{
			Data.Clear();
		}

		private bool isLoad = false;
		public bool IsLoad
		{
			get { return isLoad; }
			set { isLoad = value; }
		}

		public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
		{
			return SaveQ(propertyValue, propertyLambda, argTag.ToString());
		}

		public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, string argTag = null)
		{
			MemberExpression me = propertyLambda.Body as MemberExpression;

			MemberExpression m1 = me;
			ConstantExpression c1 = null;
			string tmpClassName = "";
			string tmpPropertyName = me.Member.Name;
			if (className == "") { propertyName = new List<string>(); }

			while (tmpClassName == "")
			{
				if (m1 != null && m1.Expression != null)
				{
					c1 = m1.Expression as ConstantExpression;
					m1 = m1.Expression as MemberExpression;

					if (m1 != null)
					{
						tmpPropertyName = m1.Member.Name + "." + tmpPropertyName;
					}
					if (c1 != null)
					{
						tmpClassName = c1.Type.ToString();
					}
				}
				else
				{
					break;
				}
			}

			className = tmpClassName;
			if (propertyName.Contains(tmpPropertyName) == false)
			{
				propertyName.Add(tmpPropertyName);
			}

			/*
			DateTime saveQ = DateTime.Now;

			Func<T> f = propertyLambda.Compile();

			double t = (DateTime.Now - saveQ).TotalMilliseconds;
			all += t;
			System.IO.File.AppendAllText("LoadLog2.txt", "saveQ: " + t.ToString() + " / " + all.ToString() + "\n");

			T a = f();
            */

			T a = propertyValue;

			if (IsLoad == false)
			{
				Data.Enqueue(new ObjectInfo().Add(a, argTag));
			}
			else
			{
				a = (T)Data.Dequeue().Object;
			}

			return a;
		}


		#endregion

		#region Save

		public int Id
		{
			get { return ObjectId; }
			set { ObjectId = value; }
		}

		public string PrefabName
		{
			get { return ModelName; }
			set { ModelName = value; }
		}


		public Transform Transform
		{
			get { return transform; }
			set
			{
				transform.position = value.position;
				transform.localEulerAngles = value.localEulerAngles;
				transform.localScale = value.localScale;
			}
		}

		public bool IsActive
		{
			get { return gameObject.activeSelf; }
			set { gameObject.SetActive(value); }
		}

		public virtual void SaveData(bool argLoadMode)
		{
			IsLoad = argLoadMode;
			if (argLoadMode == false)
			{
				ClearQ();
			}
			Id = SaveQ(Id, () => Id);
			PrefabName = SaveQ(PrefabName, () => PrefabName);
		}
		#endregion

	}




	public class DataSave : IDataSave
	{
		#region IDirectConvertDataSave

		private ConvertData convertData = new ConvertData();

		public Queue<ObjectInfo> Data
		{
			get { return convertData.Data; }
		}
		public string DataTag
		{
			get { return convertData.DataTag; }
			set { convertData.DataTag = value; }
		}


		public virtual void SaveData(bool argIsLoad)
		{
			convertData.IsLoad = argIsLoad;
			if (argIsLoad == false)
			{
				convertData.ClearQ();
			}
		}

		public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
		{
			return SaveQ(propertyValue, propertyLambda, argTag.ToString());
		}


		public string ClassName { get { return convertData.ClassName; } }
		public List<string> PropertyName { get { return convertData.PropertyName; } }


		public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, string argTag = null)
		{
			return convertData.SaveQ(propertyValue, propertyLambda, argTag);
		}

		#endregion
	}


}
