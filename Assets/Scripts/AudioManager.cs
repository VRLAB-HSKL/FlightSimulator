using System;
using UnityEngine.Audio; 
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Source
    {
        ENGINE,
        FRONT
    }
    #region variables

    public Sound[] sounds;
    public AudioSource engine;
    public AudioSource front;
    

    #endregion

    #region unity lifecycle

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            /*
            //s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            */
        }
    }

    #endregion

    #region methods

    public void Play(Source source, string name, bool loop)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // find the right sound, where sound is equal to the used name
        if (s == null)
        {
            throw new ArgumentException("No clip by that name");

        }
        switch (source)
        {
            case Source.ENGINE:
                engine.clip = s.clip;
                playSource(engine,loop, s);
                break;
            case Source.FRONT:
                playSource(front, loop, s);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }
    }

    private void playSource(AudioSource source,bool loop, Sound s)
    {
        /*
        if (source.clip == s.clip)
        {
            if (source.isPlaying)
            {
                return;
            }
        }*/
        source.Stop();
        source.volume = s.volume;
        source.clip = s.clip;
        source.loop = loop;
        source.Play();
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

    public void setVolume(Source source, float volume)
    {
        switch (source)
        {
            case Source.ENGINE:
                engine.volume = volume;
                break;
            case Source.FRONT:
                front.volume = volume;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }
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
    }
    
    
    
    #endregion
    
}// End class
