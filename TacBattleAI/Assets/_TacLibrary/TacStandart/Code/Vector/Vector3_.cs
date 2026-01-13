using System.Numerics;


namespace Tac
{
	[System.Serializable]
	public class Vector3_
	{
		public float x;
		public float y;
		public float z;

		public static Vector3_ zero = new Vector3_(0, 0, 0);

		public Vector3_() { }

		public Vector3_(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static float Distance(Vector3_ argV1, Vector3_ argV2)
		{
			Vector3 v1 = new Vector3(argV1.x, argV1.y, argV1.z);
			Vector3 v2 = new Vector3(argV2.x, argV2.y, argV2.z);

			return Vector3.Distance(v1, v2);
		}

		public static Vector3_ operator +(Vector3_ argV1, Vector3_ argV2)
		{
			Vector3 v1 = new Vector3(argV1.x, argV1.y, argV1.z);
			Vector3 v2 = new Vector3(argV2.x, argV2.y, argV2.z);
			Vector3 r = v1 + v2;

			return new Vector3_(r.X, r.Y, r.Z);
		}
	}
}