using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttackHit : MonoBehaviour
{
    skeletonHp skeletonHp;

    void Start()
    {
        skeletonHp = FindObjectOfType<skeletonHp>();
    }


    void OnTriggerEnter2D(Collider2D ennemies)
    {
        if (ennemies.CompareTag("Enemy"))
        {
            skeletonHp = FindObjectOfType<skeletonHp>();
            skeletonHp.SwordTakeDamage();
        }
    }
}
