using UnityEngine;
using UnityEngine.UI;

public class MenuLevelSelectController : MonoBehaviour
{

    public GameObject loadingUI;
    public ScrollRect scrollRect;
    public Text seasonTextUI;

    public DialogSequence sequence;

    void Start(){
    }

    public void Init(){
        seasonTextUI.text = "Season: " + SudokuUtils.season;

	if(!SaveManager.Instance.settingsData.isSeasonShowed) {
	    DialogManager.Instance.StartDialogSequence(sequence);
            SaveManager.Instance.settingsData.isSeasonShowed = true;
            SaveManager.Instance.SaveGameSettings();
        }

    }

    public void Cleanup(){
    }    

    public void OnPlayButtonPressed(){
        GameManager.Instance().SwitchState(GameState.GAMEPLAY);
        AudioManager.Instance().PlayAudio("click_basic");
    }

    public void OnBackButtonPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayAudio("click_heavy");
    }

}
