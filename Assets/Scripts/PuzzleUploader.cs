using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;

public class PuzzleUploader : MonoBehaviour
{

    public InputField puzzleField;
    public InputField pointsField;
    public Button uploadButton;
    public Text messageText;

    private FirebaseDatabase _dbRef;
    
    void Awake(){

    }

    void Start(){
        messageText.text = "Initializing...!";
        messageText.color = Color.yellow;
        uploadButton.interactable = false;
        uploadButton.onClick.AddListener(OnUploadPressed);			
	FirebaseManager.Instance().OnFirebaseInitialized.AddListener(OnFirebaseInitialize);
    }

    void OnUploadPressed() {
        string puzzle = puzzleField.text;
        int points = Int32.Parse(pointsField.text);
        PuzzleToUpload puzzleToUpload = new PuzzleToUpload(puzzle, points);
        SudokuUtils.WritePuzzlesToDatabase(_dbRef, puzzleToUpload);
        messageText.text = "Uploaded successfully.";
    }

    void OnFirebaseInitialize(){
        _dbRef = FirebaseDatabase.DefaultInstance;
	messageText.text = "Enter puzzles and press upload...!";
	messageText.color = Color.green;	
        uploadButton.interactable = true;
    }


}

[Serializable]
public struct PuzzleToUpload {
    public string puzzle;
    public int points;

    public PuzzleToUpload(string __puzzle, int __points){
        this.puzzle = __puzzle;
        this.points = __points;
    }
}
