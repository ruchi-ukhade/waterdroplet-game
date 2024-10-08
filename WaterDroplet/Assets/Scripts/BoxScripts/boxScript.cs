using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IResettable
{

    public PlayerController playerController;
    private Rigidbody2D rb;

    // 1 = small; 2 = meidum; 3 = large
    [SerializeField] [Range(1, 3)] private int boxSize;

    private bool isBeingPushed = false;
    private int pushDirection = 0; // -1 = left, 1 = right

    private Vector2 initialPosition;


    // Start is called before the first frame update
    void Start()
    {
        // Register Resettable Object
        LevelManager.Instance.RegisterResettableObject(this);

        rb = GetComponent<Rigidbody2D>();
        rb.mass = 40f;
    }

    private void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            return;
        }

        if ((playerController.moveInput != pushDirection) || !isBeingPushed)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }



    // Update is called once per frame
    void Update()
    {
        // Unable to move this box when the player size is smaller
        if (playerController.playerSize < boxSize && rb.velocity.y == 0)
        {
            rb.bodyType = RigidbodyType2D.Static;

        }
        // Able to move this boxe when the player size is same or bigger
        else if (playerController.playerSize == boxSize)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 40f;
        }
        else if (playerController.playerSize > boxSize)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = 25f;
        }
    }




    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 center = collision.gameObject.transform.position;
            float direction = contactPoint.x - center.x;

            if (Mathf.Abs(direction) > 0.0001f)
            {
                isBeingPushed = true;
                pushDirection = direction > 0 ? 1 : -1;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //pushDirection = 0;
            isBeingPushed = false;
        }
    }

    public void SaveInitialState()
    {
        initialPosition = transform.position;
    }

    public void ResetToInitialState()
    {
        transform.position = initialPosition;        
    }

}