using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damageAmount = 25;

    [Header("Efek Tambahan")]
    [SerializeField] private GameObject groundHitEffectPrefab;
    [SerializeField] private float groundHitDuration = 3f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HandleGroundHit();
        }
        else
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, transform);
            }
            DestroyProjectile();
        }
    }

    private void HandleGroundHit()
    {
        if (groundHitEffectPrefab != null)
        {
            GameObject effect = Instantiate(groundHitEffectPrefab, transform.position, Quaternion.identity);
            GroundHitEffectEnemy effectScript = effect.GetComponent<GroundHitEffectEnemy>();
            if(effectScript != null)
            {
                effectScript.Initialize(damageAmount, groundHitDuration);
            }
        }
        DestroyProjectile();
    }
    
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}