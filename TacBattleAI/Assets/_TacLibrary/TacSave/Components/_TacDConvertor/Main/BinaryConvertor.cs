// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using System;
using System.IO;

namespace Tac.DConvert
{
    public class BinaryConvertor : Convertor
    {
        public BinaryConvertor() { }

        public byte[] Buffer
        {
            get
            {
                if (IsRead)
                {
                    return reader.BaseStream.ReadAllBytes();
                }
                else
                {
                    return writer.BaseStream.ReadAllBytes();
                }
            }
        }

        public override void Open(string argFileName, bool argRead)
        {
            IsRead = argRead;
            if (argRead)
            {
                reader = new BinaryReaderExt(File.Open(argFileName, FileMode.Open));
            }
            else

                writer = new BinaryWriterExt(File.Open(argFileName, FileMode.Create));
        }

        public override void Open()
        {
            IsRead = false;
            writer = new BinaryWriterExt(new MemoryStream());
        }

        public override void Open(byte[] argBuffer)
        {
            IsRead = true;
            reader = new BinaryReaderExt(new MemoryStream(argBuffer));
        }
    }


}
