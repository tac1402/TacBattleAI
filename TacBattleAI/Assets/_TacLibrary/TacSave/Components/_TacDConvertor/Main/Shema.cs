// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2023 Sergej Jakovlev

using System;
using System.IO;
using System.Collections.Generic;

namespace Tac.DConvert
{
    public class Shema
    {

        public Dictionary<string, List<string>> ShemaSaved = new Dictionary<string, List<string>>();
        public List<string> ShemaInfo = new List<string>();
        public Dictionary<string, List<string>> ShemaLoaded;


        /// <summary>
        /// Список всех типов встречающихся в сохранении (создается при сохранении)
        /// </summary>
        private Dictionary<string, int> TypeList = new Dictionary<string, int>();
        
        private Dictionary<string, int> TagList = new Dictionary<string, int>();

		/// <summary>
		/// Список всех типов встречающихся в сохранении (создается при загрузке)
		/// </summary>
		private Dictionary<int, Type> TypeListR = new Dictionary<int, Type>();

        private Dictionary<int, string> TagListR = new Dictionary<int, string>();

        public List<ConvertInfo> ConvertInfo = new List<ConvertInfo>();

        public int LoadNumber = 0;

        public List<string> ExternalAssemblys = new List<string>();

        public Shema()
        {
            TagList.Add("null", 0);
			AddType("null");
            TagListR.Add(0, null);
            TypeListR.Add(0, null);
        }

        /*public int TabCount = 0;

        public void SaveClassBegin()
        {
            TabCount++;
            ShemaInfo[ShemaInfo.Count - 1] += "{";
        }
        public void SaveClassEnd()
        {
            TabCount--;
            ShemaInfo[ShemaInfo.Count - 1] += "}";
        }


        public void SaveListInfo(int argCount)
        {
            TabCount++;
            ShemaInfo[ShemaInfo.Count - 1] += "{" + argCount.ToString();
        }
        public void SaveListEnd()
        {
            TabCount--;
            ShemaInfo[ShemaInfo.Count - 1] += "}";
        }*/

        public ConvertInfo GetCurrent()
        {
            return ConvertInfo[LoadNumber - 1];
        }


