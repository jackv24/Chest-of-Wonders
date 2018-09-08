using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    SaveManager manager;

    public override void OnInspectorGUI()
    {
        //Draw normal GUI, with extra stuff added to end
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        //Get selected SaveManager in inspector
        manager = (SaveManager)target;

        //If button is pressed, clear save data
        if(GUILayout.Button("Clear Save Data in slot " + manager.SaveSlot))
            manager.ClearSave(false);

        if (GUILayout.Button("Clear all Save Data"))
            manager.ClearSave(true);
    }
}
