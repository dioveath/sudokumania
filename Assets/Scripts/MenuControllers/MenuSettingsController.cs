using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsController : MonoBehaviour
{

    public Image soundIcon;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    public void OnSoundButtonPressed(){
	AudioManager.Instance().ToggleSound();
        SaveManager.Instance.settingsData.sound = AudioManager.Instance().IsOn();
        SaveManager.Instance.SaveGameSettings();
        UpdateButtonSprite(SaveManager.Instance.settingsData.sound);
        AudioManager.Instance().PlayAudio("click_basic");
        AudioManager.Instance().PlayMusic("game_menu");
    }

    public void OnBackButtonPressed(){
        AudioManager.Instance().PlayAudio("click_heavy");
        GameManager.Instance().SwitchState(GameState.MAINMENU);
    } 

    public void UpdateButtonSprite(bool sound){
	if(AudioManager.Instance().IsOn())
 	    soundIcon.sprite = soundOnIcon;
	else
	    soundIcon.sprite = soundOffIcon;
    }

}
