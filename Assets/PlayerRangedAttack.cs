using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour
{
    [Header("Referensi Komponen")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private LineRenderer trajectoryLine;

    [Header("Pengaturan Tembakan")]
    [SerializeField] private float maxLaunchForce = 20f;
    [SerializeField] private float maxPullDistance = 4f; // Batas maksimal tarikan ke belakang
    [SerializeField] private float maxForwardPullDistance = 1.5f; // Batas baru untuk tarikan ke depan

    [Header("Pengaturan Garis Lintasan")]
    [SerializeField] private int linePoints = 50;
    [SerializeField] private float timeBetweenPoints = 0.1f;

    // Komponen lain yang dibutuhkan
    private Camera mainCamera;
    private PlayerMovement playerMovement;

    // Status
    private bool isAiming = false;
    private Vector2 launchForce;

    void Awake()
    {
        mainCamera = Camera.main;
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        trajectoryLine.positionCount = 0;
    }

    void Update()
    {
        HandleInput();
        if (isAiming)
        {
            Aim();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1)) { StartAiming(); }
        if (Input.GetMouseButtonUp(1)) { StopAiming(); }
        if (isAiming && Input.GetMouseButtonDown(0)) { Fire(); }
    }

    private void StartAiming()
    {
        isAiming = true;
        playerMovement.SetAimingState(true);
        trajectoryLine.positionCount = linePoints;
    }

    private void StopAiming()
    {
        isAiming = false;
        playerMovement.SetAimingState(false);
        trajectoryLine.positionCount = 0;
    }

    // --- PERUBAHAN UTAMA ADA DI SINI ---
    private void Aim()
    {
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Menentukan arah hadap karakter sebagai vektor
        Vector2 playerFacingDirection = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
        
        // Vektor dari titik tembak ke posisi mouse (arah tarikan)
        Vector2 pullDirection = mouseWorldPosition - (Vector2)launchPoint.position;
        
        // Menentukan apakah tarikan ke arah depan atau belakang menggunakan Dot Product
        // Jika hasilnya > 0, berarti mouse berada di depan karakter
        float dotProduct = Vector2.Dot(pullDirection, playerFacingDirection);
        
        // Pilih batas jarak berdasarkan arah tarikan
        float currentMaxPull = (dotProduct > 0) ? maxForwardPullDistance : maxPullDistance;
        
        // Jika jarak tarikan melebihi batas, "jepit" posisi mouse secara virtual
        if (pullDirection.magnitude > currentMaxPull)
        {
            mouseWorldPosition = (Vector2)launchPoint.position + pullDirection.normalized * currentMaxPull;
        }

        // Hitung ulang arah dan kekuatan berdasarkan posisi mouse yang sudah disesuaikan
        Vector2 launchDirection = (Vector2)launchPoint.position - mouseWorldPosition;
        float distance = Vector2.Distance((Vector2)launchPoint.position, mouseWorldPosition);
        
        // Skala kekuatan tetap dibagi dengan jarak maksimal agar tarikan ke depan tidak bisa sekuat tarikan ke belakang
        float forceMagnitude = (distance / maxPullDistance) * maxLaunchForce;
        launchForce = launchDirection.normalized * forceMagnitude;

        DrawTrajectory(launchForce);
    }

    private void DrawTrajectory(Vector2 force)
    {
        for (int i = 0; i < linePoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector2 pointPosition = (Vector2)launchPoint.position + (force * t) + (0.5f * Physics2D.gravity * (t * t));
            trajectoryLine.SetPosition(i, pointPosition);
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(launchForce, ForceMode2D.Impulse);
        StopAiming();
    }
}