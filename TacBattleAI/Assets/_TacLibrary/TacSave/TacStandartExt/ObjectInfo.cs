// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021-26 Sergej Jakovlev

using System;

namespace Tac.DConvert
{
    public class ObjectInfo
    {
        public object Object;
        public Type Type;
        public string Tag;

        public ObjectInfo() { }

        public ObjectInfo Add<T>(T argObject, string argTag = null)
        {
            Object = argObject;
            Type = typeof(T);
            Tag = argTag;
            return this;
        }

        public ObjectInfo Clone()
        { 
            return (ObjectInfo)this.MemberwiseClone();
        }

    }

}
