using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevelSelectController : MonoBehaviour
{

    public GameObject loadingUI;

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
