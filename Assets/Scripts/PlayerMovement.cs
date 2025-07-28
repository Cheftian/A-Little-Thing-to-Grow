using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Komponen & Variabel Dasar
    private Rigidbody2D rb;
    private float moveDirection; // Menggantikan horizontalInput
    public bool isFacingRight = true;

    private bool isAiming = false;
    [SerializeField] private float aimingSpeedMultiplier = 0.3f; // Seberapa lambat saat membidik

    [Header("Pengaturan Gerakan")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Pengaturan Lompatan")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private int extraJumpsValue = 1;
    private int extraJumps;

    [Header("Pengecekan Tanah")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Pengaturan Menginjak (Stomp)")]
    [SerializeField] private int stompDamage = 100; // Damage saat menginjak
    [SerializeField] private float stompBounceForce = 14f; // Kekuatan pantulan setelah menginjak
    private bool isGrounded;

    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        // --- INPUT SPESIFIK A/D ---
        // Pengecekan input diubah untuk hanya mendeteksi tombol A dan D
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection = 1f; // Bergerak ke kanan
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = -1f; // Bergerak ke kiri
        }
        else
        {
            moveDirection = 0f; // Diam jika tidak ada tombol yang ditekan
        }

        // --- LOGIKA LOMPAT & FLIP (SAMA SEPERTI SEBELUMNYA) ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && !isAiming)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
            }
        }

        CheckForStomp();

        // Kondisi if yang sebelumnya ada di dalam Flip(), sekarang diletakkan di sini
        if ((isFacingRight && moveDirection < 0f) || (!isFacingRight && moveDirection > 0f))
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = isAiming ? moveSpeed * aimingSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);
    }
    public void Flip()
    {
        // Fungsi ini sekarang murni hanya untuk membalikkan badan
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void SetAimingState(bool aiming)
    {
        isAiming = aiming;
    }
    
    private void CheckForStomp()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);

        foreach (var col in colliders)
        {
            UlatHealth ulat = col.GetComponent<UlatHealth>();
            if (ulat != null)
            {
                if (rb.linearVelocity.y < -0.1f) // Pastikan player sedang bergerak ke bawah
                {
                    // --- LOGIKA BARU DENGAN SWITCH ---
                    switch (ulat.ulatType)
                    {
                        // KASUS 1: Jika menginjak ulat normal
                        case UlatType.Normal:
                            ulat.TakeDamage(stompDamage, transform); // Ulat yang kena damage
                            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce); // Player memantul
                            break;

                        // KASUS 2: Jika menginjak ulat berduri
                        case UlatType.Berduri:
                            playerHealth.TakeDamage(stompDamage, ulat.transform); // Justru PLAYER yang kena damage
                            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce); // Player tetap memantul ke atas
                            break;
                    }
                    
                    // Hentikan loop agar tidak memproses lebih dari satu ulat per frame
                    return; 
                }
            }
        }
    }
}