using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    // ... (Semua variabel lama Anda tetap di sini) ...
    private Rigidbody2D rb;
    private float moveDirection;
    public bool isFacingRight = true;
    private bool isAiming = false;
    [SerializeField] private float aimingSpeedMultiplier = 0.3f;
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
    [SerializeField] private int stompDamage = 100;
    [SerializeField] private float stompBounceForce = 14f;
    private bool isGrounded;
    [Header("Deteksi Tembok")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    [SerializeField] private LayerMask wallLayer;
    private bool isTouchingWall;
    private PlayerHealth playerHealth;
    private GameObject currentOneWayPlatform;
    private float distanceMoved = 0f;
    private Vector3 lastPosition;
    private float idleTimerOnLeaf = 0f;
    private bool cameraGuideShown = false;

    // --- VARIABEL BARU UNTUK SUARA LANGKAH ---
    [Header("Pengaturan Suara Langkah")]
    [Tooltip("Seberapa cepat suara langkah diputar (detik).")]
    [SerializeField] private float footstepRate = 0.4f;
    private float footstepTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        extraJumps = extraJumpsValue;
        lastPosition = transform.position;
    }

    void Update()
    {
        // ... (Semua logika Update lama Anda tetap di sini) ...
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * transform.localScale.x, wallCheckDistance, wallLayer);
        if (Input.GetKey(KeyCode.D)) { moveDirection = 1f; }
        else if (Input.GetKey(KeyCode.A)) { moveDirection = -1f; }
        else { moveDirection = 0f; }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && !isAiming && !Input.GetKey(KeyCode.S))
        {
            if (isGrounded || extraJumps > 0)
            {
                AudioManager.Instance.PlaySFX("Jump");

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (!isGrounded) extraJumps--;
            }
        }
        CheckForStomp();
        CheckDropDown();
        distanceMoved += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        if (distanceMoved > 10f && !cameraGuideShown)
        {
            if(GuideManager.Instance != null) GuideManager.Instance.ShowTimedGuide(GuideType.Camera);
            cameraGuideShown = true;
        }
        if ((isFacingRight && moveDirection < 0f) || (!isFacingRight && moveDirection > 0f))
        {
            Flip();
        }

        // --- PANGGIL FUNGSI BARU DI SINI ---
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        // ... (Fungsi FixedUpdate Anda tidak berubah) ...
        float currentSpeed = isAiming ? moveSpeed * aimingSpeedMultiplier : moveSpeed;
        float xVelocity = moveDirection * currentSpeed;
        if ((moveDirection > 0 && isTouchingWall) || (moveDirection < 0 && isTouchingWall))
            xVelocity = 0f;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
    }
    
    // --- FUNGSI BARU UNTUK MENGATUR SUARA LANGKAH ---
    private void HandleFootsteps()
    {
        // Cek jika karakter sedang di darat DAN sedang bergerak
        if (isGrounded && moveDirection != 0)
        {
            // Kurangi timer setiap frame
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0)
            {
                // Reset timer
                footstepTimer = footstepRate;

                // Deteksi ground di bawah kaki
                Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
                
                if (groundCollider != null)
                {
                    // Jika menginjak daun (platform satu arah)
                    if (groundCollider.CompareTag("OneWayPlatform"))
                    {
                        AudioManager.Instance.PlaySFX("RunDaun");
                    }
                    // Jika menginjak tanah biasa (tag lain atau tidak ada tag)
                    else
                    {
                        AudioManager.Instance.PlaySFX("RunGround");
                    }
                }
            }
        }
        else
        {
            // Reset timer jika berhenti atau di udara
            footstepTimer = 0;
        }
    }


    // ... (Semua fungsi lama lainnya seperti Flip, OnDrawGizmos, dll. tetap sama) ...
    public void Flip()
    {
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
                if (rb.linearVelocity.y < -0.1f)
                {
                    switch (ulat.ulatType)
                    {
                        case UlatType.Normal:
                            ulat.TakeDamage(stompDamage, transform);
                            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
                            break;
                        case UlatType.Berduri:
                            playerHealth.TakeDamage(stompDamage, ulat.transform);
                            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
                            break;
                    }
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