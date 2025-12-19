// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2022 Sergej Jakovlev

using UnityEngine;

namespace Tac.DConvert
{
    public class Vector2_ : ICustomConvert<Vector2_, Vector2>
    {
        public float x;
        public float y;

        public Vector2_() { }

        public Vector2_(float argX, float argY)
        {
            x = argX; y = argY;
        }

        public Vector2_(Vector2 v)
        {
            ConvertFrom(v);
        }

        public void ConvertFrom(Vector2 v)
        {
            x = v.x; y = v.y;
        }

        public Vector2 ConvertTo()
        {
            return new Vector2(x, y);
        }
    }
}

