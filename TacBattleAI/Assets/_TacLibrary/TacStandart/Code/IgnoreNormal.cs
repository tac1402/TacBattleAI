// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;

namespace Tac
{

    [Serializable]
    public class IgnoreNormal
    {
        public bool X = true;
        public bool Y = true;
        public bool Z = true;

        public IgnoreNormal() { }
        public IgnoreNormal(bool argX, bool argY, bool argZ)
        {
            X = argX;
            Y = argY;
            Z = argZ;
        }

        public IgnoreNormal GetReverse()
        {
            bool x = (X == true ? false : true);
            bool z = (Z == true ? false : true);
            return new IgnoreNormal(x, Y, z);
        }
    }
}