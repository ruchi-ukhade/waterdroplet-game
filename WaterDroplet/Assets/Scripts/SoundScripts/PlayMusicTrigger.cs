using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicTrigger : MonoBehaviour
{
    [SerializeField] private SoundType sound;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [SerializeField] private float fade = 1f;
    [SerializeField] private bool loop = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            StartCoroutine(playSound());
        }
    }
    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.PlaySound(sound, volume, fade, loop);
    }
}
