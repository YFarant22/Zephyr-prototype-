using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunBullet : MonoBehaviour
{
    public Rigidbody2D bulletRb2d;
    skeletonHp skeletonHp;

    // Start is called before the first frame update
    void Start()
    {
        bulletRb2d = GetComponent<Rigidbody2D>();
        skeletonHp = FindObjectOfType<skeletonHp>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            skeletonHp.BulletTakeDamage();
        }
        DestroyingBullet();
    }

    private void DestroyingBullet()
    {
        bulletRb2d.velocity = Vector2.zero;
        Destroy(gameObject, 0.10f);
    }
}
