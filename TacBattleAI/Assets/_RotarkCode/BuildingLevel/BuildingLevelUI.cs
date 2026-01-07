using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;


public class BuildingLevelUI : MonoBehaviour
{
    public int CurrentLevel = 3;
	public int MinLevel = 1;
	public int MaxLevel = 3;
    public TMP_Text LevelText;
	public List<BuildingLevel> Level = new List<BuildingLevel>();



    void Start()
    {
        Level = GetComponentsInChildren<BuildingLevel>(true).ToList();
    }

    public void AddLevel()
    {
        CurrentLevel++;
        if (CurrentLevel > MaxLevel)
        {
            CurrentLevel = MinLevel;
        }
        RefreshLevel();
    }

	public void SubLevel()
	{
		CurrentLevel--;
		if (CurrentLevel < MinLevel)
		{
			CurrentLevel = MaxLevel;
		}
		RefreshLevel();
	}

	private void RefreshLevel()
    {
		LevelText.text = CurrentLevel.ToString();
		for (int i = 0; i < Level.Count; i++) 
        {
            Level[i].SetLevel(CurrentLevel);
        }
    }

}
