// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using System;
using System.IO;
using System.Text;

using UnityEngine;

namespace Tac.DConvert
{
    public class Convertor
    {
        protected IWriter writer;
        protected IReader reader;
        protected bool IsRead = false;

        public int IndentLength = 0;
        public Convertor() { }

        public virtual void Open(string argFileName, bool argRead) { }
        public virtual void Open() { }
        public virtual void Open(string argBuffer) { }
        public virtual void Open(byte[] argBuffer) { }

        public virtual void WriteBreak() { }
        public virtual void ReadBreak() { }

        public virtual void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }
            if (reader != null)
            {
                reader.Close();
            }
        }

        public void Add<T>(T argValue, string argType, bool argIsEnum = false)
        {
            writer.DebugValue = "";

            if (argIsEnum)
            {
                writer.Write(Convert.ToInt32(argValue));
            }
            else
            {
                switch (argType)
                {
                    case "System.String":
                    case "System.Collections.Generic.List`1[System.String]":
                        writer.Write(argValue.ToString());
                        break;
                    case "System.Int32":
                    case "System.Collections.Generic.List`1[System.Int32]":
                        writer.Write(Convert.ToInt32(argValue));
                        break;
					case "System.Int64":
					case "System.Collections.Generic.List`1[System.Int64]":
						writer.Write(Convert.ToInt64(argValue));
						break;
					case "System.Single":
                    case "System.Collections.Generic.List`1[System.Single]":
                        writer.Write(Convert.ToSingle(argValue));
                        break;
                    case "System.Boolean":
                    case "System.Collections.Generic.List`1[System.Boolean]":
                        writer.Write(Convert.ToBoolean(argValue));
                        break;
					case "System.DateTime":
					case "System.Collections.Generic.List`1[System.DateTime]":
						writer.Write(Convert.ToDateTime(argValue));
						break;
					case "Tac.DConvert.Vector2_":
                        Vector2_ locVector2_ = argValue as Vector2_;
                        writer.Write(locVector2_.x);
                        writer.Write(locVector2_.y);
                        break;
                    case "Tac.DConvert.Vector3_":
                        Vector3_ locVector3_ = argValue as Vector3_;
                        writer.Write(locVector3_.x);
                        writer.Write(locVector3_.y);
                        writer.Write(locVector3_.z);
                        break;
					case "Tac.DConvert.Vector2__":
						Vector2__ locVector2__ = argValue as Vector2__;
						writer.Write(locVector2__.x);
						writer.Write(locVector2__.y);
						break;
					case "Tac.DConvert.Vector3__":
						Vector3__ locVector3__ = argValue as Vector3__;
						writer.Write(locVector3__.x);
						writer.Write(locVector3__.y);
						writer.Write(locVector3__.z);
						break;
					case "Tac.DConvert.Transform_":
                        Transform_ locTransform_ = argValue as Transform_;
                        writer.Write(locTransform_.position.x);
                        writer.Write(locTransform_.position.y);
                        writer.Write(locTransform_.position.z);
                        writer.Write(locTransform_.rotation.x);
                        writer.Write(locTransform_.rotation.y);
                        writer.Write(locTransform_.rotation.z);
                        writer.Write(locTransform_.scale.x);
                        writer.Write(locTransform_.scale.y);
                        writer.Write(locTransform_.scale.z);
                        break;
                    case "Tac.DConvert.Guid_":
                        Guid_ locGuid_ = argValue as Guid_;
                        writer.Write(new Guid(locGuid_.Value));
                        break;
					case "Tac.DConvert.NamedValue_":
						NamedValue_ locNamedValue_ = argValue as NamedValue_;
						writer.Write(locNamedValue_.Name);
						writer.Write(locNamedValue_.Value);
						break;
				}
			}
        }

        public int Load(int argValue, string argType)
        {
            object locValue = argValue as Int32?;
            return (int)Load<object, int>(locValue, argType, argValue);
        }

        public T Load<T, M>(T argValue, string argType, M argOriginalObject, bool argIsEnum = false)
            where T : class
        {
            T ret = default(T);

            if (argIsEnum)
            {
                ret = reader.ReadInt32() as T;
            }
            else
            {
                switch (argType)
                {
                    case "System.String":
                        ret = reader.ReadString() as T;
                        break;
                    case "System.Int32":
                        ret = reader.ReadInt32() as T;
                        break;
					case "System.Int64":
						ret = reader.ReadInt64() as T;
						break;
					case "System.Single":
                        ret = reader.ReadSingle() as T;
                        break;
                    case "System.Boolean":
                        ret = reader.ReadBoolean() as T;
                        break;
					case "System.DateTime":
						ret = reader.ReadDateTime() as T;
						break;
					case "Tac.DConvert.Vector2_":
                        ret = new Vector2_(reader.ReadSingle(), reader.ReadSingle()) as T;
                        break;
                    case "Tac.DConvert.Vector3_":
                        ret = new Vector3_(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()) as T;
                        break;
					case "Tac.DConvert.Vector2__":
						ret = new Vector2__(reader.ReadSingle(), reader.ReadSingle()) as T;
						break;
					case "Tac.DConvert.Vector3__":
						ret = new Vector3__(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()) as T;
						break;
					case "Tac.DConvert.Transform_":
                        Vector3_ locPosition = new Vector3_(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        Vector3_ locRotation = new Vector3_(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        Vector3_ locScale = new Vector3_(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        ret = new Transform_(argOriginalObject as Transform, locPosition, locRotation, locScale) as T;
                        break;
                    case "Tac.DConvert.Guid_":
                        ret = new Guid_(reader.ReadGuid()) as T;
                        break;
					case "Tac.DConvert.NamedValue_":
						ret = new NamedValue_(reader.ReadString(), reader.ReadSingle()) as T;
						break;
				}
			}

			//DebugLoad.DebugList.Add(ret.ToString());

			return ret;
        }

    }

    public interface IWriter
    {
        string DebugValue { get; set; }
        Stream BaseStream { get; }
        void Close();

        void Write(string v);
        void Write(int v);
		void Write(long v);
		void Write(float v);
        void Write(bool v);
        void Write(Guid v);
		void Write(DateTime v);
	}

	public interface IReader
    {
        Stream BaseStream { get; }
        void Close();

        string ReadString();
        int ReadInt32();
		long ReadInt64();
		float ReadSingle();
        bool ReadBoolean();
        Guid ReadGuid();
		DateTime ReadDateTime();
	}

	public class StringReaderExt : StringReader, IReader
    {
        public StringReaderExt(string s) : base (s) { }
        public Stream BaseStream
        {
            get { return null; }
        }

        public char delimiter = '\t';

        public string ReadElement()
        {
            StringBuilder sb = new StringBuilder();
            bool IsEnd = false;
            char[] buffer = new char[1];

            while (IsEnd == false)
            {
                base.Read(buffer, 0, 1);
                if (buffer[0] == delimiter)
                {
                    IsEnd = true;
                }
                else
                {
                    sb.Append(buffer[0]);
                }
            }

            return sb.ToString();
        }

        public void ReadBreak()
        {
            char[] buffer = new char[1];
            base.Read(buffer, 0, 1);
        }
        public void ReadDelimiter()
        {
            char[] buffer = new char[1];
            base.Read(buffer, 0, 1);
        }

        public string ReadString() 
        {
            return ReadElement(); 
        }
        public int ReadInt32() 
        { 
            return Convert.ToInt32(ReadElement()); 
        }
		public long ReadInt64()
		{
			return Convert.ToInt64(ReadElement());
		}
		public float ReadSingle() 
        { 
            return Convert.ToSingle(ReadElement()); 
        }
        public bool ReadBoolean() 
        { 
            return Convert.ToBoolean(ReadElement()); 
        }
        public Guid ReadGuid()
        {
            return new Guid(ReadElement());
        }
		public DateTime ReadDateTime()
		{
			return Convert.ToDateTime(ReadElement());
		}
	}

	public class StringWriterExt : StringWriter, IWriter
    {
        private string debugValue;
        public string DebugValue
        {
            get { return debugValue; }
            set { debugValue = value; }
        }

        public StringWriterExt(StringBuilder s) : base(s) { }
        public Stream BaseStream
        {
            get { return null; }
        }

        public char delimiter = '\t';

        public void WriteBreak()
        {
            base.Write("\n");
        }
        public void WriteDelimiter()
        {
            base.Write(delimiter);
        }

        public override void Write(string v)
        {
            base.Write(v);
            base.Write(delimiter);
            debugValue += v + delimiter;
        }
        public override void Write(int v)
        {
            base.Write(v);
            //base.Write(delimiter);
            debugValue += v.ToString();
        }
		public override void Write(long v)
		{
			base.Write(v);
			//base.Write(delimiter);
			debugValue += v.ToString();
		}
		public override void Write(float v)
        {
            base.Write(v);
            //base.Write(delimiter);
            debugValue += v.ToString();
        }
        public override void Write(bool v)
        {
            base.Write(v);
            //base.Write(delimiter);
            debugValue += v.ToString();
        }
        public void Write(Guid v)
        {
            string vText = v.ToString();
            base.Write(vText);
            base.Write(delimiter);
            debugValue += vText + delimiter;
        }
		public void Write(DateTime v)
		{
			string vText = v.ToString();
			base.Write(vText);
			base.Write(delimiter);
			debugValue += vText + delimiter;
		}
	}

	public class BinaryReaderExt : BinaryReader, IReader
    {
        public BinaryReaderExt(Stream s) : base(s) { }

        public Guid ReadGuid()
        {
            return new Guid(base.ReadBytes(16));
        }

		public DateTime ReadDateTime()
		{
			return DateTime.FromBinary(base.ReadInt64());
		}
	}

	public class BinaryWriterExt : BinaryWriter, IWriter
    {
        private string debugValue;
        public string DebugValue
        {
            get { return debugValue; }
            set { debugValue = value; }
        }

        public BinaryWriterExt(Stream s) : base(s) { }

        public void Write(Guid v)
        {
            base.Write(v.ToByteArray());
        }

		public void Write(DateTime v)
		{
			base.Write(v.ToBinary());
		}
	}


	public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

}
