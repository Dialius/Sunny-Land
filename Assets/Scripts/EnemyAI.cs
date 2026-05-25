using UnityEngine;

public enum EnemyType { GroundPatrol, FlyPatrol, JumpPatrol, Hanging }

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Type")]
    public EnemyType enemyType = EnemyType.GroundPatrol;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    
    [Header("Jump Settings (For Frog)")]
    public float jumpForceX = 2f;
    public float jumpForceY = 5f;
    public float jumpDelay = 2f;
    private float jumpTimer;
    private bool isGrounded;

    [Header("Hanging Settings (For Bat)")]
    public float wakeDistance = 5f;
    private bool isHanging;

    [Header("Combat")]
    public int damage = 1;

    [Header("Air Combat (Eagle)")]
    public bool canDive = false;
    public float diveSpeed = 10f;
    public float diveRecoverSpeed = 2f;
    private bool isDiving;
    private Vector3 originalPosition;
    private Transform playerTransform;

    [Header("Effects")]
    public GameObject deathEffectPrefab;

    [Header("Health & Hurt")]
    public int maxHP = 1;
    private int currentHP;
    private bool isDead;

    // Internal Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        originalPosition = transform.position;
        
        // Detach patrol points
        if(leftPoint != null) leftPoint.parent = null;
        if(rightPoint != null) rightPoint.parent = null;

        // Setup specific physics based on type
        if (enemyType == EnemyType.FlyPatrol || enemyType == EnemyType.Hanging)
        {
            rb.gravityScale = 0; // Flying enemies don't fall
        }

        if (enemyType == EnemyType.Hanging)
        {
            isHanging = true;
            if(anim != null) anim.SetBool("isHanging", true);
        }
    }

    void Update()
    {
        if (isDead) return;

        switch (enemyType)
        {
            case EnemyType.GroundPatrol:
                Patrol();
                break;
            case EnemyType.FlyPatrol:
                if (canDive && !isDiving) CheckForDive();
                if (isDiving) DiveLogic();
                else Patrol();
                break;
            case EnemyType.JumpPatrol:
                JumpPatrol();
                break;
            case EnemyType.Hanging:
                HangingLogic();
                break;
        }
    }

    void CheckForDive()
    {
        // Simple Raycast down to find player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            StartCoroutine(PerformDive(hit.collider.transform));
        }
    }

    System.Collections.IEnumerator PerformDive(Transform target)
    {
        isDiving = true;
        anim.SetTrigger("dive");
        
        // Swoop down
        Vector2 targetPos = new Vector2(target.position.x, target.position.y);
        
        while (Vector2.Distance(transform.position, targetPos) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, diveSpeed * Time.deltaTime);
            yield return null;
        }

        // Wait a bit
        yield return new WaitForSeconds(0.5f);

        // Return up
        Vector2 recoverPos = new Vector2(transform.position.x, originalPosition.y); // Return to original height
        while (Vector2.Distance(transform.position, recoverPos) > 0.1f)
        {
             transform.position = Vector2.MoveTowards(transform.position, recoverPos, diveRecoverSpeed * Time.deltaTime);
             yield return null;
        }
        
        isDiving = false;
        anim.SetTrigger("fly"); // Back to fly
    }

    void DiveLogic() { /* Handled by Coroutine */ }

    // ... (HangingLogic) ...

    public void TakeDamage(int dmg)
    {
        if(isDead) return;

        currentHP -= dmg;
        anim.SetTrigger("hurt");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (collision.relativeVelocity.y < 0 && collision.transform.position.y > transform.position.y) 
            {
                // Player stomped on us
                TakeDamage(1); // Stomp kills 1 HP
                player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(player.GetComponent<Rigidbody2D>().linearVelocity.x, 5f);
            }
            else
            {
                if(player != null) player.TriggerHurt();
                GameManager.instance.TakeDamage(damage);
            }
        }
    }

    void Die()
    {
        isDead = true;
        rb.simulated = false; // Disable physics
        anim.SetTrigger("death"); // If you have a death anim

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 0.5f); // Wait for anim
        
        if(leftPoint != null) Destroy(leftPoint.gameObject);
        if(rightPoint != null) Destroy(rightPoint.gameObject);
    }

    [Header("Patrol Settings")]
    public float waitTime = 2f;
    [Tooltip("If true, the enemy will move up and down instead of left and right. Useful for eagles.")]
    public bool isVerticalPatrol = false;
    private float waitTimer;

    void Patrol()
    {
        if (isVerticalPatrol)
        {
            Transform targetPoint = movingRight ? rightPoint : leftPoint;
            float distanceY = Mathf.Abs(targetPoint.position.y - transform.position.y);

            if (distanceY <= 0.2f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                if(anim != null) anim.SetBool("isMoving", false);

                waitTimer -= Time.deltaTime;
                if(waitTimer <= 0)
                {
                    movingRight = !movingRight;
                    waitTimer = waitTime;
                }
            }
            else
            {
                float moveDirY = Mathf.Sign(targetPoint.position.y - transform.position.y);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveDirY * moveSpeed);
                if(anim != null) anim.SetBool("isMoving", true);
            }
        }
        else
        {
            if (movingRight)
            {
                if (transform.position.x >= rightPoint.position.x)
                {
                    // Reached point: Stop and Wait
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    if(anim != null) anim.SetBool("isMoving", false);

                    waitTimer -= Time.deltaTime;
                    if(waitTimer <= 0)
                    {
                        movingRight = false;
                        waitTimer = waitTime;
                    }
                }
                else
                {
                    // Keep moving
                    rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
                    spriteRenderer.flipX = true;
                    if(anim != null) anim.SetBool("isMoving", true);
                }
            }
            else
            {
                if (transform.position.x <= leftPoint.position.x)
                {
                    // Reached point: Stop and Wait
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    if(anim != null) anim.SetBool("isMoving", false);

                    waitTimer -= Time.deltaTime;
                    if(waitTimer <= 0)
                    {
                        movingRight = true;
                        waitTimer = waitTime;
                    }
                }
                else
                {
                    // Keep moving
                    rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
                    spriteRenderer.flipX = false;
                    if(anim != null) anim.SetBool("isMoving", true);
                }
            }
        }
    }

    void JumpPatrol()
    {
        // Proper ground check using Raycast instead of velocity
        Collider2D col = GetComponent<Collider2D>();
        bool onGround = false;
        
        if (col != null)
        {
            // Raycast slightly below the feet to check for actual ground
            Vector2 rayStart = new Vector2(col.bounds.center.x, col.bounds.min.y + 0.05f);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.15f);
            
            if (hit.collider != null && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) 
            {
                onGround = true;
            }
        }
        else
        {
            // Fallback
            onGround = Mathf.Abs(rb.linearVelocity.y) < 0.1f;
        }

        // Failsafe: if moving upwards, definitely not grounded
        if (rb.linearVelocity.y > 0.1f)
        {
            onGround = false;
        }

        isGrounded = onGround;

        if (isGrounded)
        {
            // We are on the ground, reset jumping animation state if it was a bool, 
            // or just use isMoving false to show idle state
            if(anim != null) anim.SetBool("isMoving", false);

            // Stop horizontal sliding only
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            
            jumpTimer -= Time.deltaTime;

            // Face the direction we want to go
            if (movingRight) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;

            if (jumpTimer <= 0)
            {
                Jump();
                jumpTimer = jumpDelay;
            }
        }
        else
        {
            // We are in the air
            if(anim != null) anim.SetBool("isMoving", true);
        }
    }

    void JumpingLogic()
    {
        // ... (Existing jump logic)
    }

    void HangingLogic()
    {
        if (isHanging)
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < wakeDistance)
                {
                    WakeUp();
                }
            }
        }
        else
        {
            // Once awake, fly like an eagle
            Patrol();
        }
    }

    void WakeUp()
    {
        isHanging = false;
        if(anim != null) anim.SetBool("isHanging", false);
        // Maybe set enemyType to FlyPatrol so Update() switches logic?
        // Or just let HangingLogic call Patrol().
        // Let's keep it simple: hanging check -> if awake call Patrol.
    }

    void Jump()
    {
        if(anim != null) anim.SetTrigger("jump"); // Trigger jump animation ONCE

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(jumpForceX, jumpForceY);
            if (transform.position.x >= rightPoint.position.x) movingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-jumpForceX, jumpForceY);
            if (transform.position.x <= leftPoint.position.x) movingRight = true;
        }
    }


}
