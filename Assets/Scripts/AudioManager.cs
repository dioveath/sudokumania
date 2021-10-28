using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
    public List<Sound> allSounds;

    private AudioSource _sfxSource;

    private static AudioManager _instance;

    void Awake(){
        _instance = this;
    }

    public static AudioManager Instance(){
        return _instance;
    }    

    void Start(){
        _sfxSource = GetComponent<AudioSource>();
    }



    public void PlayAudio(string name){
        Sound sound = allSounds.Find(e => e.name == name);
	if(sound != null){
            _sfxSource.clip = sound.audioClip;
            _sfxSource.Play();
        } else {
            Debug.LogWarning("No Sound found with name: " + name);
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
