using Firebase.Auth;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData _playerData { get; private set; }

    void Start()
    {
        AuthManager.Instance().authStateChangedUEvent.AddListener(AuthStateChangeCallback);
	_playerData = new PlayerData();
    }

    private void AuthStateChangeCallback(FirebaseUser user)
    {
	if(user != null){
	    _playerData.id = user.UserId;
	    _playerData.email = user.Email;
	    _playerData.fullName = user.DisplayName;
	    _playerData.authProvider = user.ProviderId;

            SaveManager.Instance.SavePlayerData(_playerData); 
        }
    }

    public void UpdatePlayer(PlayerData newData){
	_playerData.id = newData.id;
	_playerData.email = newData.email;
	_playerData.fullName = newData.fullName;
	_playerData.authProvider = newData.authProvider;

	SaveManager.Instance.SavePlayerData(_playerData); 	
    }

}
