using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Pengaturan Proyektil")]
    [SerializeField] private int damageAmount = 50;

    [Header("Efek Tambahan")]
    [Tooltip("Prefab yang muncul saat proyektil menyentuh tanah.")]
    [SerializeField] private GameObject groundHitEffectPrefab;
    [Tooltip("Berapa lama efek di tanah akan bertahan.")]
    [SerializeField] private float groundHitDuration = 3f;

    private Transform owner;
    private Rigidbody2D rb;

    public void SetOwner(Transform ownerTransform)
    {
        owner = ownerTransform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f); // Hancurkan setelah 5 detik
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek jika menyentuh tanah
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HandleGroundHit();
        }
        else // Jika menyentuh hal lain (seperti musuh)
        {
            UlatHealth ulat = collision.gameObject.GetComponent<UlatHealth>();
            if (ulat != null)
            {
                ulat.TakeDamage(damageAmount, owner);
            }
            DestroyProjectile();
        }
    }
    
    private void HandleGroundHit()
    {
        if (groundHitEffectPrefab != null)
        {
            GameObject effect = Instantiate(groundHitEffectPrefab, transform.position, Quaternion.identity);
            GroundHitEffect effectScript = effect.GetComponent<GroundHitEffect>();
            if (effectScript != null)
            {
                effectScript.Initialize(damageAmount, groundHitDuration, owner);
            }
        }
        DestroyProjectile();
    }
    
    private void DestroyProjectile()
    {
        // Langsung hancurkan proyektil tanpa perlu melepas 'trail'
        Destroy(gameObject);
    }
}