using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] tracks;

    private AudioSource source;

    int currentTrack = 0;
    int newTrack = 0;

    void Start()
    {
        source = GetComponent<AudioSource>();

        source.clip = tracks[0];
        source.Play();
    }

    public void ChangeTrack()
    {
        newTrack = Random.Range(0, tracks.Length);
        
        if(newTrack == currentTrack)
            newTrack++;

        if (newTrack >= tracks.Length)
            newTrack = 0;
        
        source.clip = tracks[newTrack];
        currentTrack = newTrack;
        source.Play();
    }

    public void PauseMusic()
    {
        source.Pause();
        GetComponent<AudioListener>().enabled = false;
    }

    public void UnpauseMusic()
    {
        GetComponent<AudioListener>().enabled = true;
        source.Play();
        
    }
}
