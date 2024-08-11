using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject respawnPoint;
    public PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.SetCheckpoint(respawnPoint.transform.position);
            GameManager.Instance.SetPlayerSize(playerController.playerSize);
        }
    }
}
