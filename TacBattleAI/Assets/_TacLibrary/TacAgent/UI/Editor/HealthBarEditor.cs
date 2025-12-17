using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthBar))]
[CanEditMultipleObjects]
public class HealthBarEditor : Editor
{
	private static HealthBar[] CurrentHealthBar;

	public static HealthBar conv(object argObject)
	{
		return argObject as HealthBar;
	}
	public virtual void OnEnable()
	{
		CurrentHealthBar = Array.ConvertAll(targets, new Converter<object, HealthBar>(conv));
	}

	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();

		if (GUILayout.Button("Damage"))
		{
			for (int i = 0; i < CurrentHealthBar.Length; i++)
			{
				CurrentHealthBar[i].ChangeHealth(new MaxValue(CurrentHealthBar[i].CurrentHealth.Current - 10, CurrentHealthBar[i].CurrentHealth.Current, 100));
				UnityEditor.EditorUtility.SetDirty(CurrentHealthBar[i]);
			}
		}
	}

}

