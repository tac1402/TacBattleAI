// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;


namespace Tac.DConvert
{
    public class TextConvertor : Convertor
    {
        public TextConvertor() 
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
        }

        public string FileName = "";

        public override void Open(string argFileName, bool argRead)
        {
            FileName = argFileName;
            IsRead = argRead;
            if (argRead)
            {
                reader = new StringReaderExt(File.ReadAllText(argFileName));
            }
            else
            {
                writer = new StringWriterExt(new StringBuilder());
            }
        }

        public override void Close()
        {
            if (FileName != "" && writer != null)
            {
                File.WriteAllText(FileName, writer.ToString());
                writer.Close();
            }
            base.Close();
        }

        public override void Open()
        {
            IsRead = false;
            writer = new StringWriterExt(new StringBuilder());
        }

        public override void Open(string argBuffer)
        {
            IsRead = true;
            reader = new StringReaderExt(argBuffer);
        }

        public override void WriteBreak() 
        {
            StringWriterExt writerString = writer as StringWriterExt;
            if (writerString != null)
            {
                writerString.WriteBreak();
                for (int i = 0; i < IndentLength; i++)
                {
                    writerString.WriteDelimiter();
                }
            }
        }


        public override void ReadBreak() 
        {
            //DebugLoad.DebugList.Add("break");

			StringReaderExt readerString = reader as StringReaderExt;
            if (readerString != null)
            {
                readerString.ReadBreak();
                for (int i = 0; i < IndentLength; i++)
                {
                    readerString.ReadDelimiter();
                }
            }
        }

    }
}