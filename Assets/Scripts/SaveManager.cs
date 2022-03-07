using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public SettingsData settingsData; // stores the settingsData
    private string _path;

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
	_path = Application.persistentDataPath + "/save.chc";    	
    }

    public void SavePlayerData(PlayerData pd){
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_path, FileMode.Create);
            formatter.Serialize(stream, pd);
            stream.Close();
        } catch(Exception e){
            Debug.LogError("Error: " + e.Message);
        }
    }

    public PlayerData LoadPlayerData(){
	try {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_path, FileMode.Open);
            PlayerData pd = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return pd;
        } catch (Exception e){
            Debug.LogError("Error: " + e.Message);
            return null;
        }
    }

    public void SaveGameSettings(){
        PlayerPrefs.SetInt("sound", settingsData.sound ? 1 : 0);
        PlayerPrefs.SetInt("leaderboard", settingsData.isLeaderboardShowed ? 1 : 0);
        PlayerPrefs.SetInt("season", settingsData.isSeasonShowed ? 1 : 0);
        PlayerPrefs.Save();
    }

    public SettingsData LoadSettings(){
        SettingsData sd = new SettingsData();
        sd.sound = PlayerPrefs.GetInt("sound", 1) == 1 ? true : false;
        sd.isLeaderboardShowed = PlayerPrefs.GetInt("leaderboard", 0) == 1 ? true : false;
	sd.isSeasonShowed = PlayerPrefs.GetInt("season", 0) == 1 ? true : false;
        settingsData = sd;
        return sd;
    }

    public void DeleteAll(){
	try {
	    foreach(var directory in Directory.GetDirectories(Application.persistentDataPath)) {
		DirectoryInfo dInfo = new DirectoryInfo(directory);
		dInfo.Delete(true);
	    }
	    foreach(var file in Directory.GetFiles(Application.persistentDataPath)) {
                FileInfo fInfo = new FileInfo(file);
                fInfo.Delete();
            }
	    Debug.Log("Deleted Everything!");	    
	} catch (Exception e){
            Debug.LogError("Error: " + e.Message);
        }
    }

}

[System.Serializable]
public class SettingsData {
    public bool sound;
    public bool isLeaderboardShowed;
    public bool isSeasonShowed;

    public SettingsData(){
	this.sound = false;
        this.isLeaderboardShowed = false;
        this.isSeasonShowed = false;
    }
}
