using System;
using UnityEngine.Audio; 
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Source
    {
        ENGINE,
        FRONT,
        LEFT,
        RIGHT
    }
    #region variables

    public Sound[] sounds;
    public AudioSource engine;
    public AudioSource front;
    public AudioSource left;
    public AudioSource right;

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

    public void Play(Source source, string name, bool loop)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the right sound, where sound is equal to the used name
        if (s == null)
        {
            return; // if there is any problem with the soundfile
            
        }
        //s.source.Play();
        
        switch (source)
        {
            case Source.ENGINE:
                engine.clip = s.clip;
                engine.loop = loop;
                engine.Play();
                break;
            case Source.FRONT:
                front.clip = s.clip;
                front.loop = loop;
                front.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }
    }

    public string isPlaying(Source source)
    {
        switch (source)
        {
            case Source.ENGINE:
                if (engine.isPlaying)
                {
                    return engine.clip.name;
                }
                break;
            case Source.FRONT:
                if (front.isPlaying)
                {
                    return front.clip.name;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }

        return "";
    }
    

    public void Stop(Source source)
    {
        switch (source)
        {
            case Source.ENGINE:
                engine.Stop();
                break;
            case Source.FRONT:
                front.Stop();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }
        /*
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the right sound, where sound is equal to the used name
        if (s == null)
        {
            return; // if there is any problem with the soundfile
            
        }
        s.source.Stop();
        */
    }
    
    
    
    #endregion
    
}// End class
