using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BuildManager))]
[CanEditMultipleObjects]
public class BuildingManagerEditor : Editor
{
	private static BuildManager[] CurrentManager;

	public static BuildManager conv(object argObject)
	{
		return argObject as BuildManager;
	}
	public virtual void OnEnable()
	{
		CurrentManager = Array.ConvertAll(targets, new Converter<object, BuildManager>(conv));
	}

	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();

		if (GUILayout.Button("Create"))
		{
			for (int i = 0; i < CurrentManager.Length; i++)
			{
				CurrentManager[i].Create();
				UnityEditor.EditorUtility.SetDirty(CurrentManager[i]);
			}
		}
	}

}



