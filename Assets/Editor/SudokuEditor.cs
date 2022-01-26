using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(SudokuLayoutGenerator))]
public class SudokuEditor : Editor
{

    public override void OnInspectorGUI()
    {
        SudokuLayoutGenerator sudokuLayoutGenerator = target as SudokuLayoutGenerator;

	if(DrawDefaultInspector()) {
            sudokuLayoutGenerator.GenerateSudokuLayout();
        }

	if(GUILayout.Button("Generate SodukoLayout")) {
	    sudokuLayoutGenerator.GenerateSudokuLayout();
	}
    }

}
