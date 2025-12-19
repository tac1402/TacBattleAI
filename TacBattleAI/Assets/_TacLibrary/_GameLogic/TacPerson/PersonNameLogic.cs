using System.Collections.Generic;
using System.Linq;

namespace Tac.Person
{
	public partial class PersonName
	{
		System.Random rnd;

		List<string> menFullName, menName, menSurname;
		List<string> womenFullName, womenName, womenSurname;
		List<string> allSurname;
		RandomUnique randomMenName, randomWomenName, randomSurname;

		public void LoadName(System.Random argRnd, string argMenText, string argWomenText)
		{
			rnd = argRnd;

			string[] menText = argMenText.Split('\n');
			string[] womenText = argWomenText.Split('\n');

			menFullName = new List<string>();
			womenFullName = new List<string>();
			for (int i = 0; i < menText.Length; i++)
			{
				menFullName.Add(menText[i].Substring(0, menText[i].Length - 1));
			}
			for (int i = 0; i < womenText.Length; i++)
			{
				womenFullName.Add(womenText[i].Substring(0, womenText[i].Length - 1));
			}

			menName = new List<string>();
			menSurname = new List<string>();
			for (int i = 0; i < menFullName.Count; i++)
			{
				string[] n = menFullName[i].Split(" ");

				if (menName.Contains(n[0]) == false) { menName.Add(n[0]); }
				if (menSurname.Contains(n[1]) == false) { menSurname.Add(n[1]); }
			}
			womenName = new List<string>();
			womenSurname = new List<string>();
			for (int i = 0; i < womenFullName.Count; i++)
			{
				string[] n = womenFullName[i].Split(" ");
				if (womenName.Contains(n[0]) == false) { womenName.Add(n[0]); }
				if (womenSurname.Contains(n[1]) == false) { womenSurname.Add(n[1]); }
			}

			allSurname = Enumerable.Concat(menSurname, womenSurname).ToList();

			randomMenName = new RandomUnique(rnd);
			randomWomenName = new RandomUnique(rnd);
			randomSurname = new RandomUnique(rnd);
		}

		private void CheckUsed(GenderType argGender)
		{
			if (argGender == GenderType.Men)
			{
				if (randomMenName.UsedCount > menName.Count - 50)
				{
					Generate(100, menFullName, menName, menSurname);
				}
			}
			else if (argGender == GenderType.Women)
			{
				if (randomWomenName.UsedCount > womenName.Count - 50)
				{
					Generate(100, womenFullName, womenName, womenSurname);
				}
			}
		}

		private void Generate(int argCount, List<string> argFull, List<string> argName, List<string> argSurname)
		{
			bool IsEnd = false;
			int count = 0;
			while (IsEnd == false)
			{
				int nameIndex = rnd.Next(0, argName.Count);
				string name = argName[nameIndex];
				int surnameIndex = rnd.Next(0, argSurname.Count);
				string surname = argName[surnameIndex];

				string fullname = name + " " + surname;
				if (argFull.Contains(fullname) == false)
				{
					argFull.Add(fullname);
					count++;
				}

				if (count == argCount)
				{
					IsEnd = true;
				}
			}
		}

		public string GetSurname()
		{
			string surname = "";

			int index = randomSurname.Get(allSurname.Count);
			surname = allSurname[index];

			return surname;
		}

		public string GetFamilyName(GenderType argGender, string argSurname)
		{
			string fullname = "";

			for (int i = 0; i < 10; i++)
			{
				if (argGender == GenderType.Men)
				{
					int menNameIndex = rnd.Next(0, menName.Count);
					string name = menName[menNameIndex];

					fullname = name + " " + argSurname;

					if (menFullName.Contains(fullname) == false)
					{
						menFullName.Add(fullname);
						randomMenName.MarkUsed(menFullName.Count);
						break;
					}
				}
				else if (argGender == GenderType.Women)
				{
					int womenNameIndex = rnd.Next(0, womenName.Count);
					string name = womenName[womenNameIndex];

					fullname = name + " " + argSurname;

					if (womenFullName.Contains(fullname) == false)
					{
						womenFullName.Add(fullname);
						randomWomenName.MarkUsed(womenFullName.Count);
						break;
					}
				}
			}
			return fullname;
		}


		public string GetUniqueName(GenderType argGender)
		{
			string name = "";

			CheckUsed(argGender);
			if (argGender == GenderType.Men)
			{
				int menNameIndex = randomMenName.Get(menFullName.Count);
				name = menFullName[menNameIndex];
			}
			else if (argGender == GenderType.Women)
			{
				int womenNameIndex = randomWomenName.Get(womenFullName.Count);
				name = womenFullName[womenNameIndex];
			}

			return name;
		}

	}
}
