using System;
using System.Threading;
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
            s.source = s.parent.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = 1f;
            s.source.minDistance = 100f;
        }
    }

    #endregion

    #region methods

    public void Play(string name)
    {
        Sound s = mapSound(name);
        s.source.Play();
    }
    public bool isPlaying(string name)
    {
        Sound s = mapSound(name);
        return s.source.isPlaying;
    }


    public void Stop(string name)
    {
        mapSound(name).source.Stop();
    }

    public void setVolume(string name, float volume)
    {
        mapSound(name).source.volume = volume;
    }
    
    private Sound mapSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the right sound, where sound is equal to the used name
        if (s == null)
        {
            throw new ArgumentException($"No clip by that name {name}");

        }

        return s;
    }
    
    #endregion
    
}// End class
