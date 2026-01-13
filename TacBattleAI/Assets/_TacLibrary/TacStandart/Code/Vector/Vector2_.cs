using System.Numerics;

namespace Tac
{
	[System.Serializable]
	public class Vector2_
	{
		public float x;
		public float y;

		public static Vector2_ zero = new Vector2_(0, 0);

		public Vector2_() { }

		public Vector2_(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static float Distance(Vector2_ argV1, Vector2_ argV2)
		{
			Vector2 v1 = new Vector2(argV1.x, argV1.y);
			Vector2 v2 = new Vector2(argV2.x, argV2.y);

			return Vector2.Distance(v1, v2);
		}

		public static Vector2_ operator +(Vector2_ argV1, Vector2_ argV2)
		{
			Vector2 v1 = new Vector2(argV1.x, argV1.y);
			Vector2 v2 = new Vector2(argV2.x, argV2.y);
			Vector2 r = v1 + v2;

			return new Vector2_(r.X, r.Y);
		}

	}

	[System.Serializable]
	public class Vector2Int_
	{
		public int x;
		public int y;

		public Vector2Int_(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	[System.Serializable]
	public class Rect_
	{
		public float x;
		public float y;
		public float width;
		public float height;

		public Rect_(float argX, float argY, float argWidth, float argHeight)
		{
			x = argX;
			y = argY;
			width = argWidth;
			height = argHeight;
		}
	}
}
