using UnityEngine;
using UnityEngine.UI;

public class SudokuLevelUIManager : MonoBehaviour
{

    public Text timeElapsedText;
    public GameObject pauseUIHolder;

    void Update(){
        int minutes = (int) (SudokuManager.Instance().GetTimeElapsed() / 60);
	int seconds = (int) (SudokuManager.Instance().GetTimeElapsed() % 60);
        timeElapsedText.text = $"{minutes.ToString("00")} : {seconds.ToString("00")}";
    } 

}
