// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

namespace Tac.DConvert
{
	public class Vector3__ : ICustomConvert<Vector3__, Tac.Vector3_>
	{
		public float x;
		public float y;
		public float z;

		public Vector3__() { }

		public Vector3__(float argX, float argY, float argZ)
		{
			x = argX; y = argY; z = argZ;
		}

		public Vector3__(Tac.Vector3_ v)
		{
			ConvertFrom(v);
		}

		public void ConvertFrom(Tac.Vector3_ v)
		{
			x = v.x; y = v.y; z = v.z;
		}

		public Tac.Vector3_ ConvertTo()
		{
			return new Tac.Vector3_(x, y, z);
		}
	}
}
