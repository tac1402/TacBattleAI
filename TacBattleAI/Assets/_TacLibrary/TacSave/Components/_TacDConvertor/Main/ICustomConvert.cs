// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

namespace Tac.DConvert
{
    public interface ICustomConvert<T, K>
    {
        void ConvertFrom(K argValue);
        K ConvertTo();
    }
}