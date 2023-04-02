using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundsManager
{
    public enum Sound
    {
        ButtonClick,
        ButtonHover,
        Building,
        City,
        Attack,
        Shooting,
        UnitTrained,
        UnitDeath
    }
    public static void PlaySound(Sound sound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        soundGameObject.AddComponent<AudioSourceManager>();
        audioSource.PlayOneShot(getAudioClip(sound));
    }

    private static AudioClip getAudioClip(Sound sound)
    {
        foreach (SoundAssets.SoundAudioClip audioClip in SoundAssets.Instance.soundAudioClips)
        {
            if (audioClip.sound == sound) return audioClip.audioClip;
        }
        return null;
    }
}
