using UnityEngine;

public class Player : MonoBehaviour
{

    public PlayerData playerData;
    public bool saveExists = false;

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

    void Start()
    {
    }

    public void LoadSavedPlayerData(){
        playerData = SaveManager.Instance.LoadPlayerData();
	if(playerData != null) saveExists = true;
    }

    public void SaveCurrentPlayerData(){
	SaveManager.Instance.SavePlayerData(playerData);
    }

}
