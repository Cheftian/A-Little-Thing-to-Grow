using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private GameObject currentOneWayPlatform;
    private float distanceMoved = 0f;
    private Vector3 lastPosition;
    private float idleTimerOnLeaf = 0f;
    private bool cameraGuideShown = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        extraJumps = extraJumpsValue;
        
        lastPosition = transform.position;
    }

    void Update()
    {
        // Input Gerakan
        if (Input.GetKey(KeyCode.D)) { moveDirection = 1f; }
        else if (Input.GetKey(KeyCode.A)) { moveDirection = -1f; }
        else { moveDirection = 0f; }

        // Cek Pijakan
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        // Logika Lompat
        // Tambahkan "!Input.GetKey(KeyCode.S)" di akhir kondisi
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && !isAiming && !Input.GetKey(KeyCode.S))
        {
            if (isGrounded || extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (!isGrounded) extraJumps--;
            }
        }
        
        CheckForStomp();
        CheckDropDown();

        // Cek perpindahan untuk guide kamera
        distanceMoved += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distanceMoved > 10f && !cameraGuideShown)
        {
            if(GuideManager.Instance != null) GuideManager.Instance.ShowTimedGuide(GuideType.Camera);
            cameraGuideShown = true;
        }
        
        // Membalik karakter berdasarkan input gerakan
        if ((isFacingRight && moveDirection < 0f) || (!isFacingRight && moveDirection > 0f))
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = isAiming ? moveSpeed * aimingSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);
        CheckDropDownGuide();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }
    
    private void CheckDropDown()
    {
        if (Input.GetKey(KeyCode.S) && Input.GetButtonDown("Jump"))
        {
            StartCoroutine(DisableCollision());
        }
    }

    private IEnumerator DisableCollision()
    {
        if (currentOneWayPlatform != null)
        {
            Collider2D platformCollider = currentOneWayPlatform.GetComponent<Collider2D>();
            Collider2D playerCollider = GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(playerCollider, platformCollider);
            yield return new WaitForSeconds(0.25f);
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        }
    }

    private void CheckDropDownGuide()
    {
        if (currentOneWayPlatform != null && rb.linearVelocity.magnitude < 0.1f)
        {
            idleTimerOnLeaf += Time.fixedDeltaTime;
            if (idleTimerOnLeaf > 2f)
            {
                if (GuideManager.Instance != null) GuideManager.Instance.ShowSituationalGuide(GuideType.DropDown);
            }
        }
        else
        {
            idleTimerOnLeaf = 0f;
            if (GuideManager.Instance != null) GuideManager.Instance.HideSituationalGuide(GuideType.DropDown);
        }
    }
}