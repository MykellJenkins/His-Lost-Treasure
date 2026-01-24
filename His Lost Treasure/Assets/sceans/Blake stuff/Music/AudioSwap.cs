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
            AudioManagement.instance.SwapTrack(newTrack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManagement.instance.ReturntoDefault();
        }
    }







}
