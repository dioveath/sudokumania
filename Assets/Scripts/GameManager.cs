using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;

public class GameManager : MonoBehaviour
{
    // UI objects
    [Header("Menu Parent Holder Objects")]
    public GameObject loginHolder;
    public GameObject mainMenuHolder;
    public GameObject leaderboardHolder;
    public GameObject settingsHolder;
    public GameObject levelSelectHolder;
    public GameObject youwinHolder;
    public GameObject gameplayHolder;
    public GameObject infoHolder;
    public GameObject sudokuLinesHolder;
    public GameObject fbLinkedHolder;

    // LevelSelect Menu
    [Header("LevelSelect Objects")]
    public GameObject levelButtonHolder;
    public GameObject levelButton;
    public GameObject loadingUI;

    // MainMenu
    [Header("MainMenu Objects")]
    public Text welcomeText;

    private GameObject _lastMenu;
    private static GameManager _instance;
    private GameState _currentState;
    

    void Awake(){
        _instance = this;
    }

    public static GameManager Instance() {
	if(_instance == null) {
            Debug.LogWarning("Error: No GameManager instantiated as of now!");	    
	}
	return _instance;
    }
    
    void Start() {
        FirebaseManager.Instance().OnFirebaseInitialized.AddListener(OnFirebaseInitialize);

        SwitchState(GameState.MAINMENU);
        AudioManager.Instance().PlayMusic("game_menu");
    }


    public void SwitchState(GameState state){
        Debug.Log(_currentState + " > " + state);

        // Exit the previous state
        switch(_currentState){
	    case GameState.LOGIN:
                loginHolder.SetActive(false);
                break;
            case GameState.MAINMENU:
                mainMenuHolder.SetActive(false);
                // sudokuLinesHolder.SetActive(false);
                break;
	    case GameState.LEVEL_SELECT:
                levelSelectHolder.SetActive(false);
                break;
	    case GameState.LEADERBOARD:
                leaderboardHolder.SetActive(false);
                break;
            case GameState.GAMEPLAY:
                sudokuLinesHolder.SetActive(false);
                SudokuManager.Instance().Finish();
                gameplayHolder.SetActive(false);
                break;
            case GameState.YOUWIN:
                youwinHolder.SetActive(false);
                break;
	    case GameState.SETTINGS:
                settingsHolder.GetComponent<MenuSettingsController>().Cleanup();
                settingsHolder.SetActive(false);
		break;
	    case GameState.INFO:
                infoHolder.SetActive(false);
                break;

        }

	// Enter the given state
	switch(state){
	    case GameState.LOGIN:
                loginHolder.SetActive(true);
                break;
            case GameState.MAINMENU:
		if(AuthManager.Instance().isSignedIn) {
                    welcomeText.text = "Welcome, Mr. " + AuthManager.Instance().username;
                    fbLinkedHolder.SetActive(true);
                }
                else {
                    welcomeText.text = "Welcome, fellow Traveller!";
                    fbLinkedHolder.SetActive(false);		    
		}

                // sudokuLinesHolder.SetActive(true);
                mainMenuHolder.GetComponent<MenuMainController>().SetSudokuPoints();
                mainMenuHolder.SetActive(true);
                break;
	    case GameState.LEVEL_SELECT:
                HandleInitializeLevels();
                levelSelectHolder.SetActive(true);
                break;
	    case GameState.LEADERBOARD:
                leaderboardHolder.SetActive(true);
                leaderboardHolder.GetComponent<MenuLeaderboardController>().LoadLeaderboardEntry();
                break;
	    case GameState.SETTINGS:
                settingsHolder.GetComponent<MenuSettingsController>().Init(SaveManager.Instance.settingsData);
                settingsHolder.SetActive(true);
		break;
            case GameState.GAMEPLAY:
                sudokuLinesHolder.SetActive(true);
                gameplayHolder.SetActive(true);		
                break;
	    case GameState.YOUWIN:
                youwinHolder.SetActive(true);
                youwinHolder.GetComponent<MenuYouwinController>().SetCompletedStats();
                break;
	    case GameState.INFO:
                infoHolder.SetActive(true);
                break;
        }

        _currentState = state;
    }

