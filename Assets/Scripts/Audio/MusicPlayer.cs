using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public AudioListener audioListener;

    // Start is called before the first frame update
    void Start()
    {
        audioListener = GetComponent<AudioListener>();
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandom();
        }
    }
    void PlayRandom()
    {
        int clip = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[clip];
        audioSource.Play();
    }
}
