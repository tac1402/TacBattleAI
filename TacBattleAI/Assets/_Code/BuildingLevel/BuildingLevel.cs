using System.Collections;
using System.Collections.Generic;
using TAC.UI;
using UnityEngine;

public class BuildingLevel : MonoBehaviour
{
	public int Level;

	public List<GameobjectList> List;

	public void SetLevel(int argLevel)
	{ 
		Level = argLevel;
		SetLevel();
	}

	public void SetLevel()
	{ 
		for (int i = 0; i < List[Level - 1].Disabled.Count; i++) 
		{
			List[Level - 1].Disabled[i].SetActive(false);
		}
		for (int i = 0; i < List[Level - 1].Enabled.Count; i++)
		{
			List[Level - 1].Enabled[i].SetActive(true);
		}

		for (int i = 0; i < List[Level - 1].ShowWireframe.Count; i++)
		{
			List[Level - 1].ShowWireframe[i].SetActive(true);

			Wireframe[] wireframes = List[Level - 1].ShowWireframe[i].GetComponentsInChildren<Wireframe>();
			for (int j = 0; j < wireframes.Length; j++) 
			{
				wireframes[j].Show(Wireframe.WireframeMode.Gray);
			}
		}
		for (int i = 0; i < List[Level - 1].HideWireframe.Count; i++)
		{
			List[Level - 1].HideWireframe[i].SetActive(true);

			Wireframe[] wireframes = List[Level - 1].HideWireframe[i].GetComponentsInChildren<Wireframe>();
			for (int j = 0; j < wireframes.Length; j++)
			{
				wireframes[j].Hide();
			}
		}

	}
}



