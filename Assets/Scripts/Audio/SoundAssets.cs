using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundAssets : MonoBehaviour
{
    private static SoundAssets instance;
    public static SoundAssets Instance {
        get {
            return instance;
        }
    }

    public SoundAudioClip[] soundAudioClips;

    private void Awake()
    {
        instance = this;
    }

    [Serializable]
    public class SoundAudioClip
    {
        public SoundsManager.Sound sound;
        public AudioClip audioClip;
    }

}
