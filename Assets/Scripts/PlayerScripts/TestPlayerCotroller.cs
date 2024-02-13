using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
     private float horizontalMovement;
    private float speed = 8f;
    [SerializeField] private bool hasUsedDoubleJump;
    private float jumpingPower = 16f;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool isGrounded = false;
    public float groundRadius;

    [SerializeField] private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;

    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower;

    private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    Animator animator;

    [Header("Gun Attaque")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private Transform gunDirection;
    [SerializeField] private GameObject bullet;
    [SerializeField] private AudioSource pistolShot;

    [Header("SwordAttaque")]
    //Cooldown time between attacks (in seconds)
    public float cooldown = 0.5f;
    //Max time before combo ends (in seconds)
    public float maxTime = 0.8f;
    public int maxCombo = 3;
    int combo = 0;
    public float lastTime;

    public float dashForce = 10f; // La force du dash
    public float dashDuration = 0.2f; // 
        public float dashTimer = 0f;


    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pistolShot = GetComponent<AudioSource>();
                StartCoroutine(SwordAttackCoroutine());
    }

    private void Update()
    {
        PlayerMove();

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalMovement * speed, rb.velocity.y);
        }
        
            WallSlide();
                WallJump();
                    GroundCheck();
                    if (isWallSliding && !isGrounded)
                    {
                        animator.SetBool("isJumping", false);
                        animator.SetBool("isRunning", false);
                        animator.SetBool("isSliding", true);
                    }

                    if (!isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && dashTimer <= 0f)
                    {
                        // Applique une force de dash horizontale
                        rb.AddForce(new Vector2(transform.localScale.x * dashForce, 0), ForceMode2D.Impulse);
                        
                        // Active le timer du dash
                        dashTimer = dashDuration;
                    }

                    // Diminue le timer du dash
                    if (dashTimer > 0f)
                    {
                        dashTimer -= Time.deltaTime;
                    }
                        PlayerJump();

                animator.SetFloat("yAxis", rb.velocity.y);

        GunShot();

        if (!isWallJumping)
        {
            Flip();
        } else if (isWallSliding && hasUsedDoubleJump)
        {
            hasUsedDoubleJump = false;
        }

        

    }

    private void FixedUpdate()
    {
        
    }

    private void PlayerMove()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        if (horizontalMovement != 0)
        {
            animator.SetBool("isRunning", true);
        } else
        {
            animator.SetBool("isRunning", false);
        }
    }

private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump(jumpingPower);
                hasUsedDoubleJump = false;
            }
            else if (!hasUsedDoubleJump && !isWallSliding)
            {
                Jump(jumpingPower);
                hasUsedDoubleJump = true;
            }
        }
    }

    private void Jump(float jumpPower)
    {
        animator.SetBool("isJumping", true);
        rb.velocity = new UnityEngine.Vector2(rb.velocity.x, jumpPower);

    }
    private void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders =  Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, groundLayer);

        if (colliders.Length > 0)
        {
            isGrounded = true;
            hasUsedDoubleJump = false;
        }
        animator.SetBool("isJumping", !isGrounded);

    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !isGrounded)
        {
            animator.SetBool("isSliding", true);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            isWallSliding = true;
            horizontalMovement = 0f;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("isSliding", false);

        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void GunShot()
    {
        if (Input.GetKeyDown(KeyCode.P) && isGrounded)
                {
                    isShooting = true;
                    animator.SetBool("gunShot", true);
                    Debug.Log("Tiré");
                }else if(Input.GetKeyUp(KeyCode.P) || !isGrounded)
                {
                    isShooting = false;
                    animator.SetBool("gunShot", false);
                }
    }
    public void FxGunShot()
    {
        pistolShot.Play();
    }
    // Instanciation de l'objet Bullet
    public void BulletInstanciate()
    {
        GameObject newBullet = Instantiate(bullet, gunDirection.position, gunDirection.rotation);
        Rigidbody2D rbBullet = newBullet.GetComponent<Rigidbody2D>();

        if (isFacingRight)
        {
        rbBullet.AddForce(transform.right * bulletSpeed, ForceMode2D.Impulse);
        } else
        {
        rbBullet.AddForce(-transform.right * bulletSpeed, ForceMode2D.Impulse);
        }

    }

    IEnumerator SwordAttackCoroutine ()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                combo++;
                //Debug.Log("Attack" + combo);
                animator.SetBool("attack1", true);
                lastTime = Time.time;
 
                while ((Time.time - lastTime) < maxTime && combo < maxCombo)
                {
                    if (Input.GetKeyDown(KeyCode.K) && (Time.time - lastTime) > cooldown)
                    {
                        combo++;
                        //Debug.Log("Attack " + combo);
                animator.SetBool("attack1", false);
                animator.SetBool("attack2", true);
                        lastTime = Time.time;
                    }
                    yield return null;
                }
                combo = 0;
                yield return new WaitForSeconds(cooldown - (Time.time - lastTime));
            } else
            {
                animator.SetBool("attack1", false);
                animator.SetBool("attack2", false);
            }
            yield return null;
        }
    }
    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}