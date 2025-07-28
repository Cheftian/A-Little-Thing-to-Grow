using UnityEngine;
using System.Collections;

public class UlatDaratBehaviour : MonoBehaviour
{
  // Menambahkan state baru: Idle
    private enum State { Patrol, Chase, Idle }
    private State currentState;

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
            // Pesan error jika Player dengan tag "Player" tidak ditemukan
            Debug.LogError("Objek Player dengan tag 'Player' tidak ditemukan di scene!");
        }

        currentState = State.Patrol; // Mulai dengan patroli
    }

    void Update()
    {
        HandleStateTransitions();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Idle:
                Idle();
                break;
        }

        Flip();
    }

    // --- LOGIKA UTAMA ADA DI SINI ---
    private void HandleStateTransitions()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Jika player terdeteksi
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = State.Chase;
        }
        // Jika player tidak terdeteksi
        else
        {
            // Jika sebelumnya sedang mengejar, sekarang berhenti (Idle)
            if (currentState == State.Chase)
            {
                currentState = State.Idle;
                idleTimer = 0; // Mulai hitung waktu diam
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
        if (transform.position.x < playerTransform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }
        rb.linearVelocity = new Vector2(chaseSpeed * moveDirection, rb.linearVelocity.y);
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