using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    FORESTVOICE
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList; // List of all sounds
    private static SoundManager instance;
    private AudioSource audioSource; //Audio that will get played

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Play sound, input sound type and volume(preset to be 1)
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    // Stop the currently playing sound
    public static void StopSound()
    {
        instance.audioSource.Stop();
    }
}
