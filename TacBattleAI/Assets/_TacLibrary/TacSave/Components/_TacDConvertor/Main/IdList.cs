// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2023 Sergej Jakovlev

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tac.DConvert
{
    public class IdList<T> : ConvertData, IEnumerable<KeyValuePair<int, T>>
		where T : IId
    {
		public Dictionary<int, T> IdArray = new Dictionary<int, T>();

		public List<T> List
        {
            get { return IdArray.Values.ToList(); }
        }

        public int Count { get { return List.Count; } }

        public T this[int index, bool isList]
        {
            get { return List[index]; }
        }

		public T this[int index]
		{
			get { return IdArray[index]; }
		}

		public void Add(T item) 
        {
			MaxId.Value++;
			item.Id = MaxId.Value;
            IdArray.Add(item.Id, item);
        }

        public void Clear()
        { 
            IdArray.Clear();
        }

        public void Init(List<T> argList)
        { 
            for (int i = 0; i < argList.Count; i++) 
            { 
                Add(argList[i]);
            }
        }

		#region IEnumerable

		public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
		{
			return IdArray.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion

		#region Save

		public int MaxIdValue
		{
			get { return MaxId.Value; }
			set { MaxId.Value = value; }
		}

		public override void SaveData(bool argLoadMode)
		{
			base.SaveData(argLoadMode);

			MaxIdValue = SaveQ(MaxIdValue, () => MaxIdValue);
			IdArray = SaveQ(IdArray, () => IdArray);
		}

		#endregion
	}

    public static class MaxId
    { 
        private static int maxId;
		public static int Value
		{
			get { return maxId; }
			set
			{
				if (value > maxId)
				{
					maxId = value;
				}
			}
		}
		public static void Reset()
		{ 
			maxId = 0;
		}

	}


	public interface IPrefabId : IId
	{
		public string PrefabName { get; set; }
	}


	public interface IId
    { 
        public int Id { get; set; }
	}

}
