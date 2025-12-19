// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2023 Sergej Jakovlev

using System.Collections.Generic;

namespace Tac.DConvert
{
    public interface IDataSave
    {
        Queue<ObjectInfo> Data { get; }
        string DataTag { get; set; }

        string ClassName { get; }
        List<string> PropertyName { get; }


        void SaveData(bool LoadMode);
    }
}
