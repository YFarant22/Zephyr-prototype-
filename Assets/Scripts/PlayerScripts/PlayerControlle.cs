using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class PlayerControlle : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float horizontalMovement;
    [SerializeField] private float speed = 8f;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private bool isMoving = false;
    public PlayerInput playerInput;
    [SerializeField] private UnityEngine.Vector2 move;

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

    [Header("Saut")]
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float groundRadius;
    [SerializeField] private bool hasUsedDoubleJump = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isGrounded = false;

    [Header("Glissade & Saut Mural")]
    [SerializeField] private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private float maxXJumpPower;
    [SerializeField] private UnityEngine.Vector2 wallJumpingPower;    
    [SerializeField] private float wallRadius;
    
    

    [Header("Composents")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        pistolShot = GetComponent<AudioSource>();
        //Starts the looping coroutine
        StartCoroutine(SwordAttackCoroutine());
    }

    private void Update()
    {
        
        Flip();
        // Mise à jour des Fonctions
            PlayerJump();
                animator.SetFloat("yAxis", rb.velocity.y);
                WallJump();
                    GunShot();
                        

            // Détecter le sol
            if (isGrounded)
            {
                isJumping = false;
                isWallSliding = false;
            } else
            {
                isJumping = true;
            }


                // Vérification si le joueur détecte un mur
                if (WallCheck() && rb.velocity.y < 0)
                {
                    isWallSliding = true;
                    rb.velocity = new UnityEngine.Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isFalling", true);
                } else
                {
                    isWallSliding= false;
                    animator.SetBool("isFalling", false);
                } 

                    

    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        PlayerMove();

        // Appliquer la vélocité pour le mouvement horizontal seulement
        rb.velocity = new UnityEngine.Vector2(move.x, rb.velocity.y);
    }


    // Fonction de déplacement
    private void PlayerMove()
    {
        if (!isShooting)
        {
        horizontalMovement = Input.GetAxis("Horizontal") * speed;
        move = new UnityEngine.Vector2(horizontalMovement, rb.velocity.y);

        if (horizontalMovement != 0)
        {
            isMoving = true;
            animator.SetBool("isRunning", true);
            animator.SetBool("gunShot", false);

        } else
        {
            animator.SetBool("isRunning", false); 
            isMoving = false; 
        }
            
        } else
        {
            move = new UnityEngine.Vector2(0f, 0f);
            isMoving = false;
            animator.SetBool("isRunning", false);
        }
    }

    // Fonction de saut
    private void PlayerJump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isShooting = false;
            if (isGrounded)
            {
                Jump(jumpingPower);
                hasUsedDoubleJump = false;
            }else if (WallCheck() && rb.velocity.y < 0)
            {
                isWallSliding = true;
            } 
            
            
            else if (!hasUsedDoubleJump)
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

    // Fonction de détection du sol
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

     private bool WallCheck()
     {
        return Physics2D.OverlapCircle(wallCheck.position, wallRadius, wallLayer);
     }

     // Sytème de saut Mural
    private void WallJump()
    {
        if (WallCheck() && !isGrounded)
    {
        isWallSliding = true;
        isWallJumping = false;
        hasUsedDoubleJump = false;
        wallJumpingDirection = -transform.localScale.x;
        wallJumpingCounter = wallJumpingTime;

        // Ajout de la condition pour éviter de répéter l'invocation
        if (!IsInvoking(nameof(StopWallJumping)))
        {
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    else
    {
        isWallSliding = false;
        CancelInvoke(nameof(StopWallJumping));
        wallJumpingCounter -= Time.deltaTime;
    }

    if ((Input.GetKeyDown(KeyCode.Space) || playerInput.actions["Jump"].triggered) && wallJumpingCounter > 0f && isWallSliding)
    {
        isWallJumping = true;
        float xJumpPower = wallJumpingDirection * wallJumpingPower.x;
        float yJumpPower = wallJumpingPower.y;
        xJumpPower = Mathf.Clamp(xJumpPower, -maxXJumpPower, maxXJumpPower);

        // Utiliser AddForce pour le saut
        rb.AddForce(new UnityEngine.Vector2(0f, yJumpPower), ForceMode2D.Impulse);
        rb.AddForce(new UnityEngine.Vector2(xJumpPower, 0f), ForceMode2D.Impulse);

        Debug.Log("x Saut : " + xJumpPower);
        wallJumpingCounter = 0f;

        if (transform.localScale.x != wallJumpingDirection)
        {
            isFacingRight = !isFacingRight;
            UnityEngine.Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    }


    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    // Système de tire
     void GunShot()
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
    public void FxShoot()
    {
        pistolShot.Play();
    }
    // Instanciation de l'objet Bullet
    public void gunShooting()
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

    // Fonction de Direction
   private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            UnityEngine.Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }






    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCheck.position, wallRadius);
    }

}