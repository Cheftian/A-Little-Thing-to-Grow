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
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        Flip();
    }

    private void FixedUpdate()
    {
        float currentSpeed = isAiming ? moveSpeed * aimingSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        // Logika flip diubah untuk menggunakan moveDirection
        if ((isFacingRight && moveDirection < 0f) || (!isFacingRight && moveDirection > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
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
}