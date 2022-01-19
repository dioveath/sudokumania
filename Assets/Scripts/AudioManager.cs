using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
    public List<Sound> allSounds;
    public List<Sound> allMusics;

    public AudioSource _sfxSource;
    public AudioSource _musicSource;
    private bool _on;

    private static AudioManager _instance;

    void Awake(){
        _instance = this;
	SetSound(SaveManager.Instance.settingsData.sound);	
    }

    public static AudioManager Instance(){
        return _instance;
    }    

    void Start(){
        _sfxSource = GetComponent<AudioSource>();

    }

    public void SetSound(bool sound){
	_on = sound;
	if(_on && _musicSource.clip != null)
	    _musicSource.Play();
	else
	    _musicSource.Pause();	
    }

    public void ToggleSound(){
	_on = !_on;
	if(_on)
	    _musicSource.Play();
	else
	    _musicSource.Pause();	
    }

    public bool IsOn(){
	return _on;
    }

    public void PlayAudio(string name){
        if(!_on) return;
        Sound sound = allSounds.Find(e => e.name == name);
	if(sound != null){
            _sfxSource.clip = sound.audioClip;
            _sfxSource.Play();
        } else {
            Debug.LogWarning("No Sound found with name: " + name);
        }
    }

    public void PlayMusic(string name){
        if(!_on) return;
        Sound sound = allMusics.Find(e => e.name == name);
	if(sound != null){
            _musicSource.clip = sound.audioClip;
            _musicSource.loop = true;
	    _musicSource.Play();	    
	} else {
            Debug.LogWarning("No Music found with name: " + name);
        }
    }

}


[Serializable]
public class Sound {
    public AudioClip audioClip;
    public string name;

    public Sound(AudioClip __audioClip, string __name){
        this.audioClip = __audioClip;
        this.name = __name;
    }
}
