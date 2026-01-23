using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSwap : MonoBehaviour
{
    public AudioClip newTrack;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioClip newClip = Resources.Load<AudioClip>("NewAmbience"); // Replace with your audio clip path
            AudioManagement.instance.SwapTrack(newTrack);
        }

    }
}
