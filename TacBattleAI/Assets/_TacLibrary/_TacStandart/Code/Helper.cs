using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tac
{
	public static class Helper
	{
		public static bool In<T>(this T t, params T[] args)
		{
			return args.Contains(t);
		}

		public static bool Range(this int x, int min, int max)
		{
			 return ((x - max) * (x - min) <= 0);
		}
		public static bool Range(this float x, float min, float max)
		{
			return ((x - max) * (x - min) <= 0);
		}

		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			foreach (T item in enumeration)
			{
				action(item);
			}
		}

        public static string Repeat(this string text, int n)
        {
            return new StringBuilder(text.Length * n)
              .Insert(0, text, n)
              .ToString();
        }
    }

	public static class EnumExt
	{
        public static T GetValue<T>(string argValueName) where T : Enum
        {
			T ret = default(T);
            foreach (var item in Enum.GetValues(typeof(T)))
            {
				if (item.ToString() == argValueName)
				{ 
					ret = (T) item;
					break;
				}
            }
			return ret;
        }
    }

	public delegate void Change();
	public delegate void Send(params object[] argInfo);
	public delegate string ChangeAndReturn(params object[] argInfo);

}

