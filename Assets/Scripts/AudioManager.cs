using System;
using UnityEngine.Audio; 
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region variables

    public Sound[] sounds;

    #endregion

    #region unity lifecycle

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    #endregion

    #region methods

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the right sound, where sound is equal to the used name
        if (s == null)
        {
            return; // if there is any problem with the soundfile
            
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the right sound, where sound is equal to the used name
        if (s == null)
        {
            return; // if there is any problem with the soundfile
            
        }
        s.source.Stop();
    }
    
    
    
    #endregion
    
}// End class
