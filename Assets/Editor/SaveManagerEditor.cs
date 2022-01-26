using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(SaveManager))]
public class SaveManagerEditor : Editor {

    public override void OnInspectorGUI(){
        SaveManager saveManager = target as SaveManager;

	if(GUILayout.Button("Delete all Persistent Path Data")) {
            saveManager.DeleteAll();
        }
    }

}
