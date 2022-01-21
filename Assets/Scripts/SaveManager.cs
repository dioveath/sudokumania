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
