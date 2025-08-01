using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damageAmount = 25;
    
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
        // Cek jika mengenai Player
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Beri damage pada Player
            playerHealth.TakeDamage(damageAmount, transform);
        }

        // Langsung hancurkan proyektil setelah mengenai APA PUN
        Destroy(gameObject);
    }
}