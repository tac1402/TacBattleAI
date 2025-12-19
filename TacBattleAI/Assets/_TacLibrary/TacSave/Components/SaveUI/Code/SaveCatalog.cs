using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SaveCatalog : MonoBehaviour
{
    public string Version = "v1.35";

    public IDayNightController DayNightController;

	public TMP_InputField PlaythroughName;

    public string Day
    {
        get 
        {
            int day = DayNightController.CurrentDay;
            string DayTxt = "Day #" + day.ToString() + " " + DayNightController.GameTime.text;
            return DayTxt;
        }
    }

    public TMP_InputField CheckPointName;
    public TMP_Text LoadName;

    public GameObject PlaythroughPrefab;
    public GameObject CheckPointPrefab;

    public GameObject PlaythroughList;
    public GameObject CheckPointList;
    public GameObject SaveButton;
    public GameObject SaveTitle;


    public DialogBox DialogYouSure;


    public Dictionary<int, Playthrough> AllPlaythrough = new Dictionary<int, Playthrough>();
    public Dictionary<string, SaveCheckPoint> AllCheckPoint = new Dictionary<string, SaveCheckPoint>();

    public int CurrentPlaythroughId = 0;
    public int CurrentCheckPointId = 0;

    public int SelectedPlaythroughId = 0;

    private ISaveManager saveManager;
    public ISaveManager SaveManager
    {
        get { return saveManager; }
        set
        {
            saveManager = value;
            saveManager.Version = Version;
        }
    }

    private void Start()
    {
		GameObject world = GameObject.Find("World");
        if (world != null)
        {
            DayNightController = world.GetComponent<IDayNightController>();
        }

		LoadAllPlaythrough();
    }

    public void Show()
    { 
        gameObject.SetActive(true);
    }

	public void Hide()
	{
		gameObject.SetActive(false);
	}


	public void AddPlaythrough()
    {
        if (PlaythroughName.text != "")
        {
            CurrentPlaythroughId++;
            Playthrough walkthrough = AddPlaythrough(CurrentPlaythroughId, PlaythroughName.text);
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            di.CreateSubdirectory(walkthrough.FullName);
        }
    }

    public Playthrough AddPlaythrough(int argId, string argName)
    {
        GameObject gameObject = Instantiate(PlaythroughPrefab, PlaythroughList.transform);
        Playthrough playthrough = gameObject.GetComponent<Playthrough>();

        playthrough.Id = argId;
        playthrough.IdTxt.text = argId.ToString().PadLeft(3, '0')+".";
        playthrough.Name.text = argName;
        playthrough.SaveCatalog = this;
        playthrough.Select(false);
        AllPlaythrough.Add(playthrough.Id, playthrough);
        return playthrough;
    }

    public SaveCheckPoint AddCheckPoint(string argFileName, string argDay, string argDescription, string argDate, string argVersion, bool argSupport, int argPlaythroughId)
    {
        GameObject gameObject = Instantiate(CheckPointPrefab, CheckPointList.transform);
        SaveCheckPoint checkPoint = gameObject.GetComponent<SaveCheckPoint>();

        checkPoint.Set(argFileName, argDay, argDescription, argDate, argVersion, argSupport);

        checkPoint.PlaythroughId = argPlaythroughId;
        checkPoint.SaveCatalog = this;

        if (AllCheckPoint.ContainsKey(argDay) == false)
        {
            AllCheckPoint.Add(argDay, checkPoint);
        }

        return checkPoint;
    }


    public void Save()
    {
        Save(CheckPointName.text);
    }

    public void Save(string argCheckPointName)
    {
        if (argCheckPointName != "")
        {
            string saveInfo = Day + "\n";
            saveInfo += argCheckPointName + "\n";

            if (SelectedPlaythroughId == 0)
            {
                // "Select your playthrough", "All saves and autosaves will be grouped by the name of your playthrough."
                DialogYouSure.Show("¬ыберете ваше прохождение", "¬се сохранени€ и автосохранени€ группируютс€ по названию вашего прохождени€.", "Ok");
            }
            else
            {
                string fileName = Day.Replace(" : ", "_").Replace(" ", "_");
                string dirName = AllPlaythrough[SelectedPlaythroughId].FullName;

                File.WriteAllText(Application.persistentDataPath + "\\" + dirName + "\\" + fileName + ".txt", saveInfo);

                SaveManager.Save(dirName, fileName);
				LoadAllCheckPoint(SelectedPlaythroughId);
				//SaveGameUI.Close();
			}
		}
    }


    public string CurrentLoadDirName;
    public string CurrentLoadFileName;

    public void Load()
    {
        if (CurrentLoadDirName != "" && CurrentLoadFileName != "")
        {
            Load(CurrentLoadDirName, CurrentLoadFileName);
        }
    }

    public void Load(string argDirName, string argFileName)
    {
        if (SaveManager != null)
        {
            /*SaveGameUI.Close();
            if (MS.IsPause)
            {
                MS.Pause(); // сн€тие паузы перед загрузкой
            }*/

            SaveManager.LoadError += SaveManager_LoadError;
            SaveManager.Load(argDirName, argFileName);
        }
        else
        {
            //MS.LoadSaveGameInFreeMode();
            CurrentLoadDirName = argDirName;
            CurrentLoadFileName = argFileName;
        }
    }

    private void SaveManager_LoadError()
    {
        DialogYouSure.Show("Load Error", "¬о врем€ загрузки что-то пошло не так ..", LoadErrorExit, "Ok");
    }

    private void LoadErrorExit(int argAnswer)
    {
        /*if (MS != null)
        {
            MS.ExitMainMenu();
        }*/
    }

    public void DeselectAllPlaythrough()
    {
        foreach (var walkthrough in AllPlaythrough) 
        {
            walkthrough.Value.Deselect();
        }
    }

    public void DeselectAllCheckPoint()
    {
        foreach (var checkPoint in AllCheckPoint)
        {
            checkPoint.Value.Deselect();
        }
    }


    public void Open()
    {
        /*if (EManager != null && EManager.DayNightController != null)
        {
            CheckPointName.text = Day;

            SaveButton.SetActive(true);
            SaveTitle.SetActive(true);
            CheckPointName.gameObject.SetActive(true);
        }
        else
        {
            SaveButton.SetActive(false);
            SaveTitle.SetActive(false);
            CheckPointName.gameObject.SetActive(false);
        }*/

        LoadAllPlaythrough();
    }

    public void LoadAllPlaythrough()
    {
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] all = di.GetDirectories("???.*").OrderByDescending(fi => fi.LastWriteTime).ToArray();

        //FileInfo[] all = di.GetFiles("*.bin").OrderByDescending(fi => fi.LastWriteTime).ToArray();

        AllPlaythrough.Clear();
        Playthrough[] oldSlot = PlaythroughList.GetComponentsInChildren<Playthrough>();
        for (int i = 0; i < oldSlot.Length; i++)
        {
            Destroy(oldSlot[i].gameObject);
        }

        if (all.Length > 0)
        {
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].Name.Length > 4 && all[i].Name.Substring(3, 1) == ".")
                {
                    int id = 0;
                    if (int.TryParse(all[i].Name.Substring(0, 3), out id))
                    {
                        string name = all[i].Name.Substring(4);

                        AddPlaythrough(id, name);

                        if (id > CurrentPlaythroughId)
                        {
                            CurrentPlaythroughId = id;
                        }
                    }
                }
            }
            AllPlaythrough.First().Value.Select();
        }
    }

    public void LoadAllCheckPoint(int argPlaythroughId)
    {
        AllCheckPoint.Clear();
        SaveCheckPoint[] oldSlot = CheckPointList.GetComponentsInChildren<SaveCheckPoint>();
        for (int i = 0; i < oldSlot.Length; i++)
        {
            Destroy(oldSlot[i].gameObject);
        }

        if (argPlaythroughId != 0)
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] all = di.GetFiles(AllPlaythrough[argPlaythroughId].FullName + "\\*.bin").OrderByDescending(fi => fi.LastWriteTime).ToArray();

            for (int i = 0; i < all.Length; i++)
            {
                string fileName = all[i].Name;
                string version = "?";
                bool support = false;

                int index = fileName.IndexOf("_v");
                if (index != -1)
                {
                    version = fileName.Substring(index + 1).Replace(".bin", "");
                    fileName = fileName.Substring(0, index);
                }
                if (Version == version)
                {
                    support = true;
                }

                string dateTime = DT(all[i].LastWriteTime);

                string[] info = File.ReadAllLines(Application.persistentDataPath + "\\" + AllPlaythrough[argPlaythroughId].FullName + "\\" + fileName + ".txt");

                AddCheckPoint(fileName, info[0], info[1], dateTime, version, support, argPlaythroughId);
            }
        }
    }


    private string DT(DateTime dateTime)
    {
        string ret = dateTime.Day.ToString("D2") +
                "." + dateTime.Month.ToString("D2") +
                "." + dateTime.Year.ToString() +
                "\t" + dateTime.Hour.ToString("D2") +
                ":" + dateTime.Minute.ToString("D2") +
                ":" + dateTime.Second.ToString("D2");
        return ret;
    }
}







