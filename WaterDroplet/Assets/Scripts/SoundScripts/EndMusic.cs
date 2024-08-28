using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMusic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.StopSound(SoundType.GAMEPLAYERMUSIC, 1f);
            SoundManager.StopSound(SoundType.CAVESOUND, 3f);
        }
    }
}
