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
            //add the audiosource to the parent gameobject and initialise it
            s.source = s.parent.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            //set sound to be 3d
            s.source.spatialBlend = 1f;
            //increase the minimal hearing distance to 100
            s.source.minDistance = 100f;
        }
    }

    #endregion

    #region methods
    
    /// <summary>
    /// Plays the sound file with the corresponding name.
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {
        Sound s = mapSound(name);
        s.source.Play();
    }
    /// <summary>
    /// Is the sound with the corresponding name currenty getting played?
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool isPlaying(string name)
    {
        Sound s = mapSound(name);
        return s.source.isPlaying;
    }


    public void Stop(string name)
    {
        mapSound(name).source.Stop();
    }

    /// <summary>
    /// Sets the volume of the audiosource of the sound to the input volume.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume"></param>
    public void setVolume(string name, float volume)
    {
        mapSound(name).source.volume = volume;
    }
    
    /// <summary>
    /// Map the name to the corresponding sound.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If no sound exists with the input name</exception>
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
