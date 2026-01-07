using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingLevel))]
[CanEditMultipleObjects]
public class BuildingLevelEditor : Editor
{
	private static BuildingLevel[] CurrentBuildingLevel;

	public static BuildingLevel conv(object argObject)
	{
		return argObject as BuildingLevel;
	}
	public virtual void OnEnable()
	{
		CurrentBuildingLevel = Array.ConvertAll(targets, new Converter<object, BuildingLevel>(conv));
	}

	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();

		if (GUILayout.Button("SetLevel"))
		{
			for (int i = 0; i < CurrentBuildingLevel.Length; i++)
			{
				CurrentBuildingLevel[i].SetLevel();
				UnityEditor.EditorUtility.SetDirty(CurrentBuildingLevel[i]);
			}
		}
	}

}

