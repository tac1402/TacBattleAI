using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Building2))]
[CanEditMultipleObjects]
public class BuildingPackEditor : Editor
{
    private static Building2[] CurrentBuilding;

    public static Building2 conv(object argObject)
    {
        return argObject as Building2;
    }
    public virtual void OnEnable()
    {
        CurrentBuilding = Array.ConvertAll(targets, new Converter<object, Building2>(conv));
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        if (GUILayout.Button("ChangeModel"))
        {
            for (int i = 0; i < CurrentBuilding.Length; i++)
            {
                CurrentBuilding[i].ChangeModel();
                UnityEditor.EditorUtility.SetDirty(CurrentBuilding[i]);
            }
        }
    }

}


