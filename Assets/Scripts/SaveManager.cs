using UnityEngine;

public class SaveManager : MonoBehaviour {

    private PlayerData _lastPlayerData;

    private static SaveManager _instance;
    public static SaveManager Instance {
	get {
	    if(_instance == null) {
		Debug.LogError("No Instantiation SaveManager Script");
		return null;		
	    }
            return _instance;
        }
    }

    void Start(){
	if(_instance != null) {
            DestroyImmediate(this.gameObject);	    
	}
        _instance = this;

	

    }

    public void SavePlayerData(PlayerData pd){
        PlayerPrefs.SetString(pd.id, JsonUtility.ToJson(pd));
        FirebaseDatabaseManager.Instance.SavePlayerToDB(pd);
    }

    
    

}
