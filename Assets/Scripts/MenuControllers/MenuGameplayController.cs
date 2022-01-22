using UnityEngine;

public class MenuGameplayController : MonoBehaviour
{
    public GameObject pauseMenuHolder;

    public void OnMenuPressed(){
	if(Player.Instance.saveExists) {
	    Player.Instance.playerData.playingLevel = SudokuManager.Instance().GetCurrentSudokuLevel();
	} else {
            Player.Instance.playerData = new PlayerData(SudokuManager.Instance().GetCurrentSudokuLevel());
        }
        Player.Instance.SaveCurrentPlayerData();

        pauseMenuHolder.SetActive(true);
        SudokuManager.Instance().PauseSudokuGame();
        AudioManager.Instance().PlayAudio("click_basic");
    }

    public void OnLeaderboardPressed(){
        AudioManager.Instance().PlayAudio("click_basic");
    }
    public void OnSettingsPressed(){
        AudioManager.Instance().PlayAudio("click_basic");  
    }

    public void OnResumePressed(){
        pauseMenuHolder.SetActive(false);
        SudokuManager.Instance().ResumeSudokuGame();
        AudioManager.Instance().PlayAudio("click_basic");
    }

    public void OnHomePressed(){
        pauseMenuHolder.SetActive(false);
        GameManager.Instance().SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayAudio("click_basic");
    }

}
