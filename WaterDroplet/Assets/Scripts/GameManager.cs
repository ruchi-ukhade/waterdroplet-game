using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //private Vector2 lastCheckpoint;
    //private int playerSize; // record player size when it reaches a checkpoint

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

/*
    // Setter and Getter for check point position
    public void SetCheckpoint(Vector2 position)
    {
        lastCheckpoint = position;
    }

    public Vector2 GetLastCheckpoint()
    {
        return lastCheckpoint;
    }


    // Setter and Getter for player size when hit check point 
    public void SetPlayerSize(int size)
    {
        playerSize = size;
    }
    public int GetPlayerSize()
    {
        return playerSize;
    }
*/
}
