using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public PlayerController playerController;
    private Rigidbody2D rb;

    // 1 = small; 2 = meidum; 3 = large
    [SerializeField] [Range(1,3)] private int boxSize; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Unable to move this box when the player size is smaller
        if (playerController.playerSize < boxSize)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        // Able to move this boxe when the player size is same or bigger
        else 
        {
            if(playerController.playerSize > boxSize)
            {
                rb.mass = 8f;
            }else { rb.mass = 13f; }
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
