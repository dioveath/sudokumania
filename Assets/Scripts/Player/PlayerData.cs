using System;

[Serializable]
public class PlayerData {

    public string id;
    public string email;
    public string fullName;
    public string authProvider;

    public PlayerData() { }

    public PlayerData(string id, string email, string fullName, string authProvider){
        this.id = id;
        this.email = email;
        this.fullName = fullName;
        this.authProvider = authProvider;
    }

}
