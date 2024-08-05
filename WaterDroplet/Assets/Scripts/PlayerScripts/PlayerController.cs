using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Ruchi
    //Animation
    public Animator animator;

    // Movement 
    private float speed;
    public float groundSpeed;
    public float airSpeed;
    public float moveInput;
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
    //[SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask boxLayer;
    private LayerMask layersPlayerCanStand;

    // Interaction with water and sand
    // Grow and Shrink
    private float timeInWater = 0f;
    private bool isInWater = false;
    public float growthTimeNeeded = 1f;
    private float timeInSand = 0f;
    private bool isInSand = false;
    public float shrinkTimeNeeded = 1f;

    // Pushing Boxes
    public bool standOnBox = false;
    private bool pushingBox = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        jumpForce = jumpForceS;
        layersPlayerCanStand = LayerMask.GetMask("Ground", "Box");

        //Ruchi
        //get the Animator component attached to the same GameObject
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isGrounded(layersPlayerCanStand))
        {
            speed = groundSpeed;
        }
        else if (!isGrounded(layersPlayerCanStand))
        {
            speed = airSpeed;
        }

        // Player horizontal Movement
        //moveInput = Input.GetAxisRaw("Horizontal"); 
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        //transform.Translate(new Vector2(moveInput * speed, 0) * Time.deltaTime * speed);

        // Fliping player direction
        if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            Flip();
        }
    }
    // Update is called once per frame
    void Update()
    {

        // Player Jump
        if (Input.GetButtonDown("Jump") && isGrounded(layersPlayerCanStand))
        {
            rb.velocity = Vector2.zero;
            //rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        //Ruchi
        // Update the Animator
        animator.SetBool("IsPushing", pushingBox);

        // Check if the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set the IsJumping parameter to true
            animator.SetBool("IsJumping", true);
        }
        else
        {
            // Set IsJumping to false when the spacebar is not pressed
            animator.SetBool("IsJumping", false);
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
            timeInSand = 0f;
            timeInWater += Time.deltaTime;
            if(timeInWater >= growthTimeNeeded)
            {
                changeSize(true); //grow bigger
                timeInWater = 0f;
            }
        }
        //else { timeInWater = 0f;}

        if (isInSand)
        {
            timeInWater = 0f;
            timeInSand += Time.deltaTime;
            if (timeInSand >= shrinkTimeNeeded)
            {
                changeSize(false); //shrink smaller
                timeInSand = 0f;
            }
        }
        //else { timeInSand = 0f; }

        // Determine whether the player is standing on a box
        standOnBox = isGrounded(boxLayer);
      
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



    // Manual change Size for debug
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



    // When Pushing Boxes, change player movement into constant speed
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            pushingBox = true;
            rb.interpolation = RigidbodyInterpolation2D.None;
        }
    }
   
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            pushingBox = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }


    // Check if the player is grounded
    private bool isGrounded(LayerMask layerToDetect)
    {
        // use box cast to detact ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            _feetCollider.bounds.center,
            _feetCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            layerToDetect
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
