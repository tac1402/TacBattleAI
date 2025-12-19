using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Сохраненное прохождение
/// </summary>
public class Playthrough : MonoBehaviour
{
    public SaveCatalog SaveCatalog;

    public int Id;

    public Text IdTxt;
    public Text Name;

    public string FullName
    {
        get { return Id.ToString().PadLeft(3, '0') + "." + Name.text; }
    }

    public GameObject Icon;
    public GameObject IconSelect;

    public void Select() => Select(true);

    public void Select(bool argLoadCheckPoint)
    {
        bool selectMode = Icon.activeSelf;

        SaveCatalog.DeselectAllPlaythrough();

        SaveCatalog.CurrentLoadDirName = "";
        SaveCatalog.CurrentLoadFileName = "";
        SaveCatalog.LoadName.text = "";

        if (selectMode == true)
        {
            Icon.SetActive(false);
            IconSelect.SetActive(true);
            SaveCatalog.PlaythroughName.text = Name.text;
            SaveCatalog.SelectedPlaythroughId = Id;
            if (argLoadCheckPoint == true)
            {
                SaveCatalog.LoadAllCheckPoint(Id);
            }
        }
        else
        {
            SaveCatalog.PlaythroughName.text = "";
            SaveCatalog.SelectedPlaythroughId = 0;
            SaveCatalog.LoadAllCheckPoint(0);
        }
    }
    public void Deselect()
    {
        IconSelect.SetActive(false);
        Icon.SetActive(true);
    }
    public void Delete()
    {
        // "Are you sure?", "All saves associated with this playthrough will be deleted."
        SaveCatalog.DialogYouSure.Show("Вы уверены?", "Все сохранения ассоциированные с этим прохождением будут уадлены.", 
                        YouSureDelete, "Да", "Нет");
    }

    public void YouSureDelete(int argAnswer)
    {
        if (argAnswer == 0)
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath+"\\" + FullName);
            di.Delete(true);
            SaveCatalog.LoadAllPlaythrough();
        }
    }

}


