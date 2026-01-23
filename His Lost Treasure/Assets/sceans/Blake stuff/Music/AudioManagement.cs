using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManagement : MonoBehaviour
{
    public AudioClip defaultAmbience;

    public AudioSource track01, track02;
    private bool isPlayingTrack01;

    public static AudioManagement instance;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
         
        }
    }

    private void Start()
    {
        track01 = gameObject.AddComponent<AudioSource>();
        track02 = gameObject.AddComponent<AudioSource>();
        isPlayingTrack01 = true;

        SwapTrack(defaultAmbience);
    }


    public void SwapTrack(AudioClip newClip)
    { 
        if (isPlayingTrack01)
        {
            track02.clip = newClip;
            track02.Play();
            track01.Stop();
        }
        else
        {
            track01.clip = newClip;
            track01.Play();
            track02.Stop();
        }
        isPlayingTrack01 = !isPlayingTrack01;


    }














}
