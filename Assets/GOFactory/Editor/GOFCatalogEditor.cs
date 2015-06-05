using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(GOFCatalog))]
public class GOFCatalogEditor : Editor 
{
    GOFCatalog myTarget;
    List<GOFactoryMachineTemplate> list;
    SerializedProperty listProperty;
    SerializedObject GetTarget;
    int ListSize;

    void OnEnable()
    {
        myTarget = (GOFCatalog)target;
        list = myTarget.MachinesList;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        if(GUILayout.Button("Add a machine"))
        {
            list.Add(new GOFactoryMachineTemplate());
        }

        GetTarget = new SerializedObject(myTarget);
        listProperty = GetTarget.FindProperty("MachinesList");
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty ListRef = listProperty.GetArrayElementAtIndex(i);
            SerializedProperty Name = ListRef.FindPropertyRelative("MachineName");
            SerializedProperty InstantiateCount = ListRef.FindPropertyRelative("PreInstantiateCount");
            SerializedProperty Prefab = ListRef.FindPropertyRelative("Prefab");
            SerializedProperty LifeSpan = ListRef.FindPropertyRelative("LifeSpan");
            SerializedProperty InactiveLifeSpan = ListRef.FindPropertyRelative("InactiveLifeSpan");
            SerializedProperty DefaultPosition = ListRef.FindPropertyRelative("DefaultPosition");

            string title = (Name.stringValue == "" ? "New machine" : Name.stringValue);
            list[i].folded = EditorGUILayout.Foldout(list[i].folded, i + ". " + title);
            if (list[i].folded)
            {
                if (GUILayout.Button("Delete this machine"))
                {
                    list.RemoveAt(i);
                    return;
                }
                EditorGUILayout.PropertyField(Name);
                EditorGUILayout.PropertyField(Prefab);
                EditorGUILayout.PropertyField(InstantiateCount);
                EditorGUILayout.PropertyField(LifeSpan);
                EditorGUILayout.PropertyField(InactiveLifeSpan);
                EditorGUILayout.PropertyField(DefaultPosition);
                EditorGUILayout.Space();
            }
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }
}

/*
        if(GUILayout.Button("+"))
        {
            ListSize++;
        }
        if (GUILayout.Button("-"))
        {
            ListSize--;
        }

        if (ListSize != listProperty.arraySize)
        {
            while (ListSize > listProperty.arraySize)
            {
                listProperty.InsertArrayElementAtIndex(listProperty.arraySize);
            }
            while (ListSize < listProperty.arraySize)
            {
                listProperty.DeleteArrayElementAtIndex(listProperty.arraySize - 1);
            }
        }*/