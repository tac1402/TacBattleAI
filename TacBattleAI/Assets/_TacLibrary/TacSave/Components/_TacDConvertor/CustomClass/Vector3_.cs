// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using UnityEngine;

namespace Tac.DConvert
{
    public class Vector3_ : ICustomConvert<Vector3_, Vector3>
    {
        public float x;
        public float y;
        public float z;

        public Vector3_() { }

        public Vector3_(float argX, float argY, float argZ)
        {
            x = argX; y = argY; z = argZ;
        }

        public Vector3_(Vector3 v)
        {
            ConvertFrom(v);
        }

        public void ConvertFrom(Vector3 v)
        {
            x = v.x; y = v.y; z = v.z;
        }

        public Vector3 ConvertTo()
        {
            return new Vector3(x, y, z);
        }
    }
}
