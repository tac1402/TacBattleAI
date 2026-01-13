// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac.DConvert
{
	public class Vector2__ : ICustomConvert<Vector2__, Tac.Vector2_>
	{
		public float x;
		public float y;

		public Vector2__() { }

		public Vector2__(float argX, float argY)
		{
			x = argX; y = argY;
		}

		public Vector2__(Tac.Vector2_ v)
		{
			ConvertFrom(v);
		}

		public void ConvertFrom(Tac.Vector2_ v)
		{
			x = v.x; y = v.y;
		}

		public Tac.Vector2_ ConvertTo()
		{
			return new Tac.Vector2_(x, y);
		}
	}
}

