using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Movement 
    private float speed;
    public float groundSpeed;
    public float airSpeed;
    private float moveInput;
    private bool facingRight = true;

    // Jump
    private float jumpForce;
    [SerializeField] private float jumpForceS;
    [SerializeField] private float jumpForceM;
    [SerializeField] private float jumpForceL;

    // Player Size
    // 1 = small; 2 = meidum; 3 = large
    [Range(1, 3)] public int playerSize;
    [SerializeField] private Vector2 playerSizeS = new Vector2(0.12f, 0.12f);
    [SerializeField] private Vector2 playerSizeM = new Vector2(0.2f, 0.2f);
    [SerializeField] private Vector2 playerSizeL = new Vector2(0.3f, 0.3f);


    // Player Physics
    private Rigidbody2D rb;
    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private Collider2D _bodyCollider;

    // Layers
    [SerializeField] private LayerMask groundLayer;

    // Interaction with water and sand
    // Grow and Shrink
    private float timeInWater = 0f;
    private bool isInWater = false;
    public float growthTimeNeeded = 1f;
    private float timeInSand = 0f;
    private bool isInSand = false;
    public float shrinkTimeNeeded = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        jumpForce = jumpForceS;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded())
        {
            speed = groundSpeed;
        }
        else if (!isGrounded())
        {
            speed = airSpeed;
        }

        // Player horizontal Movement
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            Flip();
        }

        // Player Jump
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }


        // Resizing (Scale)
        if (Input.GetKey(KeyCode.Alpha1)) // Small
        {
            transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.12f, 0.12f);
            jumpForce = jumpForceS;
            playerSize = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha2)) // Medium
        {
            transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.2f, 0.2f);
            jumpForce = jumpForceM;
            playerSize = 2;
        }
        else if (Input.GetKey(KeyCode.Alpha3)) // Large
        {
            transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.3f, 0.3f); ;
            jumpForce = jumpForceL;
            playerSize = 3;
        }


        // Grow & Shrink based on sand and water
        if (isInWater)
        {
            timeInWater += Time.deltaTime;
            if(timeInWater >= growthTimeNeeded)
            {
                changeSize(true); //grow bigger
                timeInWater = 0f;
            }
        }
        else { timeInWater = 0f;}

        if (isInSand)
        {
            timeInSand += Time.deltaTime;
            if (timeInSand >= shrinkTimeNeeded)
            {
                changeSize(false); //shrink smaller
                timeInSand = 0f;
            }
        }
        else { timeInSand = 0f; }
    }


    // Enter Sand & Water
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
        }
        else if (other.CompareTag("Sand"))
        {
            isInSand = true;

            if (playerSize == 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
        }
        else if (other.CompareTag("Sand"))
        {
            isInSand = false;
        }
    }



    private void changeSize(bool getBigger)
    {
        if (getBigger && playerSize != 3) // touch water, getting bigger
        {
            if (playerSize == 1)
            {
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.2f, 0.2f);
                jumpForce = jumpForceM;
            }
            else if (playerSize == 2)
            {
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.3f, 0.3f);
                jumpForce = jumpForceL;
            }
            playerSize++;
        }
        else if (!getBigger) // touch sand, getting smaller
        {
            if (playerSize == 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (playerSize == 3)
            {
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.2f, 0.2f);
                jumpForce = jumpForceM;
            }
            else if (playerSize == 2)
            {
                transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * 0.12f, 0.12f);
                jumpForce = jumpForceS;
            }
            playerSize--;
        }
    }


    // Check if the player is grounded
    private bool isGrounded()
    {
        // use box cast to detact ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            _feetCollider.bounds.center,
            _feetCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );

        // In the air: raycastHit.collider is null
        // On the ground : raycastHit.collider is not null
        return raycastHit.collider != null;
    }

    // Change player direction
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }



}