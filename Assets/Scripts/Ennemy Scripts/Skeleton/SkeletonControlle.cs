using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonControlle : MonoBehaviour
{
    public float speed;
    [SerializeField] private float followDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private bool isFollowing = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isFacingRight = false;
    private string playerTag = "Player";
    public Transform player;
    public Animator animator;

    skeletonHp skeletonHp;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        skeletonHp = FindObjectOfType<skeletonHp>();
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
        AttackPlayer();

        if (skeletonHp.isDead)
        {
            transform.Translate(Vector2.zero);
        }

    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followDistance && !isAttacking)
        {
            isFollowing = true;
        }else
        {
            isFollowing = false;
        }

        if (isFollowing && !isAttacking)
        {
            
            Vector2 direction = (player.position - transform.position).normalized;
            direction.y = 0f;
            transform.Translate(direction * speed * Time.deltaTime) ;

            if (direction.x > 0 && isFacingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && !isFacingRight)
            {
                Flip();
            }
        }

        animator.SetBool("isWalking", isFollowing);
    }
    private void AttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            isAttacking = true;
        }else
        {
            isAttacking = false;
        }

        if (isAttacking)
        {
            Vector2 direction = (player.position - transform.position).normalized;
             if (direction.x > 0 && isFacingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && !isFacingRight)
            {
                Flip();
            }

            transform.Translate(Vector2.zero);
            
        }
            animator.SetBool("Attack", isAttacking);

    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}