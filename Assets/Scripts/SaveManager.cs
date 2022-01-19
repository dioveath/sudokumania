using UnityEngine;

public class SaveManager : MonoBehaviour {

    private PlayerData _lastPlayerData;
    public SettingsData settingsData; // stores the settingsData

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

    void Awake(){
	if(_instance != null) {
            DestroyImmediate(this.gameObject);	    
	}
        _instance = this;
        LoadSettings();
    }

    public void SavePlayerData(PlayerData pd){
        PlayerPrefs.SetString(pd.id, JsonUtility.ToJson(pd));
        FirebaseDatabaseManager.Instance.SavePlayerToDB(pd);
    }

    public void SaveGameSettings(){
        PlayerPrefs.SetInt("sound", settingsData.sound ? 1 : 0);
    }

    public SettingsData LoadSettings(){
        Debug.Log("Loading settings...!");
        SettingsData sd = new SettingsData(PlayerPrefs.GetInt("sound", 0) == 0 ? false : true);
        settingsData = sd;
        return sd;
    }

}


[System.Serializable]
public class SettingsData {
    public bool sound;

    public SettingsData(bool sound){
	this.sound = sound;
    }
}