        public bool CompareInfo(ConvertInfo argInfo)
        {
            ConvertInfo LoadedInfo = ConvertInfo[LoadNumber - 1];
            bool ret = false;
            if (LoadedInfo.IsDictionary == argInfo.IsDictionary && LoadedInfo.IsList == argInfo.IsList)
            {
                if (LoadedInfo.IsDictionary == true)
                {
                    if (LoadedInfo.KeyType == argInfo.KeyType && LoadedInfo.ValueType == argInfo.ValueType &&
                        LoadedInfo.ListType == argInfo.ListType && LoadedInfo.ListCreateMode == argInfo.ListCreateMode && LoadedInfo.Tag == argInfo.Tag)
                    {
                        ret = true;
                    }
                }
                else if (LoadedInfo.IsList == true)
                {
                    if (LoadedInfo.ListType == argInfo.ListType && LoadedInfo.ListCreateMode == argInfo.ListCreateMode && LoadedInfo.Tag == argInfo.Tag)
                    {
                        ret = true;
                    }
                }
                else
                {
                    if (LoadedInfo.Type == argInfo.Type && LoadedInfo.Tag == argInfo.Tag)
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        public void ClearInfo()
        {
			ShemaInfo.Clear();
		}


		public void UpdateInfo(ConvertInfo convertInfo)
        {
            //string tab = "\t".Repeat(TabCount);

            string info = "3-";
            if (convertInfo.IsDictionary)
            {
                info = "1-";

				int keyTypeId = AddType(convertInfo.KeyType);
				int valueTypeId = AddType(convertInfo.ValueType);

                int isList = 0;
                int listTypeId = 0;
                if (convertInfo.IsList)
                {
                    isList = 1;
					listTypeId = AddType(convertInfo.ListType);
                }

                info += keyTypeId.ToString() + "-" + valueTypeId.ToString();
                info += "-" + isList.ToString() + "-" + listTypeId.ToString() + "-" + ((int)convertInfo.ListCreateMode).ToString();
            }
            else if (convertInfo.IsList)
            {
                info = "2-";

				int listTypeId = AddType(convertInfo.ListType);

                info += listTypeId.ToString() + "-" + ((int)convertInfo.ListCreateMode).ToString();
            }
            else
            {
                int typeId = AddType(convertInfo.Type);
                info += typeId.ToString();
            }

            int tagId = TagList.Count;
            if (convertInfo.Tag != null)
            {
                if (TagList.ContainsKey(convertInfo.Tag) == false)
                {
                    TagList.Add(convertInfo.Tag, tagId);
                }
                tagId = TagList[convertInfo.Tag];
            }
            else
            {
                tagId = 0;
            }

            info += "-" + tagId.ToString();

            ShemaInfo.Add(info);
        }

        private int AddType(Type argType)
        {
            return AddType(argType.ToString());
        }

		private int AddType(string argTypeName)
        {
			if (TypeList.ContainsKey(argTypeName) == false)
			{
				TypeList.Add(argTypeName, TypeList.Count);
			}
            return TypeList[argTypeName];
		}


		public void UpdateClass(IDataSave data, string argVersion)
        {
            string key = data.ClassName;
            if (argVersion != "")
            {
                key += "-" + argVersion;
            }

            if (key != "")
            {
                if (ShemaSaved.ContainsKey(key) == false)
                {
                    ShemaSaved.Add(key, new List<string>());

					foreach (string property in data.PropertyName)
					{
						if (ShemaSaved[key].Contains(property) == false)
						{

							ShemaSaved[key].Add(property);
						}
					}
				}
			}
        }

        public List<string> Save()
        {
            List<string> list = new List<string>();

            string shemaType = "";
            foreach (var item in TypeList)
            {
                shemaType += "\t" + item.Key + "\n";
            }
            list.Add(shemaType);

            string shemaTag = "";
            foreach (var item in TagList)
            {
                shemaTag += "\t" + item.Key + "\n";
            }
			list.Add(shemaTag);

			string shema = "";
			foreach (var item in ShemaSaved)
			{
				shema += item.Key + "\n{\n";
				foreach (string value in item.Value)
				{
					shema += "\t" + value + "\t" + "\n";
				}
				shema += "}\n";
			}
			list.Add(shema);

			string dataTypes = "";
            foreach (var item in ShemaInfo)
            {
                dataTypes += item + "\n";
            }
			list.Add(dataTypes);
            return list;
		}


        public void LoadType(string argInfo)
        {
			string[] shema = argInfo.Split("\n");
            foreach (string it in shema)
            {
                string item = it.Trim();
                if (item != "null" && item != "")
                {
                    Type type = Type.GetType(item);
                    // Если это не базовый тип, и его не находим в текущей dll, то ищем 
                    // или в дефолтной сборке или в движке Unity
                    if (type == null)
                    {
                        type = Type.GetType(item + ", Assembly-CSharp");
                    }
                    if (type == null)
                    {
                        type = Type.GetType(item + ", UnityEngine");
                    }
                    if (type == null)
                    {
                        type = GetType(item);
                    }

                    TypeListR.Add(TypeListR.Count, type);
                }
            }
		}

        private Type GetType(string typeName)
        {
            Type type = GetTypeExternalAssemblys(typeName);
            if (type == null)
            {
                int index = typeName.IndexOf("`1");
                if (index != -1)
                {
                    index += 2;
                    type = GetTypeExternalAssemblys(typeName.Substring(0, index));
                    Type genericType = GetTypeExternalAssemblys(typeName.Substring(index + 1, typeName.Length - index - 2));
                    if (genericType != null)
                    {
                        type = type.MakeGenericType(genericType);
                    }
                }
            }
            return type;
        }

        private Type GetTypeExternalAssemblys(string typeName)
        {
            Type type = null;
            for (int i = 0; i < ExternalAssemblys.Count; i++)
            {
                if (type == null)
                {
                    type = Type.GetType(typeName + ", " + ExternalAssemblys[i]);
                }
                else
                {
                    break;
                }
            }
            return type;
        }

        public void LoadTag(string argInfo)
		{
			string[] shema = argInfo.Split("\n");
			foreach (string it in shema)
			{
				string item = it.Trim();
				if (item != "null")
				{
					TagListR.Add(TagListR.Count, item);
				}
			}
		}

		public void LoadShema(string argInfo)
		{
            string[] shema = argInfo.Split("\n");

			ShemaLoaded = new Dictionary<string, List<string>>();
			string header = "";
			bool IsBegin = false;
			foreach (string item in shema)
			{
				string line = item.Trim();
				if (line != "")
				{
					if (IsBegin)
					{
						if (line == "}")
						{
							IsBegin = false;
						}
						else
						{
							ShemaLoaded[header].Add(line);
						}
					}
					else
					{
						if (line == "{")
						{
							IsBegin = true;
							ShemaLoaded.Add(header, new List<string>());
						}
						else
						{
							header = line;
						}
					}
				}
			}
		}

		public void LoadDataTypes(string argInfo)
		{
			string[] shema = argInfo.Split("\n");
            int ii = 0;
			foreach (string it in shema)
			{
                ii++;
                if (ii == 1700)
                {
                    int a = 1;
                }
                
                string item = it.Trim();

				if (TypeListR.Count > 1 && item != "}")
				{
					string[] line = item.Split("-");
					string tag = null;
					ConvertInfo convertInfo = null;

					if (line[0] == "1")
					{
						int keyId = int.Parse(line[1]);
						int valueTypeId = int.Parse(line[2]);
						Type keyType = TypeListR[keyId];
						Type valueType = TypeListR[valueTypeId];
						int isList = int.Parse(line[3]);
						bool isList2 = false; if (isList == 1) { isList2 = true; }

						int listTypeId = int.Parse(line[4]);
						Type listType = TypeListR[listTypeId];
						ListCreateMode mode = (ListCreateMode)int.Parse(line[5]);
						tag = TagListR[int.Parse(line[6])];

						convertInfo = new ConvertInfo(null, null, null, isList2, listType, mode, true, keyType, valueType);
						convertInfo.Tag = tag;
					}
					else if (line[0] == "2")
					{
						int listTypeId = int.Parse(line[1]);
						Type listType = TypeListR[listTypeId];
						ListCreateMode mode = (ListCreateMode)int.Parse(line[2]);
						tag = TagListR[int.Parse(line[3])];

						convertInfo = new ConvertInfo(null, null, null, true, listType, mode);
						convertInfo.Tag = tag;
					}
					else if (line[0] == "3")
					{
						int typeId = int.Parse(line[1]);
						Type type = TypeListR[typeId];
						int tagId = int.Parse(line[2]);
						if (TagListR.ContainsKey(tagId))
						{
							tag = TagListR[tagId];
						}

						convertInfo = new ConvertInfo(null, type);
						convertInfo.Tag = tag;
					}

					ConvertInfo.Add(convertInfo);
				}

			}

		}


	}
}