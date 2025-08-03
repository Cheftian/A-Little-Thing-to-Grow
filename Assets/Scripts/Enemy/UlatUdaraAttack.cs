using UnityEngine;

public class UlatUdaraAttack : MonoBehaviour
{
    [Header("Referensi")]
    private Transform playerTransform;
    [SerializeField] private GameObject projectilePrefab; // Prefab proyektil musuh
    [SerializeField] private Transform launchPoint; // Titik proyektil akan muncul

    [Header("Pengaturan Serangan")]
    [SerializeField] private float detectionRadius = 15f; // Jarak deteksi
    [SerializeField] private float fireRate = 2f; // Tembakan per detik (misal: 0.5 = 1 tembakan tiap 2 detik)
    [SerializeField] private float projectileSpeed = 10f;

    // Variabel Internal
    private float nextFireTime = 0f;
    private bool isFacingRight = true;

    void Start()
    {
        // Cari objek Player menggunakan Tag saat permainan dimulai
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Objek Player dengan tag 'Player' tidak ditemukan di scene!");
        }
    }


    void Update()
    {
        if (playerTransform == null) return;

        // Cek jarak ke player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Jika player dalam jangkauan, serang!
        if (distanceToPlayer <= detectionRadius)
        {
            FacePlayer();

            // Cek apakah sudah waktunya untuk menembak lagi
            if (Time.time >= nextFireTime)
            {
                Shoot();
                // Atur waktu untuk tembakan berikutnya
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    private void FacePlayer()
    {
        // Membalik visual ulat agar selalu menghadap Player
        if ((playerTransform.position.x > transform.position.x && !isFacingRight) || (playerTransform.position.x < transform.position.x && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void Shoot()
    {
        AudioManager.Instance.PlaySFX("ThrowEnemy");

        // Membuat proyektil
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        // Menghitung arah tembakan ke Player
        Vector2 direction = (playerTransform.position - launchPoint.position).normalized;

        // Memberi kecepatan pada proyektil
        rb.linearVelocity = direction * projectileSpeed;
    }

    // Menggambar radius deteksi di editor agar mudah dilihat
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}