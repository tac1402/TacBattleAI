// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2023 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tac.DConvert
{
    public interface IDataSave : IPrefabId
    {
        Queue<ObjectInfo> Data { get; }
        string DataTag { get; set; }

        string ClassName { get; set; }
        List<string> PropertyName { get; set; }

		bool IsLoad { get; set; }

		#region default

		private void ClearQ()
		{
			Data.Clear();
		}

		public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
		{
			return SaveQ(propertyValue, propertyLambda, argTag.ToString());
		}

		public void SetMeta<T>(Expression<Func<T>> propertyLambda)
		{
			MemberExpression me = propertyLambda.Body as MemberExpression;

			MemberExpression m1 = me;
			ConstantExpression c1 = null;
			string tmpClassName = "";
			string tmpPropertyName = me.Member.Name;
			if (ClassName == "") { PropertyName = new List<string>(); }

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

			ClassName = tmpClassName;
			if (PropertyName.Contains(tmpPropertyName) == false)
			{
				PropertyName.Add(tmpPropertyName);
			}
		}

		public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, string argTag = null)
		{
			SetMeta(propertyLambda);
			return SaveD(propertyValue, argTag);
		}

		public T SaveD<T>(T propertyValue, string argTag = null)
		{
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



		public void SaveData(bool argLoadMode)
		{
			SaveDataInner(argLoadMode);
		}

		public void SaveDataInner(bool argLoadMode)
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
}
