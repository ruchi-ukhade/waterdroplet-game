using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPlayerController : MonoBehaviour
{
    public float speed = 10;
    //private float moveInput;

    private Rigidbody2D rb;
    public float jumpForce = 27f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 direction = new Vector2(x, y);

        
        rb.velocity = new Vector2(x * speed, rb.velocity.y);


        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("jump!");
            Jump(Vector2.up);
        }
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }


}
