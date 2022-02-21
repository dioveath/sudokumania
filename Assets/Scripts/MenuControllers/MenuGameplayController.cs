using UnityEngine;

public class MenuGameplayController : MonoBehaviour
{
    public GameObject pauseMenuHolder;
    public GameObject tutorialObject;
    private bool _paused;

    [SerializeField]
    public DialogSequence sequence;
    public DialogSequence secondSequence;
    public DialogSequence thirdSequence;

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
	if(!Player.Instance.playerData.isTutorialComplete) {
            ShowTutorial();
	}
        SudokuManager.Instance().ResumeSudokuGame();
    }

    public void CleanUp(){
	SudokuManager.Instance().Finish();
    }

    public void ShowTutorial(){
        DialogManager.Instance.StartDialogSequence(sequence, () => {
            Player.Instance.playerData.isTutorialComplete = true;
            Player.Instance.SaveCurrentPlayerData();
            DialogManager.Instance.HideDialog();
        });
    }

    public void ShowTutorial2(){
        SudokuManager.Instance().SetInputActive(true);
	DialogManager.Instance.StartDialogSequence(secondSequence, () => {
	    DialogManager.Instance.HideDialog();
	    SudokuManager.Instance().SetInputActive(false);
	    TutorialManager.Instance.PromptInputBlock(SudokuManager.Instance().GetTutorialInputBlock(), () => {
		SudokuManager.Instance().SetInputActive(true);
		SudokuManager.Instance().OnInput(SudokuManager.Instance().GetTutorialInputBlock().GetInputValue());
		DialogManager.Instance.StartDialogSequence(thirdSequence, () =>
		{
		    DialogManager.Instance.HideDialog();
		});
	    });
	});
    }

}