    void Update(){
	if(Input.GetKeyDown(KeyCode.Escape)){
	    switch(_currentState){
		case GameState.MAINMENU:
		    Application.Quit();
		    break;
		case GameState.LEVEL_SELECT:
                    AudioManager.Instance().PlayAudio("click_heavy");
                    SwitchState(GameState.MAINMENU);
		    break;
		case GameState.LEADERBOARD:
                    AudioManager.Instance().PlayAudio("click_heavy");		    
		    SwitchState(GameState.MAINMENU);
		    break;
		case GameState.SETTINGS:
                    AudioManager.Instance().PlayAudio("click_heavy");
		    SwitchState(GameState.MAINMENU);		    
                    break;
		case GameState.INFO:
                    AudioManager.Instance().PlayAudio("click_heavy");		    
                    SwitchState(GameState.MAINMENU);
                    break;
            }
	}
    }

    // this should be moved to MenuLevelSelectcontroller
    async void HandleInitializeLevels(){
        loadingUI.SetActive(true);

        Button[] currentButtons = levelButtonHolder.GetComponentsInChildren<Button>();
        int currentTotalButtons = currentButtons.Length;	
        for (int i = 0; i < currentTotalButtons; i++)
	    currentButtons[i].gameObject.SetActive(false);
	
	if(FirebaseManager.Instance().isInitialized) {
	    await SudokuUtils.GetAllLevels(FirebaseDatabase.DefaultInstance);
	} else {
            await SudokuUtils.GetAllLevels(null);
        }
        if(SudokuUtils.allSudokuLevels == null){
            Debug.LogWarning("NULL SudokuUtils.allSudokuLevels");
        }

        int levelsCount = SudokuUtils.allSudokuLevels.Count;
        loadingUI.SetActive(false);

        for (int i = 0; i < levelsCount; i++){
            GameObject buttonObj;
            if(i >= currentTotalButtons) {
		buttonObj = Instantiate(levelButton, Vector3.zero, Quaternion.identity);
	    } else {
                buttonObj = currentButtons[i].gameObject;
            }

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.SetParent(levelButtonHolder.transform);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
	    rect.anchoredPosition3D = new Vector3(0, -1400 -(i * 150), 0);
	    rect.localScale = Vector3.one;

            int levelIndex = i;

            buttonObj.GetComponentInChildren<Text>().text = "Level " + (levelIndex+1);
	    if(Player.Instance.playerData.lastPlayedId == SudokuUtils.allSudokuLevels[levelIndex].id)
                buttonObj.GetComponent<Image>().color = Color.green;
	    else
                buttonObj.GetComponent<Image>().color = Color.white;		

            buttonObj.GetComponent<Button>().onClick.AddListener(() => {
                SwitchState(GameState.GAMEPLAY);
		SudokuManager.Instance().Init(SudokuUtils.allSudokuLevels[levelIndex]);
                AudioManager.Instance().PlayAudio("click_wooden1");
            });
        }

	if(currentTotalButtons > levelsCount){
	    Debug.Log("Destroying..!");
            for (int i = levelsCount; i < currentTotalButtons; i++){
                Debug.Log(i);
                currentButtons[i].gameObject.SetActive(false);
                DestroyImmediate(currentButtons[i]);
            }
	}

	for (int i = 0; i < currentButtons.Length; i++)
	    currentButtons[i].gameObject.SetActive(true);		
    }

    public void OnFirebaseInitialize(){
        // AuthManager.Instance().authStateChangedUEvent.AddListener(OnAuthStateChange);	
    }
    
    public void OnAuthStateChange(FirebaseUser user){
	// NOTE: We're not using any authentication system for initial release
        // if(user != null){
        //     SwitchState(GameState.MAINMENU);
	//     Debug.Log("Game logged in with: " + user.DisplayName);
        // } else {
        //     SwitchState(GameState.LOGIN);
        // }	
    }

    
}




[Serializable]
public enum GameState
{
    LOGIN,
    MAINMENU,
    LEADERBOARD,
    SETTINGS,
    LEVEL_SELECT,
    INFO,
    YOUWIN,
    GAMEPLAY
};
