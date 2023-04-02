using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 1 && (audioSource == null || audioSource.isPlaying == false))
        {
            Destroy(gameObject);
        }
    }
}
