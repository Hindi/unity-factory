using GOF;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;
using System.Collections.Generic;

public class GOFConfigCreatorEditor : EditorWindow
{
    private List<GOFactoryMachineTemplate> templates = new List<GOFactoryMachineTemplate>();

    string name = "";
    GameObject prefab;

    AnimBool fold;
    
    [MenuItem("Window/GameObjectFactory")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GOFConfigCreatorEditor));
    }
    
    void OnGUI()
    {
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            name = EditorGUILayout.TextField("Prefab name", name, GUILayout.Width(300));
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), GUILayout.Width(300));
        }

        if(GUILayout.Button("Create Config"))
        {
            GOFactoryMachineTemplate machine = new GOFactoryMachineTemplate();
            machine.modelName = name;
            templates.Add(machine);
        }

        foreach (var t in templates)
        {
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    templates.Remove(t);
                    return;
                }
                EditorGUILayout.LabelField(t.modelName, GUILayout.Width(EditorGUIUtility.currentViewWidth));
            }
        }
    }
}