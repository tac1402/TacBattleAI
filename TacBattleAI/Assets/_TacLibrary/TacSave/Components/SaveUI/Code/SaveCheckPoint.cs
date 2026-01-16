using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Tac.DConvert;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Сохраненная контрольная точка в прохождении
/// </summary>
public class SaveCheckPoint : MonoBehaviour
{
    public SaveCatalog SaveCatalog;

    public Text Day;
    public Text Info;
    public Text Description;

    public GameObject Icon;
    public GameObject IconSelect;

    public string FullName
    {
        get 
        {
            return FileName + "_" + SaveVersion;
        }
    }

    public int PlaythroughId;
    public string FileName;
    public string SaveVersion;

    public void Set(string argFileName, string argDay, string argDescription, string argDate, string argVersion, bool argVersionSupport)
    {
        FileName = argFileName;
        Day.text = argDay;
        Description.text = argDescription;

        Info.text = "<color=yellow>" + argDate + "</color>";
        if (argVersionSupport)
        {
            Info.text += "\t<color=green>" + argVersion + "</color>";
        }
        else
        {
            Info.text += "\t<color=red>" + argVersion + "</color>";
        }
        SaveVersion = argVersion;
    }

    public void Select()
    {
        
        bool selectMode = Icon.activeSelf;

        SaveCatalog.DeselectAllCheckPoint();

        //SaveCatalog.Day = Day.text;
        SaveCatalog.LoadName.text = Description.text;

        if (selectMode == true)
        {
            Icon.SetActive(false);
            IconSelect.SetActive(true);

            string playthroughName = SaveCatalog.AllPlaythrough[PlaythroughId].FullName;
            SaveCatalog.CurrentLoadDirName = playthroughName;
            SaveCatalog.CurrentLoadFileName = FullName;
        }
        else
        {
            SaveCatalog.CurrentLoadDirName = "";
            SaveCatalog.CurrentLoadFileName = "";
            SaveCatalog.LoadName.text = "";
        }
    }
    public void Deselect()
    {
        IconSelect.SetActive(false);
        Icon.SetActive(true);
    }

    public void Load()
    {
        string playthroughName = SaveCatalog.AllPlaythrough[PlaythroughId].FullName;
        SaveCatalog.Load(playthroughName, FullName);
    }

    public void Delete()
    {
        // "Are you sure?", "This save will be deleted."
        SaveCatalog.DialogYouSure.Show("Вы уверены?", "Это сохранение будет удалено.", YouSureDelete, "Да", "Нет");
    }

    public void YouSureDelete(int argAnswer)
    {
        if (argAnswer == 0)
        {
            string playthroughName = SaveCatalog.AllPlaythrough[PlaythroughId].FullName;

            string[] allSave = Directory.GetFiles(SaveCatalog.ISaveManager.SaveRootDir + "\\" + playthroughName, FileName + "*.*");
            for (int i = 0; i < allSave.Length; i++)
            {
                File.Delete(allSave[i]);
            }
            SaveCatalog.LoadAllCheckPoint(PlaythroughId);
        }
    }

}


