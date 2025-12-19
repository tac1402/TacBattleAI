// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using System;

namespace Tac.DConvert
{
    public class ConvertInfo
    {
        public object Object;
        public Type Type;
        public object MainObject;
        
        public bool IsList;
        public Type ListType;
        public ListCreateMode ListCreateMode;

        public bool IsDictionary;
        public Type KeyType;
        public Type ValueType;

        public string Tag;

        public ConvertInfo() { }

        public ConvertInfo(object argObject, Type argType)
        : this(argObject, argType, null, false, null, ListCreateMode.Recreate, false, null, null) { }

        public ConvertInfo(object argObject, Type argType, object argMainObject)
        : this(argObject, argType, argMainObject, false, null, ListCreateMode.Recreate , false, null, null) { }

        public ConvertInfo(object argObject, Type argType, object argMainObject, bool argIsList, Type argListType, ListCreateMode argListCreateMode)
        : this(argObject, argType, argMainObject, argIsList, argListType, argListCreateMode, false, null, null) { }

        public ConvertInfo(object argObject, Type argType, object argMainObject, bool argIsList, Type argListType, ListCreateMode argListCreateMode,
            bool argIsDictionary, Type argKeyType, Type argValueType)
        {
            Object = argObject;
            Type = argType;
            MainObject = argMainObject;
            IsList = argIsList;
            ListType = argListType;
            ListCreateMode = argListCreateMode;
            IsDictionary = argIsDictionary;
            KeyType = argKeyType;
            ValueType = argValueType;
        }

    }
}
