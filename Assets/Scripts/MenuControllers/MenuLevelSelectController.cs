using UnityEngine;
using UnityEngine.UI;

public class MenuLevelSelectController : MonoBehaviour
{

    public GameObject loadingUI;
    public ScrollRect scrollRect;

    void Start(){

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
