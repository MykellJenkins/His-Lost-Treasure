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

    
    
    }














}
