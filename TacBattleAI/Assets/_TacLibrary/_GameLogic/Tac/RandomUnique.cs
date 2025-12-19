using System.Collections;
using System.Collections.Generic;


namespace Tac
{
	public delegate int GetInt();


	public class RandomWorld
	{
		private System.Random rnd = new System.Random();

		public Vector2_ GetRandomPosition(Vector2_ argCenter, float argRadius)
		{
			Vector2_ ret = Vector2_.zero;
			LimitCircle c = new LimitCircle(argCenter, argRadius);

			for (int index2 = 0; index2 < 10; index2++)
			{
				ret.x = (float)(c.Center.x - c.Radius + rnd.Next((int)c.Radius * 2) + rnd.NextDouble());
				ret.y = (float)(c.Center.y - c.Radius + rnd.Next((int)c.Radius * 2) + rnd.NextDouble());
				if (Vector2_.Distance(ret, c.Center) < c.Radius)
				{
					break;
				}
			}
			return ret;
		}

		public Vector2_ GetRandomPosition(Rect_ argRect)
		{
			Vector2_ ret = Vector2_.zero;
			ret.x = (float)(argRect.x + rnd.Next((int)(argRect.width - argRect.x)) + rnd.NextDouble());
			ret.y = (float)(argRect.y + rnd.Next((int)(argRect.height - argRect.y)) + rnd.NextDouble());
			return ret;
		}

	}

	public class RandomUnique
	{
		public int UsedCount
		{
			get { return Used.Count; }
		}

		private List<int> Used = new List<int>();

		private System.Random rnd;

		public RandomUnique(System.Random argRnd)
		{
			rnd = argRnd;
		}

		public void MarkUsed(int argIndex)
		{
			Used.Add(argIndex);
		}

		public int Get(int argMax, GetInt getInt = null)
		{
			// Почти все израсходованы
			if (UsedCount > argMax - 10) { return -1; }

			int ret = 0;
			bool IsEnd = false;
			while (IsEnd == false)
			{
				if (getInt == null)
				{
					ret = rnd.Next(argMax);
				}
				else
				{
					ret = getInt();
				}

				bool IsFound = false;
				for (int k = 0; k < Used.Count; k++)
				{
					if (ret == Used[k])
					{
						IsFound = true;
						break;
					}
				}
				if (IsFound == false)
				{
					IsEnd = true;
				}
			}
			Used.Add(ret);
			return ret;
		}
	}

}
