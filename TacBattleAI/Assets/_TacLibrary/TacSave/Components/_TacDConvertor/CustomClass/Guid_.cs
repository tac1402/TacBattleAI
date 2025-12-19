// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using System;

namespace Tac.DConvert
{
    public class Guid_ : ICustomConvert<Guid_, Guid>
    {
        public byte[] Value;

        public Guid_() { }

        public Guid_(byte[] argValue)
        {
            Value = argValue;
        }

        public Guid_(Guid v)
        {
            ConvertFrom(v);
        }

        public void ConvertFrom(Guid v)
        {
            Value = v.ToByteArray();
        }

        public Guid ConvertTo()
        {
            return new Guid(Value);
        }
    }
}
