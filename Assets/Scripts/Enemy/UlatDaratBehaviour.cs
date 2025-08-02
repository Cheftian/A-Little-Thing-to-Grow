using UnityEngine;
using System.Collections;

public class UlatDaratBehaviour : MonoBehaviour
{
  // Menambahkan state baru: Idle
    private enum State { Patrol, Chase, Idle }
    private State currentState;
    [SerializeField] private WormType wormType;
    private enum WormType { Hijau, Coklat, Biru }

    [Header("Referensi")]
    private Transform playerTransform;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Pengaturan Perilaku")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRadius = 8f;
    [Tooltip("Berapa lama ulat diam sebelum kembali patroli.")]
    [SerializeField] private float idleDuration = 2f; // Waktu untuk diam
    private float currentSpeed = 0f; // Tambahkan di atas bersama variabel lain
    [SerializeField] private float smoothTime = 0.2f; // Tambahkan ini untuk pengaturan kehalusan
    private float velocityXSmoothing = 0f;

    // Komponen & Variabel Internal
    private Rigidbody2D rb;
    private int moveDirection = 1;
    private bool isFacingRight = true;
    private float idleTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Objek Player dengan tag 'Player' tidak ditemukan di scene!");
        }

        currentState = (wormType == WormType.Biru) ? State.Idle : State.Patrol;
    }

    void Update()
    {
    if (wormType != WormType.Biru)
        HandleStateTransitions();

    switch (currentState)
    {
        case State.Patrol:
            if (wormType != WormType.Biru)
                Patrol();
            break;
        case State.Chase:
            if (wormType != WormType.Biru)
                Chase();
            break;
        case State.Idle:
            if (wormType == WormType.Biru)
                Idle(); // hanya ulat biru yang idle
            break;
    }

    if (wormType != WormType.Biru)
        Flip();
    }

    // --- LOGIKA UTAMA ADA DI SINI ---
    private void HandleStateTransitions()
    {
        if (playerTransform == null || wormType == WormType.Biru) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            currentState = State.Chase;
        }
        else
        {
            if (currentState == State.Chase)
            {
                // Player kabur, kembali ke patroli
                currentState = State.Patrol;
            }
        }
    }

    private void Patrol()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (groundInfo.collider == false)
        {
            TurnAround();
        }
        rb.linearVelocity = new Vector2(patrolSpeed * moveDirection, rb.linearVelocity.y);
    }

    private void Chase()
    {
        // === CEK APAKAH MASIH ADA TANAH DI DEPAN ===
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (!groundInfo.collider)
        {
            // Kalau tanah habis, berhenti mengejar dan balik
            TurnAround();
            currentState = State.Patrol;
            return;
        }

        float deltaX = playerTransform.position.x - transform.position.x;
        float deadZoneThreshold = 0.2f;

        // Jika terlalu dekat, berhenti
        if (Mathf.Abs(deltaX) < deadZoneThreshold)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, 0, ref velocityXSmoothing, smoothTime);
        }
        else
        {
            float targetSpeed = chaseSpeed * Mathf.Sign(deltaX); // Tentukan arah dengan halus
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocityXSmoothing, smoothTime);

            moveDirection = deltaX > 0 ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private void Idle()
    {
        // 1. Berhenti total
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // 2. Hitung waktu
        idleTimer += Time.deltaTime;

        // 3. Jika sudah cukup lama diam, kembali patroli
        if (idleTimer >= idleDuration)
        {
            currentState = State.Patrol;
        }
    }

    private void TurnAround()
    {
        moveDirection *= -1;
    }

    private void Flip()
    {
        if ((moveDirection > 0 && !isFacingRight) || (moveDirection < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}