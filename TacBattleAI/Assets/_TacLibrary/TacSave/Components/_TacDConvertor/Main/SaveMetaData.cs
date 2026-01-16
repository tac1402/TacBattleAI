// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2023 Sergej Jakovlev

using System;
using System.Collections.Generic;
using System.IO;

namespace Tac.DConvert
{
    public class SaveMetaData 
    {
		public string FileName;

		public Dictionary<string, int> KeyId = new Dictionary<string, int>();
		public int KeyIndex = 1000000;
		public const int MinKeyIndex = 1000000;

		public Shema Shema;

		private string BeginSymbol = "<";
		private string EndSymbol = ">";

		public void Init(string argFileName, bool argSaveMode)
		{
			FileName = argFileName;

			if (argSaveMode == false)
			{
				KeyId.Clear();
				KeyIndex = MinKeyIndex;

				Load();
			}
		}

		public int ReserveKey(string argKey)
		{
			int ret = -1;
			if (KeyId.ContainsKey(argKey) == false)
			{
				KeyId.Add(argKey, KeyIndex++);
			}
			ret = KeyId[argKey];
			return ret;
		}

		public void Load()
		{
			if (File.Exists(FileName + ".me"))
			{
				string all = File.ReadAllText(FileName + ".me");
				string[] parts = all.Replace(BeginSymbol + "\n", "").Split(EndSymbol + "\n");

				string[] part1 = parts[0].Split("\n");

				for (int i = 0; i < part1.Length; i++)
				{
					if (part1[i] != "")
					{
						string[] item = part1[i].Split('\t');
						int id = Convert.ToInt32(item[1]);
						if (KeyIndex < id) { KeyIndex = id; }
						KeyId.Add(item[0], id);
					}
				}
				Shema.LoadType(parts[1]);
				Shema.LoadTag(parts[2]);
				Shema.LoadShema(parts[3]);
				Shema.LoadDataTypes(parts[4]);
			}
		}


		public void Save()
		{
			string part1 = "";
			foreach (var item in KeyId)
			{
				part1 += item.Key + "\t" + item.Value.ToString() + "\n";
			}

			File.Delete(FileName + ".me");
			File.AppendAllText(FileName + ".me", BeginSymbol + "\n" + part1 + EndSymbol + "\n");

			List<string> shemaParts = Shema.Save();
			for (int i = 0; i < shemaParts.Count; i++)
			{
				File.AppendAllText(FileName + ".me", BeginSymbol + "\n" + shemaParts[i] + EndSymbol + "\n");
			}


		}

	}
}
