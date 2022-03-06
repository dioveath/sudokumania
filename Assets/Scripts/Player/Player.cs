using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData playerData;

    private static Player _instance;
    public static Player Instance {
	get {
	    if(_instance == null) {
                Debug.LogError("No instantiation Player Script");
            }
            return _instance;
        }
    }

    void Awake(){
	if(_instance != null){
            DestroyImmediate(this.gameObject);
        }
        _instance = this;
	LoadSavedPlayerData();
        Debug.Log("Player data loaded!");
    }


    public void LoadSavedPlayerData(){
        playerData = SaveManager.Instance.LoadPlayerData();
	if(playerData == null)
            playerData = new PlayerData();
    }

    public void SaveCurrentPlayerData(){
	SaveManager.Instance.SavePlayerData(playerData);
	if(playerData.isLinked) {
            AuthManager.Instance().SavePlayerToDB(playerData);
        }
    }

    public void AddPlayingSudokuLevel(SudokuLevel level){
        bool replaced = false;
        for (int i = 0; i < playerData.playingLevels.Count; i++){
	    if(playerData.playingLevels[i].id == level.id) {
                playerData.playingLevels[i] = level;
                replaced = true;
		Debug.Log("Replaced level to playingLevels");		
                break;
            }
	}
	if(!replaced) {
            Debug.Log("Added level to playingLevels");
            playerData.playingLevels.Add(level);
        }
    }

    public SudokuLevel GetPlayingSudokuLevel(string id){
        for (int i = 0; i < playerData.playingLevels.Count; i++){
	    if(id == playerData.playingLevels[i].id) return playerData.playingLevels[i];
        }
        Debug.Log("Not found sudoku level with id: " + id);
        return null;
    }

}
