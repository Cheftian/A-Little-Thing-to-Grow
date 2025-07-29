using UnityEngine;
using System;

public class PlayerRangedAttack : MonoBehaviour
{

    public static event Action<int> OnAmmoChanged;

    [Header("Referensi Komponen")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private LineRenderer trajectoryLine;

    [Header("Amunisi")]
    [SerializeField] private int maxAmmo = 15;
    [SerializeField] private int startAmmo = 5;
    private int currentAmmo;

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
        currentAmmo = startAmmo;
        OnAmmoChanged?.Invoke(currentAmmo);
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

        // --- LOGIKA PERBAIKAN UNTUK AUTO-FLIP ---

        // 1. Lakukan pengecekan awal HANYA untuk menentukan apakah perlu berbalik.
        Vector2 initialFacingDirection = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
        Vector2 initialPullDirection = mouseWorldPosition - (Vector2)launchPoint.position;
        float initialDotProduct = Vector2.Dot(initialPullDirection, initialFacingDirection);

        if (initialDotProduct > 0 && initialPullDirection.magnitude >= maxForwardPullDistance)
        {
            // Jika kondisi terpenuhi, segera balikkan karakter.
            playerMovement.Flip();
        }

        // 2. SEKARANG, ulangi semua kalkulasi dari awal dengan arah hadap yang sudah diperbarui.
        // Ini memastikan semua logika di bawahnya menggunakan data yang paling akurat.
        Vector2 currentFacingDirection = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
        Vector2 pullDirection = mouseWorldPosition - (Vector2)launchPoint.position;
        float dotProduct = Vector2.Dot(pullDirection, currentFacingDirection);

        // Pilih batas jarak berdasarkan arah tarikan yang sudah benar.
        float currentMaxPull = (dotProduct > 0) ? maxForwardPullDistance : maxPullDistance;

        // "Jepit" posisi mouse secara virtual jika melebihi batas.
        if (pullDirection.magnitude > currentMaxPull)
        {
            mouseWorldPosition = (Vector2)launchPoint.position + pullDirection.normalized * currentMaxPull;
        }

        // Hitung ulang arah dan kekuatan untuk hasil akhir.
        Vector2 launchDirection = (Vector2)launchPoint.position - mouseWorldPosition;
        float distance = Vector2.Distance((Vector2)launchPoint.position, mouseWorldPosition);

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
        // --- CEK AMUNISI SEBELUM MENEMBAK ---
        if (currentAmmo <= 0)
        {
            Debug.Log("Amunisi bawang habis!");
            StopAiming(); // Batalkan aiming jika tidak punya amunisi
            return;
        }

        // Kurangi amunisi
        currentAmmo--;
        // Kirim sinyal ke UI bahwa amunisi berkurang
        OnAmmoChanged?.Invoke(currentAmmo);

        GameObject projectileInstance = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetOwner(transform);
        }

        Rigidbody2D rb = projectileInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(launchForce, ForceMode2D.Impulse);
        }

        StopAiming();
    }
    
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        // Batasi agar tidak melebihi kapasitas maksimal
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
        Debug.Log("Bawang bertambah! Jumlah sekarang: " + currentAmmo);
        // Kirim sinyal ke UI bahwa amunisi bertambah
        OnAmmoChanged?.Invoke(currentAmmo);
    }
}