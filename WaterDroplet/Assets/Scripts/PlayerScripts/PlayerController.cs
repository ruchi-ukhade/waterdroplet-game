using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Ruchi
    //Animation
    public Animator animator;

    // Lock movements
    public bool movementEnabled = true;

    // Movement 
    private float speed;
    public float groundSpeed;
    public float airSpeed;
    public float moveInput;
    //[SerializeField] private bool facingRight = true;


    // Jump
    private float jumpForce;
    [SerializeField] private float jumpForceS;
    [SerializeField] private float jumpForceM;
    [SerializeField] private float jumpForceL;
    private float jumpBufferTimeAvaliable = 0f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    private float coyoteTimeAvaliable = 0f;
    [SerializeField] private float coyoteTime = 0.1f;


    // Player Size
    // 1 = small; 2 = meidum; 3 = large
    [Range(1, 3)] public int playerSize;
    private Vector2 initialSize;
    private Vector2 targetSize;
    public float growDuration = 0.5f;

    // Player Death
    private bool isDead = false;
    public float respawnDelay = 0.1f;



    // Player Physics
    public Rigidbody2D rb;
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
    private bool standOnBox = false;
    private bool pushingBox = false;


    // Landing animation
    private bool jumped = false;
    private bool grounded = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        layersPlayerCanStand = LayerMask.GetMask("Ground", "Box");

        // Initalize player size
        jumpForce = jumpForceL;
        playerSize = 3;

        //Ruchi
        //get the Animator component attached to the same GameObject
        animator = GetComponent<Animator>();
    }


    private void FixedUpdate()
    {
        // If player movement is diaabled, do nothing
        if (movementEnabled == false) return;
        // If player is dead, do nothing
        if (isDead) return;
        

        if (isGrounded(layersPlayerCanStand))
        {
            speed = groundSpeed;
        }
        else if (!isGrounded(layersPlayerCanStand))
        {
            speed = airSpeed;
        }

        // Player horizontal Movement
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        
        if (moveInput > 0) // moving right
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            
        }
        else if (moveInput < 0) // moving left
        {
            transform.localScale = new Vector2(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }


        // Stop pushing when character stop
        if (pushingBox == true && moveInput == 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            SoundManager.StopSound(SoundType.PUSH, 0.1f);
            pushingBox = false;
        }

    }



    // Update is called once per frame
    void Update()
    {
        grounded = isGrounded(layersPlayerCanStand);

        // If player movement is diaabled, do nothing
        if (movementEnabled == false) return;
        // If player is dead, do nothing
        if (isDead) return;

        // Suicide restart
        if (Input.GetKey(KeyCode.R))
        {
            Die();
        }


        // coyoteTime
        coyoteTimeAvaliable -= Time.deltaTime;
        if (isGrounded(layersPlayerCanStand))
        {
            coyoteTimeAvaliable = coyoteTime;
        }

        // Jump buffer
        jumpBufferTimeAvaliable -= Time.deltaTime;
        if (Input.GetButtonDown("Jump")) {
            jumpBufferTimeAvaliable = jumpBufferTime; // when jump is pressed set jump buffer time
        }


        // Player Jump
        if ((jumpBufferTimeAvaliable > 0) && (coyoteTimeAvaliable > 0))
        {
            // Set the IsJumping parameter to true
            animator.SetBool("IsJumping", true);

            jumpBufferTimeAvaliable = 0;
            coyoteTimeAvaliable = 0;
            rb.velocity = Vector2.zero;
            //rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // Increment player jump stats
            LevelManager.Instance.incrementJumps();

            SoundManager.PlaySound(SoundType.JUMP, 0.3f);
            jumped = true;
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

        //Ruchi
        // Update the Animator
        animator.SetBool("IsPushing", pushingBox);

        // Manual Resizing (Scale) for debug
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



        // Shiver aniamtion
        if (playerSize >= 3) { animator.SetBool("IsShivering", false); }
        else { animator.SetBool("IsShivering", (isInWater || isInSand)); }

        // Grow & Shrink based on sand and water
        if (isInWater)
        {
            //Ruchi 
            // Start shivering animation
            if (playerSize < 3)
            {
                //animator.SetBool("IsShivering", true);
            }

            timeInSand = 0f;
            timeInWater = timeInWater + Time.deltaTime;
            if(timeInWater >= growthTimeNeeded)
            {
                changeSize(true); //grow bigger
                timeInWater = 0f;
            }
        }
        //else { timeInWater = 0f;}

        if (isInSand)
        {
            //Ruchi 
            // Start shivering animation
            //animator.SetBool("IsShivering", true);

            timeInWater = 0f;
            timeInSand = timeInSand + Time.deltaTime;
            if (timeInSand >= shrinkTimeNeeded)
            {
                changeSize(false); //shrink smaller
                timeInSand = 0f;
            }
        }
        //else { timeInSand = 0f; }




    }


    #region Change player size
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
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Die();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            //animator.SetBool("IsShivering", false);
        }
        else if (other.CompareTag("Sand"))
        {
            isInSand = false;
            //animator.SetBool("IsShivering", false);
        }
    }



    // Change water droplet Size
    public void changeSize(bool getBigger)
    {
        if (getBigger && playerSize != 3) // touch water, getting bigger
        {
            if (playerSize == 1)
            {
                setSize(2, 0.2f, jumpForceM);
            }
            else if (playerSize == 2)
            {
                setSize(3, 0.3f, jumpForceL);
            }
        }
        else if (!getBigger) // touch sand, getting smaller
        {
            if (playerSize == 1)
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Die();
            }
            if (playerSize == 3)
            {
                setSize(2, 0.2f, jumpForceM);
            }
            else if (playerSize == 2)
            {
                setSize(1, 0.12f, jumpForceS);
            }
        }
    }

    // Set player's size
    private void setSize(int size, float scale, float newJumpforce)
    {
        playerSize = size; // player size number (1,2,3)
        initialSize = transform.localScale; // actual current player size
        targetSize = new Vector2(Mathf.Sign(transform.localScale.x) * scale, scale);
        jumpForce = newJumpforce;



        StartCoroutine(ResizeOverTime());
        //transform.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * scale, scale);

        //Increment player size changes stats
        LevelManager.Instance.incrementSizeChanges();
    }

    private IEnumerator ResizeOverTime()
    {
        //animator.SetBool("IsShivering", true);
        float elapsedTime = 0f; // inital time
     
        while (elapsedTime < growDuration)
        {
            elapsedTime += Time.deltaTime;
            // value t between 0 and 1 that represents the progress of the growth.
            float t = Mathf.Clamp01(elapsedTime / growDuration);
            // Lerp from initial to target by t%
            transform.localScale = Vector2.Lerp(initialSize, targetSize, t);
            yield return null;
        }

        // Ensure the final size is exactly the target size
        transform.localScale = targetSize;
        //Ruchi
        // Stop shivering animation after resizing is complete
        //animator.SetBool("IsShivering", false);
    }


    #endregion



    #region Player Death and respwan
    //Player Death
    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            // Disable player movements
            rb.velocity = Vector2.zero;
            rb.simulated = false;

            // Start respawn process
            StartCoroutine(RespawnCoroutine());
        }
    }
    // Player Respawn
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        // Set player size back to when it touches the check point
        int size = LevelManager.Instance.GetPlayerSize();
        if (size == 1) { setSize(size, 0.12f, jumpForceS); }
        else if (size == 2) { setSize(size, 0.2f, jumpForceM); }
        else if (size == 3) { setSize(size, 0.3f, jumpForceL); }

        // Reset the level
        LevelManager.Instance.ResetLevel();

        // Reset position to last checkpoint
        transform.position = LevelManager.Instance.GetLastCheckpoint();


        animator.SetTrigger("isDead");
        yield return new WaitForSeconds(0.8f);
        // Re-enable player control
        isDead = false;
        rb.simulated = true;

        // Increment retry counts
        LevelManager.Instance.incrementRetries();
    }
    #endregion


    // Collision againtst ground after a jump
    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Box"))
            && jumped)
        {            
            animator.SetTrigger("landed");
            jumped = false;
        }

    }

    // When Pushing Boxes, set bool values for pushingBox   
    void OnCollisionStay2D(Collision2D collision)
    {
        // Determine whether the player is standing on a box
        standOnBox = isGrounded(boxLayer);
        if (collision.gameObject.CompareTag("Box") && !standOnBox)
        {

            if (pushingBox == false)
            {
                SoundManager.PlaySound(SoundType.PUSH, 0.5f);
            }
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

            SoundManager.StopSound(SoundType.PUSH, 0.1f);
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



    // Enable or disable character movement
    public void SetMovement(bool moveStats)
    {
        movementEnabled = moveStats;
        // Stop player
        rb.velocity = Vector2.zero;
    }



}
