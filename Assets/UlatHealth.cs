using UnityEngine;
using System.Collections;

public enum UlatType { Normal, Berduri }

public class UlatHealth : MonoBehaviour
{


    [Header("Tipe Ulat")]
    public UlatType ulatType = UlatType.Normal; // Atur tipe defaultnya


    [Header("Pengaturan Kesehatan")]
    [SerializeField] private int maxHealth = 100; // Sesuai GDD, Ulat 1 memiliki 100 HP
    private int currentHealth;

    [Header("Efek Saat Terluka")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float knockbackForce = 7f;

    [Header("Drop Item Saat Kalah")]
    [SerializeField] private GameObject itemDropPrefab; // Prefab Bawang
    [Tooltip("Peluang (dalam persen) item akan drop.")]
    [Range(0, 100)]
    [SerializeField] private int dropChance = 25; // Sesuai GDD, drop rate Bawang 25%


    // Komponen
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        // Hentikan gerakan ulat sejenak saat terluka
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Efek terpental
        Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        // Efek kilat
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashEffect()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Seekor ulat telah dikalahkan!");

        // Logika Drop Item
        if (itemDropPrefab != null)
        {
            if (Random.Range(0, 100) < dropChance)
            {
                Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
            }
        }
        
        // Hancurkan objek ulat
        Destroy(gameObject);
    }
}