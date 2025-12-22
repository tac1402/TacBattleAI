using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BuildingChange))]
[CanEditMultipleObjects]
public class BuildingPackEditor : Editor
{
    private static BuildingChange[] CurrentBuilding;

    public static BuildingChange conv(object argObject)
    {
        return argObject as BuildingChange;
    }
    public virtual void OnEnable()
    {
        CurrentBuilding = Array.ConvertAll(targets, new Converter<object, BuildingChange>(conv));
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        if (GUILayout.Button("ChangeModel"))
        {
            for (int i = 0; i < CurrentBuilding.Length; i++)
            {
                //CurrentBuilding[i].ChangeModel();
                UnityEditor.EditorUtility.SetDirty(CurrentBuilding[i]);
            }
        }
    }

}


