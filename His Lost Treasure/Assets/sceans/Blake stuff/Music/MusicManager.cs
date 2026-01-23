using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource sourceA;
    public AudioSource sourceB;

    public float fadeDuration = 2f;

    private AudioSource currentSource;
    private AudioSource nextSource;

    void Awake()
    {
        currentSource = sourceA;
        nextSource = sourceB;
    }

    public void PlaySong(AudioClip newClip)
    {
        StartCoroutine(CrossFade(newClip));
    }

    IEnumerator CrossFade(AudioClip newClip)
    {
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            currentSource.volume = Mathf.Lerp(1f, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 1f;

        // Swap sources
        var temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }
}
