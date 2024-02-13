using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skeletonHp : MonoBehaviour
{
    [SerializeField]private int pvActuel;
    [SerializeField] private float blinkSpeed;

    public bool isDead= false;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = new Color(1f, 0.627f, 0.627f);
    private Animator animator;
    private SpriteRenderer spRenderer;
    public List<GameObject> lootage = new List<GameObject>();
    public int swordHit = 10;
    public int bulletHit = 7;
    private CapsuleCollider2D circle;

    // Start is called before the first frame update
    void Start()
    {
        pvActuel = 100;
        animator = GetComponent<Animator>();
        spRenderer = GetComponent<SpriteRenderer>();
        circle = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (pvActuel <= 30 && pvActuel >= 0)
        {
            StartCoroutine(BlinkCoroutine());
        }else if( pvActuel <= 0)
        {
            StopCoroutine(BlinkCoroutine());
            spRenderer.color = startColor;

        }
    }

    public void SwordTakeDamage()
    {
        pvActuel -= swordHit;
        animator.SetTrigger("Hit");
        if (pvActuel <= 0)
        {
            circle.enabled = false;
            Mort();
            StartCoroutine(LootSpawnsCoroutine());
        }

    }

    public void BulletTakeDamage()
    {
        pvActuel -= bulletHit;
        animator.SetTrigger("Hit");
        if (pvActuel <= 0)
        {
            circle.enabled = false;
            Mort();
            StartCoroutine(LootSpawnsCoroutine());
        }

    }

    private void Mort()
    {
        isDead = true;
        
        animator.SetBool("Attack", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("Dead", true);
        animator.ResetTrigger("Hit");
        Destroy(this.gameObject, 2f);
    }

     private IEnumerator BlinkCoroutine()
    {
        float time = 0f;

        while (true)
        {
            float blend = Mathf.PingPong(time * blinkSpeed, 1f);
            Color lerpedColor = Color.Lerp(startColor, endColor, blend);
            spRenderer.color = lerpedColor;
            time += Time.deltaTime;

            yield return null;
        }
    }


    IEnumerator LootSpawnsCoroutine()
    {
        yield return new WaitForSeconds(1f);

        foreach (GameObject loot in lootage)
        {
            Vector3 spawnLoot = transform.position + new Vector3(Random.Range(2f, 2f), Random.Range(2f, 1f), 0f);
            Instantiate(loot, spawnLoot, Quaternion.identity); // Instanciation de l'objet

          
        }
    }
}
