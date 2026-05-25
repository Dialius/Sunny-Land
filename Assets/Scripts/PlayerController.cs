using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool isHurt;
    private bool isClimbing; // New
    private float horizontalInput;
    private float verticalInput; // New

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Input
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical"); // New for Climb/Crouch

        // Jump (Disable jump if crouching or climbing?)
        // For now, allow jump unless crouching
        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Animation State
        if (!isHurt) // Only update normal animations if not hurt
        {
            UpdateAnimationState();
        }
    }

    // New method to trigger Hurt animation
    public void TriggerHurt()
    {
        isHurt = true;
        anim.SetTrigger("hurt");
        
        // Push player back
        rb.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 5f, 5f);
        
        // Make sure player renders in front of enemy when hurt/dying
        spriteRenderer.sortingOrder = 50; 

        // Reset hurt state after a short delay (simple way)
        Invoke("ResetHurt", 0.5f);
    }

    void ResetHurt()
    {
        isHurt = false;
        spriteRenderer.sortingOrder = 0; // Restore sorting order (or whatever the default is, usually 0)
    }

    void FixedUpdate()
    {
        if (isHurt) return;

        // Climb Logic
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * moveSpeed);
            return; // Skip normal movement
        }
        else
        {
            rb.gravityScale = 3f; // Reset gravity (assuming default is 3 or 1?)
             // Better to store initial gravity in Start()
        }

        // Crouch Logic (Simple: Stop moving if crouching)
        bool isCrouching = verticalInput < -0.1f && isGrounded;
        
        if (isCrouching)
        {
            rb.linearVelocity = Vector2.zero; // Stop
            anim.SetBool("isCrouching", true);
        }
        else
        {
            anim.SetBool("isCrouching", false);
            // Normal Movement
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Flip Sprite
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    void UpdateAnimationState()
    {
        if (isClimbing)
        {
            anim.SetBool("isClimbing", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isJumping", false);
            return;
        }
        else
        {
            anim.SetBool("isClimbing", false);
        }

        if (Mathf.Abs(horizontalInput) > 0.1f && isGrounded)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (!isGrounded)
        {
            anim.SetBool("isJumping", true);
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
       
        // Fall animation logic could be added here if you have a separate fall animation
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }
    
    // Visualize Ground Check in Editor
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            // Green if grounded, Red if in air
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    // Ladder Detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 3f; // Restore gravity found in inspector
        }
    }
}
