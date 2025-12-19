// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2022 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Tac.DConvert
{
	public class ConvertData : IDataSave, IId
	{
        #region IDirectConvertDataSave

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

        public virtual void SaveData(bool argIsLoad)
        {
            IsLoad = argIsLoad;
            if (argIsLoad == false)
            {
                ClearQ();
            }
        }

        public T SaveQ<T>(T propertyValue, Expression<Func<T>> propertyLambda, PredefinedTag argTag)
        {
            return SaveQ(propertyValue, propertyLambda, argTag.ToString());
        }

        private string className = "";
        private List<string> propertyName;
        public string ClassName { get { return className; } }
        public List<string> PropertyName { get { return propertyName; } }

        static double all = 0;

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

        #region Id

        private int id;
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		#endregion

	}

	public enum PredefinedTag
    {
        OnlyPrefabId = 1,
		UseCurrent = 2,
		//OnlyCacheId = 3,
		//SaveInCañhe = 4,
		UseCurrentPrefabId = 5
	}
}
