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
       StopAllCoroutines();


        StartCoroutine(FadeTrack(newClip));
        isPlayingTrack01 = !isPlayingTrack01;


    }

    public void ReturntoDefault()
    {
        SwapTrack(defaultAmbience);
    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {

        float timeToFade = 1.25f;
        float timeElasped = 0;
        if (isPlayingTrack01)
        {
            track02.clip = newClip;
            track02.Play();

            while(timeElasped < timeToFade)
            {
                track02.volume = Mathf.Lerp(0, 1, timeElasped / timeToFade);
                track01.volume = Mathf.Lerp(1, 0, timeElasped / timeToFade);
                timeElasped += Time.deltaTime;
                yield return null;
            }

            track01.Stop();
        }
        else
        {
            track01.clip = newClip;
            track01.Play();

            while (timeElasped < timeToFade)
            {
                track01.volume = Mathf.Lerp(0, 1, timeElasped / timeToFade);
                track02.volume = Mathf.Lerp(1, 0, timeElasped / timeToFade);
                timeElasped += Time.deltaTime;
                yield return null;
            }
                track02.Stop();
        }
        

    }












}
