using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Pengaturan Proyektil")]
    [SerializeField] private int damageAmount = 50;

    private Transform owner;
    private Rigidbody2D rb;

    public void SetOwner(Transform ownerTransform)
    {
        owner = ownerTransform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 5f); // Hancurkan setelah 5 detik jika tidak mengenai apa pun
    }

    void Update()
    {
        // Logika rotasi tetap sama
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek jika mengenai ulat
        UlatHealth ulat = collision.gameObject.GetComponent<UlatHealth>();
        if (ulat != null)
        {
            // Beri damage pada ulat
            ulat.TakeDamage(damageAmount, owner);
        }
        
        // Langsung hancurkan proyektil setelah mengenai APA PUN (ulat, tanah, dinding, dll.)
        Destroy(gameObject);
    }
}