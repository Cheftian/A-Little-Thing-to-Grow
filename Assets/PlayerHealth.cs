using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Pengaturan Kesehatan")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Efek Saat Terluka")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float invincibilityDuration = 0.5f;

    // --- BAGIAN BARU UNTUK REGENERASI ---
    [Header("Pengaturan Regenerasi")]
    [Tooltip("Aktifkan jika ingin player bisa regenerasi.")]
    [SerializeField] private bool canRegenerate = true;
    [Tooltip("Waktu (detik) setelah diserang sebelum regenerasi dimulai.")]
    [SerializeField] private float delayBeforeRegen = 5f;
    [Tooltip("Jumlah darah yang pulih setiap kali regenerasi.")]
    [SerializeField] private int regenAmountPerTick = 5;
    [Tooltip("Seberapa sering (detik) darah akan pulih.")]
    [SerializeField] private float regenTickSpeed = 0.5f;

    // Komponen & Status
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool canTakeDamage = true;
    private Color originalColor;
    private Coroutine regenerationCoroutine; // Variabel untuk menyimpan proses regenerasi
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        if (!canTakeDamage) return;

        currentHealth -= damage;
        Debug.Log("Player Health: " + currentHealth);

        canTakeDamage = false;
        
        // --- PERUBAHAN DI SINI ---
        // Hentikan proses regenerasi yang mungkin sedang berjalan
        if (regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
        }
        
        // Mulai kembali timer untuk regenerasi setelah diserang
        if (canRegenerate)
        {
            regenerationCoroutine = StartCoroutine(RegenerationRoutine());
        }

        StartCoroutine(DamageEffects(damageSource));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageEffects(Transform damageSource)
    {
        Vector2 knockbackDirection = (transform.position - damageSource.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(invincibilityDuration - flashDuration);
        canTakeDamage = true;
    }

    // --- COROUTINE BARU UNTUK PROSES REGENERASI ---
    private IEnumerator RegenerationRoutine()
    {
        // 1. Tunggu sejenak setelah diserang
        yield return new WaitForSeconds(delayBeforeRegen);

        // 2. Selama darah belum penuh, terus pulihkan
        while (currentHealth < maxHealth)
        {
            currentHealth += regenAmountPerTick;

            // Pastikan darah tidak melebihi batas maksimal
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            Debug.Log("Player Health Regenerated to: " + currentHealth);

            // Tunggu sebelum pemulihan berikutnya
            yield return new WaitForSeconds(regenTickSpeed);
        }

        // Proses regenerasi selesai
        regenerationCoroutine = null;
    }

    private void Die()
    {
        Debug.Log("Player Telah Kalah!");
        SceneManager.LoadScene("MainMenu");
    }
}