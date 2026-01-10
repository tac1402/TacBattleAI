// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2021 Sergej Jakovlev

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tac.DConvert
{

    public class DirectConvert
    {
        private List<ConvertInfo> ConvertInfo = new List<ConvertInfo>();
        private Queue<ObjectInfo> ConvertInfoQ = new Queue<ObjectInfo>();

        private Dictionary<string, Type> KnowConverter = new Dictionary<string, Type>();

        private SaveMetaData Meta = new SaveMetaData();
        private Shema Shema = new Shema();


        private Type converterType;
        private object converter;

        public DirectConvert()
        {
            KnowConverter.Add(typeof(Vector2).ToString(), typeof(Vector2_));
            KnowConverter.Add(typeof(Vector3).ToString(), typeof(Vector3_));
            KnowConverter.Add(typeof(Transform).ToString(), typeof(Transform_));
            KnowConverter.Add(typeof(Guid).ToString(), typeof(Guid_));
        }

        public SaveMetaData Connect()
        {
            Meta.Shema = Shema;
            return Meta;
        }

        #region IVersion
        protected float version;
        public float Version
        {
            get { return version; }
            set { version = value; }
        }
        #endregion

        public void Clear()
        {
            ConvertInfo.Clear();
            ConvertInfoQ.Clear();
            OnLoadStep = null;
        }

        public void AddExternalAssembly(string argAssemblyName)
        {
            Shema.ExternalAssemblys.Add(argAssemblyName);
        }

        public void Set<T>(T argObject, ListCreateMode argListCreateMode = ListCreateMode.Recreate)
        {
            Set(new ObjectInfo().Add(argObject), argListCreateMode);
        }

        public void Set(ObjectInfo argObject, ListCreateMode argListCreateMode = ListCreateMode.Recreate)
        {
            if (argObject != null)
            {
                Type t = argObject.Type;

                if (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    Type[] arguments = t.GetGenericArguments();
                    Type listType = arguments[0];

                    ConvertInfo.Add(new ConvertInfo(argObject.Object, t, null, true, listType, argListCreateMode));
                }
                else if (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    Type[] arguments = t.GetGenericArguments();
                    Type keyType = arguments[0];
                    Type valueType = arguments[1];

                    bool IsList = false;
                    Type listType = null;
                    if (valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        IsList = true;
                        Type[] arguments2 = valueType.GetGenericArguments();
                        listType = arguments2[0];
                    }
                    ConvertInfo.Add(new ConvertInfo(argObject.Object, t, null, IsList, listType, argListCreateMode, true, keyType, valueType));
                }
                else
                {
                    ConvertInfo.Add(new ConvertInfo(argObject.Object, t));
                }
            }
            else
            {
                ConvertInfo.Add(null);
            }
        }

        public T Get<T>(T argObject)
        {
            return (T)ConvertInfoQ.Dequeue().Object;
        }

        public void Save(string argFileName, ConvertorType argConvertorType)
        {
            level = 1;
            Shema.ClearInfo();

            Convertor bin = GetConvertor(argConvertorType);
            bin.Open(argFileName, false);
            Save(bin);
            bin.Close();
            //Shema.Save(argFileName.Replace(".bin", ""));
        }

        public byte[] BinSave()
        {
            level = 1;
            Shema.ClearInfo();

            BinaryConvertor bin = new BinaryConvertor(); ;
            bin.Open();
            Save(bin);
            bin.Close();
            return bin.Buffer;
        }

        int level = 0;

        private void Save(Convertor bin, bool BreakBefor = false)
        {
            for (int i = 0; i < ConvertInfo.Count; i++)
            {
                if (level == 1)
                {

                    if (i == 4)
                    {
                        int a = 1;
                    }
                }

                Shema.UpdateInfo(ConvertInfo[i]);

                if (BreakBefor) { bin.WriteBreak(); }
                if (ConvertInfo[i].IsDictionary == true)
                {
                    SaveDictionary(ConvertInfo[i].Object, ConvertInfo[i].ValueType,
                        ConvertInfo[i].IsList, ConvertInfo[i].ListType, ConvertInfo[i].Tag, bin);
                    //Shema.SaveListEnd();
                }
                else if (ConvertInfo[i].IsList == true)
                {
                    SaveList(ConvertInfo[i].Object, ConvertInfo[i].Type, ConvertInfo[i].Tag, bin);
                    //Shema.SaveListEnd();
                }
                else
                {
                    SaveClassType(ConvertInfo[i].Object, ConvertInfo[i].Type, ConvertInfo[i].Tag, bin);
                }
                if (!BreakBefor) { bin.WriteBreak(); }
            }
        }


        public byte[] DampLoad(string argDampTxt)
        {
            return DampLoad(Array.ConvertAll<string, byte>(argDampTxt.Split('-'), s => Convert.ToByte(s, 16)));
        }


        public byte[] DampLoad(byte[] argBuffer)
        {
            BinaryConvertor bin = new BinaryConvertor(); ;
            bin.Open(argBuffer);
            Load(bin);
            byte[] ret = bin.Buffer;
            bin.Close();
            return ret;
        }

        public void Load(string argFileName, ConvertorType argConvertorType)
        {
            if (IsDebugMode)
            {
                if (File.Exists("LoadLog.txt"))
                {
                    File.Delete("LoadLog.txt");
                }
                //DebugLoad.DebugList = new List<string>();
            }

            //Shema.Load(argFileName.Replace(".bin", ""));
            Convertor bin = GetConvertor(argConvertorType);
            bin.Open(argFileName, true);
            Load(bin);
            bin.Close();
        }

        public delegate void IndexInfo(int argIndex);
        public event IndexInfo OnLoadStep;

        public bool IsDebugMode = true;

        public IEntity IEntity;

        private void Load(Convertor bin, bool BreakBefor = false)
        {
            DateTime begin = DateTime.Now;
            int debugId = 0;

            for (int i = 0; i < ConvertInfo.Count; i++)
            {
                Shema.LoadNumber++;

                if (IsDebugMode)
                {
                    string info = "";

                    info += Shema.LoadNumber.ToString();
                    if (ConvertInfo[i] != null && ConvertInfo[i].Object != null)
                    {
                        info += ". " + ConvertInfo[i].Object.ToString();
                    }
                    else
                    {
                        info += ". NULL";
                    }

                    info += "\n";

                    File.AppendAllText("LoadLog.txt", info);

                    if (Shema.LoadNumber == 7940)
                    {
                        int a = 1;
                    }

                }

                //bool ret = Shema.CompareInfo(ConvertInfo[i]);

                if (ConvertInfo[i] == null)
                {
                    ConvertInfo[i] = Shema.GetCurrent();
                    int a = 1;
                }

                if (BreakBefor) { bin.ReadBreak(); }
                if (ConvertInfo[i].IsDictionary == true)
                {
                    ConvertInfo[i].Object = LoadDictionary(ConvertInfo[i].Object, ConvertInfo[i].KeyType, ConvertInfo[i].ValueType,
                        ConvertInfo[i].IsList, ConvertInfo[i].ListType, ConvertInfo[i].ListCreateMode, ConvertInfo[i].Tag, bin);
                }
                else if (ConvertInfo[i].IsList == true)
                {
                    if (ConvertInfo[i].Tag == PredefinedTag.UseCurrent.ToString())
                    {
                        ConvertInfo[i].ListCreateMode = ListCreateMode.UseCurrent;
                    }
                    /*else if (ConvertInfo[i].Tag == PredefinedTag.UseCurrentPrefabId.ToString())
                    {
						ConvertInfo[i].ListCreateMode = ListCreateMode.UseCurrent;
                        ConvertInfo[i].Tag = PredefinedTag.OnlyPrefabId.ToString();
					}*/

                    ConvertInfo[i].Object = LoadList(ConvertInfo[i].Object, ConvertInfo[i].ListType, ConvertInfo[i].ListCreateMode, ConvertInfo[i].Tag, bin);
                }
                else
                {
                    ConvertInfo[i].Object = LoadClassType(ConvertInfo[i].Object, ConvertInfo[i].Type, ConvertInfo[i].Tag, bin);

                    if (ConvertInfo[i].Tag == PredefinedTag.UseCurrentPrefabId.ToString())
                    {
                        ConvertInfo[i].Object = GetPrefab(ConvertInfo[i].Object, ConvertInfo[i].Type, true);
                        if (ConvertInfo[i].Object != null)
                        {
                            ConvertInfo[i].Object = LoadClassType(ConvertInfo[i].Object, ConvertInfo[i].Type, "", bin, PrefabCreateMode.Prefab);
                        }
                    }

                    /*if (ConvertInfo[i].Tag == PredefinedTag.SaveInCaсhe.ToString())
					{
						IEnumerable idList = ConvertInfo[i].Object as IEnumerable;

						if (idList != null)
                        { 
                            foreach (var item in idList) 
                            {
                                IId iid = item as IId;
                                if (iid != null)
                                {
                                    IEntityFactory.AddInCache(iid.Id, item);
                                }
							}
						}

					}*/
                }

                ConvertInfoQ.Enqueue(new ObjectInfo().Add(ConvertInfo[i].Object));
                if (!BreakBefor) { bin.ReadBreak(); }

                if (OnLoadStep != null)
                {
                    OnLoadStep(i);
                }

            }
        }

        private Convertor GetConvertor(ConvertorType argConvertorType)
        {
            Convertor bin = null;
            switch (argConvertorType)
            {
                case ConvertorType.Text:
                    bin = new TextConvertor();
                    break;
                case ConvertorType.Bin:
                    bin = new BinaryConvertor();
                    break;
            }
            return bin;
        }

        private IDictionary LoadDictionary<T>(T argValue, Type argKeyType, Type argValueType,
            bool argIsList, Type argListType, ListCreateMode argListCreateMode, string argTag, Convertor bin)
        {
            IDictionary locDic = argValue as IDictionary;
            if (locDic != null)
            {
                locDic.Clear();

                bin.IndentLength++;
                bin.ReadBreak();
                int locCount = bin.Load(locDic.Count, locDic.Count.GetType().ToString());
                for (int n = 0; n < locCount; n++)
                {
                    object locKey = GetDefault(argKeyType);
                    object locValue = Activator.CreateInstance(argValueType);

                    bin.ReadBreak();
                    locKey = bin.Load(locKey, argKeyType.ToString(), 0, argKeyType.IsEnum);
                    if (argIsList == true)
                    {
                        locValue = LoadList(locValue, argListType, argListCreateMode, argTag, bin); ;
                    }
                    else
                    {
                        locValue = LoadClassType(locValue, argValueType, argTag, bin);
                    }
                    locDic.Add(locKey, locValue);
                }
                bin.IndentLength--;
            }
            return locDic;
        }

        private object CreateInstance(Type argType)
        {
            object ret = null;
            switch (argType.ToString())
            {
                case "System.String":
                    ret = "";
                    break;
                default:
                    ret = Activator.CreateInstance(argType);
                    break;
            }
            return ret;
        }

        public string ErrorLoadInList = "";

        private IList LoadList<T>(T argValue, Type argListType, ListCreateMode argListCreateMode, string argTag, Convertor bin)
        {
            IList locList = argValue as IList;
            if (locList != null)
            {
                //IPrefabCreate<int, C> prefabInfoInt = null;
                if (argListCreateMode == ListCreateMode.Recreate || argListCreateMode == ListCreateMode.CreateFromPrefab)
                {
                    locList.Clear();
                }

                bin.IndentLength++;
                bin.ReadBreak();
                int locCount = bin.Load(locList.Count, locList.Count.GetType().ToString());
                for (int k = 0; k < locCount; k++)
                {
                    ErrorLoadInList = k.ToString() + "-" + argListType.ToString();

                    if (k == 5)
                    {
                        int a = 1;
                    }

                    if (argListCreateMode == ListCreateMode.Recreate || argListCreateMode == ListCreateMode.CreateFromPrefab)
                    {
                        object item = CreateInstance(argListType);
                        locList.Add(item);
                    }

                    bin.ReadBreak();

                    PrefabCreateMode locPrefabCreateMode = PrefabCreateMode.None;
                    if (argListCreateMode == ListCreateMode.CreateFromPrefab)
                    {
                        locPrefabCreateMode = PrefabCreateMode.PreparePrefab;
                    }
                    else if (argListCreateMode == ListCreateMode.UseCurrent)
                    {
                        argTag = PredefinedTag.UseCurrent.ToString();
                    }

                    object locListItem = LoadClassType(locList[k], locList[k].GetType(), argTag, bin, locPrefabCreateMode);

                    if (argListCreateMode == ListCreateMode.CreateFromPrefab)
                    {
                        locList[k] = GetPrefab(locListItem, argListType);
                        locListItem = LoadClassType(locList[k], locList[k].GetType(), argTag, bin, PrefabCreateMode.Prefab);
                    }

                    locList[k] = locListItem;
                }
                bin.IndentLength--;
                ErrorLoadInList = "";
            }
            return locList;
        }

        private object GetPrefab(object argItem, Type argType, bool argAllowNull = false)
        {
            IVersion version = argItem as IVersion;

            IPrefabId prefabInfoInt = argItem as IPrefabId;

            GameObject gameObject = null;
            if (prefabInfoInt != null)
            {
                gameObject = IEntity.GetObject(prefabInfoInt.Id);
                if (gameObject == null)
                {
                    if (argAllowNull == false)
                    {
                        gameObject = IEntity.CreatePrefab(prefabInfoInt.Id, prefabInfoInt.PrefabName);
                    }
                }
            }

            object item = null;
            if (gameObject != null)
            {
                item = gameObject.GetComponent(argType);

                IVersion version2 = item as IVersion;
                if (version != null && version2 != null)
                {
                    version2.LoadVersion = version.LoadVersion;
                }

                IPrefabId newItemInt = item as IPrefabId;
                if (newItemInt != null)
                {
                    newItemInt.Id = prefabInfoInt.Id;
                    newItemInt.PrefabName = prefabInfoInt.PrefabName;
                }
            }
            return item;
        }



        private object LoadClassType<T>(T argValue, Type argType, string argTag, Convertor bin, PrefabCreateMode argPrefabPrepare = PrefabCreateMode.None)
            where T : class
        {
            object ret = null;


            IDataSave data = null;
            if (argValue == null)
            {
                object obj = CreateInstance(argType);
                data = obj as IDataSave;
            }
            else
            {
                data = argValue as IDataSave;
            }

            if (data != null) // Объект
            {
                //if (argValue != null)
                {
                    int locStart = 0;
                    //int locCount = 0;

                    IPrefabId prefabCreateInt = data as IPrefabId;
                    data.DataTag = argTag;
                    string ver = "";
                    if (argPrefabPrepare == PrefabCreateMode.PreparePrefab || argTag == PredefinedTag.UseCurrentPrefabId.ToString())
                    {
                        if (argTag == PredefinedTag.UseCurrentPrefabId.ToString())
                        {
                            argPrefabPrepare = PrefabCreateMode.PreparePrefab;
                        }

                        bin.IndentLength++;

                        IVersion version = argValue as IVersion;
                        if (version != null)
                        {
                            object v = 1.00f;
                            bin.ReadBreak();
                            version.LoadVersion = (float)LoadFastType(v, typeof(float), bin);
                        }

                        if (prefabCreateInt != null)
                        {
                            bin.ReadBreak();
                            object v = 0;
                            prefabCreateInt.Id = (int)LoadFastType(v, typeof(int), bin);
                            if (prefabCreateInt.Id != 0)
                            {
                                bin.ReadBreak();
                                prefabCreateInt.PrefabName = LoadFastType("", typeof(string), bin).ToString();
                            }
                        }

                        bin.IndentLength--;
                    }
                    else if (argPrefabPrepare == PrefabCreateMode.Prefab)
                    {
                        locStart = 2;
                        Shema.LoadNumber += 2;
                        data.SaveData(false);
                        //locCount = data.Data.Count;
                    }
                    else
                    {

                        if (argValue != null && argTag != PredefinedTag.OnlyPrefabId.ToString())
                        {
                            IVersion version = argValue as IVersion;
                            if (version != null)
                            {
                                object v = 1.00f;
                                bin.IndentLength++;
                                bin.ReadBreak();
                                float loadVersion = (float)LoadFastType(v, typeof(float), bin);
                                version.LoadVersion = loadVersion;
                                ver = "-" + loadVersion.ToString("F4");
                                bin.IndentLength--;
                            }
                            data.SaveData(false);
                        }
                        else
                        {
                            data = OnlyId(data);
                        }

                        //locCount = data.Data.Count;
                    }

                    bool IsGameObject = false;
                    if (argPrefabPrepare == PrefabCreateMode.None || argPrefabPrepare == PrefabCreateMode.Prefab)
                    {

                        ObjectInfo[] locData = data.Data.ToArray();

                        ObjectInfo[] tmpData = null;

                        List<int> Remap = new List<int>();
                        List<ObjectInfo> Remap2 = new List<ObjectInfo>();

                        if (Shema.ShemaLoaded != null)
                        {
                            if (data.ClassName != "" && Shema.ShemaLoaded.ContainsKey(data.ClassName + ver))
                            {
                                int oldCount = Shema.ShemaLoaded[data.ClassName + ver].Count;
                                int newCount = locData.Length;

                                int maxCount = Math.Max(oldCount, newCount);

                                tmpData = new ObjectInfo[oldCount];

                                for (int i = 0; i < maxCount; i++)
                                {
                                    Remap.Add(-1);
                                }
                                for (int i = 0; i < maxCount; i++)
                                {
                                    Remap2.Add(null);
                                }

                                for (int i = 0; i < oldCount; i++)
                                {
                                    if (i > data.PropertyName.Count - 1 || data.PropertyName[i] != Shema.ShemaLoaded[data.ClassName + ver][i])
                                    {
                                        int index = GetShemaIndex(data.PropertyName, Shema.ShemaLoaded[data.ClassName + ver][i]);
                                        if (index != -1)
                                        {
                                            tmpData[i] = locData[index].Clone();
                                            Remap[index] = i;
                                        }
                                        else
                                        {
                                            // Нового значения в сохранении нет, оставим загрузку по старому
                                            // информация о типе загрузится позже
                                            if (IsDebugMode)
                                            {
                                                int a = 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tmpData[i] = locData[i].Clone();
                                        Remap[i] = i;
                                    }
                                }
                                for (int i = 0; i < newCount; i++)
                                {
                                    int index = GetShemaIndex(Shema.ShemaLoaded[data.ClassName + ver], data.PropertyName[i]);
                                    if (index == -1)
                                    {
                                        Remap2[i] = locData[i].Clone();
                                    }
                                }

                                locData = tmpData;
                            }
                            else // В этом случае появился новый класс/тип, для свойства с прежним названием
                            {
                                if (argTag == PredefinedTag.OnlyPrefabId.ToString())
                                {
                                    Remap.Add(0);
                                    Remap2.Add(null);
                                }
                                else
                                {
                                    if (IsDebugMode)
                                    {
                                        if (Shema.ShemaLoaded.ContainsKey(data.ClassName + ver) == false)
                                        {
                                            string info = "Error: not info about - " + data.ClassName + "\n";
                                            File.AppendAllText("LoadLog.txt", info);
                                        }
                                    }
                                }
                            }
                        }

                        DirectConvert directConvert = new DirectConvert();

                        bin.IndentLength++;

                        int k = 0;
                        for (int j = locStart; j < locData.Length; j++)
                        {
                            CheckObjectTag(locData[j]);
                            directConvert.Set(locData[j]);
                            if (directConvert.ConvertInfo[k] != null)
                            {
                                directConvert.ConvertInfo[k].Tag = currentTag;
                            }
                            k++;
                        }

                        directConvert.IEntity = IEntity;
                        directConvert.Shema = Shema;
                        directConvert.IsDebugMode = IsDebugMode;
                        directConvert.Load(bin, true);

                        data.Data.Clear();

                        if (argPrefabPrepare == PrefabCreateMode.Prefab)
                        {
                            if (prefabCreateInt != null)
                            {
                                data.Data.Enqueue(new ObjectInfo().Add(prefabCreateInt.Id));
                                data.Data.Enqueue(new ObjectInfo().Add(prefabCreateInt.PrefabName));
                            }
                        }

                        List<ObjectInfo> q_old = new List<ObjectInfo>();
                        List<ObjectInfo> q_new = new List<ObjectInfo>();
                        for (int j = locStart; j < locData.Length; j++)
                        {
                            q_old.Add(directConvert.ConvertInfoQ.Dequeue());
                        }

                        //remap
                        if (Remap.Count != 0)
                        {
                            for (int j = locStart; j < Remap.Count; j++)
                            {
                                if (Remap[j] != -1)
                                {
                                    q_new.Add(q_old[Remap[j] - locStart]);
                                }
                            }

                            List<ObjectInfo> q_remap = new List<ObjectInfo>();
                            int n = 0;
                            for (int i = locStart; i < Remap2.Count; i++)
                            {
                                if (Remap2[i] == null)
                                {
                                    if (n < q_new.Count)
                                    {
                                        q_remap.Add(q_new[n]);
                                        n++;
                                    }
                                }
                                else
                                {
                                    q_remap.Add(Remap2[i]);
                                }
                            }
                            q_new = q_remap;
                        }
                        else
                        {
                            q_new = q_old;
                        }



                        for (int j = 0; j < q_new.Count; j++)
                        {
                            data.Data.Enqueue(q_new[j]);
                        }

                        bin.IndentLength--;

                        if (argValue != null && argTag != PredefinedTag.OnlyPrefabId.ToString())
                        {
                            data.SaveData(true);
                        }
                        else
                        {
                            if (prefabCreateInt != null)
                            {
                                prefabCreateInt.Id = (int)data.Data.Dequeue().Object;
                            }
                        }

                        if (argPrefabPrepare == PrefabCreateMode.None && argTag != PredefinedTag.UseCurrent.ToString())
                        {
                            IPrefabId pcInt = data as IPrefabId;

                            if (pcInt != null && pcInt.Id != 0)
                            {
                                GameObject o = IEntity.GetObject(pcInt.Id);
                                if (o == null)
                                {
                                    o = IEntity.CreatePrefab(pcInt.Id, pcInt.PrefabName);
                                }
                                if (o != null)
                                {
                                    IsGameObject = true;
                                    ret = o.GetComponent(argType);

									if (ret == null)
									{
                                        ret = o.GetComponent<Item>();
								    }
								}
							}
                        }

                    }

                    if (IsGameObject == false)
                    {
                        ret = argValue;
                    }
                    if (argTag == PredefinedTag.UseCurrentPrefabId.ToString() && prefabCreateInt.Id != 0)
                    {
                        ret = data;
                    }


                }

            }
            else // простой тип
            {
                ret = LoadFastType(argValue, argType, bin);
            }

            return ret;
        }

        private int GetShemaIndex(List<string> argPropertyName, string argFindedProperty)
        {
            int ret = -1;
            for (int i = 0; i < argPropertyName.Count; i++)
            {
                if (argPropertyName[i] == argFindedProperty)
                {
                    ret = i;
                    break;
                }
            }
            return ret;
        }

        private object LoadFastType<T>(T argValue, Type argType, Convertor bin)
            where T : class
        {
            object ret = null;

            CheckConvertor(argValue, argType);
            if (converter != null)
            {
                converter = bin.Load(converter, converterType.ToString(), argValue);
                ret = ConvertorTo();
            }
            else
            {
                ret = bin.Load(argValue, argType.ToString(), argValue, argType.IsEnum);
            }

            return ret;
        }

        private void SaveDictionary<T>(T argValue, Type argValueType, bool argIsList,
            Type argListType, string argTag, Convertor bin)
        {
            IDictionary locDic = argValue as IDictionary;
            if (locDic != null)
            {
                List<object> tempKeys = new List<object>();
                foreach (var item in locDic.Keys)
                {
                    tempKeys.Add(item);
                }

                bin.IndentLength++;
                bin.WriteBreak();

                // Shema.SaveListInfo(tempKeys.Count);
                bin.Add(tempKeys.Count, tempKeys.Count.GetType().ToString());
                int keyIndex = 0;
                foreach (var item in locDic.Values)
                {
                    bin.WriteBreak();

                    Type keyType = tempKeys[keyIndex].GetType();
                    bin.Add(tempKeys[keyIndex], keyType.ToString(), keyType.IsEnum);
                    if (argIsList == true)
                    {
                        SaveList(item, argListType, argTag, bin);
                    }
                    else
                    {
                        SaveClassType(item, argValueType, argTag, bin);
                    }
                    keyIndex++;
                }
                bin.IndentLength--;
            }
        }
        private void SaveList<T>(T argValue, Type argType, string argTag, Convertor bin)
        {
            IList locList = argValue as IList;
            if (locList != null)
            {
                bin.IndentLength++;
                bin.WriteBreak();

                //Shema.SaveListInfo(locList.Count);
                bin.Add(locList.Count, locList.Count.GetType().ToString());
                foreach (var item in locList)
                {
                    bin.WriteBreak();
                    SaveClassType(item, argType, argTag, bin);
                }
                bin.IndentLength--;
            }
        }

        private IDataSave OnlyId(IDataSave data)
        {
            IId prefabCreateInt = data as IId;

            data.Data.Clear();
            if (prefabCreateInt != null)
            {
                /*if (prefabCreateInt.UniqueObjectId == 0)
                {
                    Debug.LogError("SaveError: UniqueObjectId == 0");
                }*/

                if (prefabCreateInt.Id != 0)
                {
                    int a = 1;
                }


                data.Data.Enqueue(new ObjectInfo().Add(prefabCreateInt.Id));
                //prefabCreateInt.EFactory.AddIdPrefab(prefabCreateInt.Id, prefabCreateInt.PrefabName);
            }
            return data;
        }

        private void SaveClassType<T>(T argValue, Type argType, string argTag, Convertor bin)
        {
            if (argType.ToString() == "Route")
            {
                int a = 1;
            }


            IDataSave data = null;
            if (argValue == null)
            {
                object obj = Activator.CreateInstance(argType);
                data = obj as IDataSave;
            }
            else
            {
                data = argValue as IDataSave;
            }

            if (data != null)
            {
                bin.IndentLength++;

                DirectConvert directConvert = new DirectConvert();

                data.DataTag = argTag;

                if (argValue != null && data.DataTag != PredefinedTag.OnlyPrefabId.ToString())
                {
                    data.SaveData(false);

                    IVersion version = argValue as IVersion;
                    string ver = "";
                    if (version != null)
                    {
                        float v = version.LoadVersion;
                        ver = v.ToString("F4");
                        bin.WriteBreak();
                        bin.Add(v, v.GetType().ToString());
                    }

                    Shema.UpdateClass(data, ver);
                }
                else
                {
                    data = OnlyId(data);
                }

                ObjectInfo[] locData = data.Data.ToArray();
                for (int j = 0; j < data.Data.Count; j++)
                {
                    CheckObjectTag(locData[j]);
                    directConvert.Set(locData[j]);
                    directConvert.ConvertInfo[j].Tag = currentTag;
                }

                directConvert.Shema = Shema;
                directConvert.Save(bin, true);

                bin.IndentLength--;
            }
            else
            {
                SaveFastType(argValue, argType, bin);
            }
        }
        private void SaveFastType<T>(T argValue, Type argType, Convertor bin)
        {
            CheckConvertor(argValue, argType);
            if (converter != null)
            {
                bin.Add(converter, converterType.ToString());
            }
            else
            {
                bin.Add(argValue, argType.ToString(), argType.IsEnum);
            }
        }



        private string currentTag;
        private void CheckObjectTag(ObjectInfo argObject)
        {
            currentTag = "";
            if (argObject != null && argObject.Tag != null)
            {
                currentTag = argObject.Tag;
            }
        }


        private void CheckConvertor(object argObject, Type argType, bool Load = false)
        {
            converterType = null;
            converter = null;
            Type t = argType;

            if (KnowConverter.ContainsKey(t.ToString()))
            {
                converterType = KnowConverter[t.ToString()];
            }

            if (converterType != null)
            {
                switch (converterType.ToString())
                {
                    case "TAC.DConvert.Vector2_":
                        converter = Activator.CreateInstance(converterType) as ICustomConvert<Vector2_, Vector2>;
                        if (converter != null)
                        {
                            if (Load == false)
                            {
                                ((ICustomConvert<Vector2_, Vector2>)converter).ConvertFrom((Vector2)argObject);
                            }
                        }
                        break;
                    case "TAC.DConvert.Vector3_":
                        converter = Activator.CreateInstance(converterType) as ICustomConvert<Vector3_, Vector3>;
                        if (converter != null)
                        {
                            if (Load == false)
                            {
                                ((ICustomConvert<Vector3_, Vector3>)converter).ConvertFrom((Vector3)argObject);
                            }
                        }
                        break;
                    case "TAC.DConvert.Transform_":
                        converter = Activator.CreateInstance(converterType) as ICustomConvert<Transform_, Transform>;
                        if (converter != null)
                        {
                            if (Load == false)
                            {
                                ((ICustomConvert<Transform_, Transform>)converter).ConvertFrom((Transform)argObject);
                            }
                        }
                        break;
                    case "TAC.DConvert.Guid_":
                        converter = Activator.CreateInstance(converterType) as ICustomConvert<Guid_, Guid>;
                        if (converter != null)
                        {
                            if (Load == false)
                            {
                                ((ICustomConvert<Guid_, Guid>)converter).ConvertFrom((Guid)argObject);
                            }
                        }
                        break;
                }
            }
        }
        private object ConvertorTo()
        {
            object ret = null;
            if (converterType != null)
            {
                if (converter != null)
                {
                    switch (converterType.ToString())
                    {
                        case "TAC.DConvert.Vector2_":
                            ret = ((ICustomConvert<Vector2_, Vector2>)converter).ConvertTo();
                            break;
                        case "TAC.DConvert.Vector3_":
                            ret = ((ICustomConvert<Vector3_, Vector3>)converter).ConvertTo();
                            break;
                        case "TAC.DConvert.Transform_":
                            ret = ((ICustomConvert<Transform_, Transform>)converter).ConvertTo();
                            break;
                        case "TAC.DConvert.Guid_":
                            ret = ((ICustomConvert<Guid_, Guid>)converter).ConvertTo();
                            break;
                    }
                }
            }
            return ret;
        }
        public object GetDefault(Type t)
        {
            return this.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
        }
        public T GetDefaultGeneric<T>()
        {
            return default(T);
        }

    }


    public static class DebugLoad
    {
        //static public List<string> DebugList = new List<string>();
    }


    public enum ConvertorType
    { 
        Text,
        Bin
    }

    public enum ListCreateMode
    { 
        Recreate = 1,
        UseCurrent = 2,
        CreateFromPrefab = 3
    }

    public enum PrefabCreateMode
    { 
        None = 0,
        PreparePrefab = 1,
        Prefab = 2
    }

}