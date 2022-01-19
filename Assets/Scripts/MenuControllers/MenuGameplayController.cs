using UnityEngine;

public class MenuGameplayController : MonoBehaviour
{
    public void OnMenuPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayAudio("click_basic");
    }

    public void OnLeaderboardPressed(){
        AudioManager.Instance().PlayAudio("click_basic");
    }
    public void OnSettingsPressed(){
        AudioManager.Instance().PlayAudio("click_basic");  
    }
}
