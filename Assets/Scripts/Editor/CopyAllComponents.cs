using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;
public class CopyAllComponents : EditorWindow
{
    [MenuItem("Tool/CopyAllComponents")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CopyAllComponents));
    }

    GameObject Source = null;
    GameObject Dest = null;
    void OnGUI()
    {

        Source =EditorGUILayout.ObjectField("原始物件", Source, typeof(GameObject),true) as GameObject;
        Dest = EditorGUILayout.ObjectField("目標物件", Dest, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("CopyAll!"))
        {
            var components = Source.GetComponents<Component>();

            foreach (var comp in components)
            {
                if (comp.GetType() == typeof(Transform) || comp.GetType() == typeof(MeshFilter) ||
                 comp.GetType() == typeof(MeshRenderer))
                    continue;

                if (IsHaveComponent(Dest, comp.GetType()))
                    continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(comp);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(Dest);
            }
        }
    }


        static bool IsHaveComponent(GameObject obj, Type MyType)
    {

        var allComp = obj.GetComponents<Component>();

        foreach (var comp in allComp)
        {
            if (comp.GetType() == MyType)
                return true;
        }

        return false;
    }
}