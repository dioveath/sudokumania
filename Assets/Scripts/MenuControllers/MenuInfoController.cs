using UnityEngine;

public class MenuInfoController : MonoBehaviour
{
    public void OnBackPressed(){
        GameManager.Instance().SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayAudio("click_heavy");
    }
}
