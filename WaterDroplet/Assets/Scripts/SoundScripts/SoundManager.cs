using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    FORESTVOICE,
    FORESTAMBIENCE,
    GAMEPLAYERMUSIC,
    JUMP,
    RESPAWN,
    CAVESOUND,
    PUSH
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList; // List of all sounds
    private static SoundManager instance;
    //private AudioSource audioSource; //Audio that will get played
    private Dictionary<SoundType, AudioSource> audioSources = new Dictionary<SoundType, AudioSource>();

    // Fade in and out audio
    private Coroutine fadeCoroutine;


    private void Awake()
    {
        instance = this;
        InitializeAudioSources();
    }
    //private void Start()
    //{
    //    audioSource = GetComponent<AudioSource>();
    //}
    private void Start()
    {
        foreach (AudioClip clip in soundList)
        {
            clip.LoadAudioData();
        }
    }
    private void InitializeAudioSources()
    {
        foreach (SoundType soundType in System.Enum.GetValues(typeof(SoundType)))
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            audioSources[soundType] = source;
        }
    }


    // Play sound
    // Input: sound type, volume(preset to be 1), fadeInDuration(preset to be 0)
    public static void PlaySound(SoundType sound, float volume = 1, float fadeInDuration = 0, bool loop = false)
    {
        //instance.StopFadeCoroutine();

        // Play sound from volume 0
        AudioSource source = instance.audioSources[sound];
        source.clip = instance.soundList[(int)sound];
        source.volume = fadeInDuration > 0 ? 0 : volume;
        source.loop = loop;
        source.Play();

        // Fade in a sound
        if (fadeInDuration > 0)
        {
            //instance.fadeCoroutine = instance.StartCoroutine(instance.FadeSound(0, volume, fadeInDuration));
            instance.StartCoroutine(instance.FadeSound(source, 0, volume, fadeInDuration));
        }

        //instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }


    // Stop the currently playing sound
    public static void StopSound(SoundType sound, float fadeOutDuration = 0)
    {
        AudioSource source = instance.audioSources[sound];

        // Fade out a sound
        if (fadeOutDuration > 0)
        {
            instance.StartCoroutine(instance.FadeSound(source, source.volume, 0, fadeOutDuration, true));
        }
        else
        {
            source.Stop();
        }
    }





    // Fade in & out a sound
    private IEnumerator FadeSound(AudioSource source,
        float startVolume, float endVolume,
        float duration, bool stopAfterFade = false)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration) // Lerp from start to endVolume over duration
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
            source.volume = newVolume;
            yield return null;
        }
        source.volume = endVolume;

        // For fading out a sound
        // Stop the audio after fade out
        if (stopAfterFade && endVolume == 0)
        {
            source.Stop();
        }
    }


    // Interrupt a currenly going on fade coroutine
    private void StopFadeCoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }
}
