using UnityEngine;

public class MenuGameplayController : MonoBehaviour
{
    public GameObject pauseMenuHolder;
    private bool _paused;

    void Update() {
	if(!(GameManager.Instance().GetCurrenetGameState() == GameState.GAMEPLAY)) return;
	if(Input.GetKeyDown(KeyCode.Escape)) {
	    if(!_paused) {
		OnMenuPressed();
                _paused = true;
            }
	    else {
                OnResumePressed();
                _paused = false;
            }
        }
    }

    public void OnMenuPressed(){
        SudokuLevel currentLevel = SudokuManager.Instance().GetCurrentSudokuLevel();
        currentLevel.lastElapsedTime = SudokuManager.Instance().GetTimeElapsed();
        Player.Instance.AddPlayingSudokuLevel(currentLevel);
        Player.Instance.playerData.lastPlayedId = currentLevel.id;
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

    public void Init(){
	if(Player.Instance.playerData.isFirstTime) {
            ShowTutorial();
        }
    }

    public void CleanUp(){
    }

    public void ShowTutorial(){
    }

}
