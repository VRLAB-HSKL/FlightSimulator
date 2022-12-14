using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;

    [Range(0f,1f)] // set slider for volume
    public float volume;
    
    [Range(.1f,3f)] // Set slider for pitch
    public float pitch;

    public bool loop;
    
    public GameObject parent; // parent to which the audio source gets appended
    
    [HideInInspector] // Not visible in inspector mode
    public AudioSource source;
}
