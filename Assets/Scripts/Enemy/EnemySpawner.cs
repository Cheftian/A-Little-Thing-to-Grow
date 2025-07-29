using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pengaturan Spawner")]
    [Tooltip("Daftar prefab musuh yang bisa di-spawn di platform ini.")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [Tooltip("Jumlah maksimal musuh yang bisa ada di platform ini pada satu waktu.")]
    [SerializeField] private int maxEnemiesOnPlatform = 3;
    [Tooltip("Waktu jeda (detik) antara setiap spawn.")]
    [SerializeField] private float spawnInterval = 5f;

    [Header("Area Spawn")]
    [Tooltip("Jarak spawn di atas permukaan platform (untuk ulat darat).")]
    [SerializeField] private float spawnHeightOffset = 1f;
    [Tooltip("Jarak spawn di samping platform (untuk ulat udara).")]
    [SerializeField] private float spawnSideOffset = 0.5f; // Variabel baru

    // Variabel internal untuk melacak musuh
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Collider2D spawnAreaCollider;
    private bool isSpawning = false;

    void Start()
    {
        spawnAreaCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (spawnedEnemies.Count < maxEnemiesOnPlatform && !isSpawning)
        {
            StartCoroutine(SpawnEnemyRoutine());
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        isSpawning = true;
        while (spawnedEnemies.Count < maxEnemiesOnPlatform)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (spawnedEnemies.Count < maxEnemiesOnPlatform)
            {
                SpawnSingleEnemy();
            }
        }
        isSpawning = false;
    }
    
    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector2 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        if (enemyToSpawn.GetComponent<UlatUdaraAttack>() != null)
        {
            // --- LOGIKA KHUSUS UNTUK ULAT TIPE 2 (YANG DIPERBARUI) ---
            float spawnX = spawnAreaCollider.bounds.max.x + spawnSideOffset;
            // Pilih posisi Y acak di sepanjang sisi kanan collider platform
            float spawnY = Random.Range(spawnAreaCollider.bounds.min.y, spawnAreaCollider.bounds.max.y);
            spawnPosition = new Vector2(spawnX, spawnY);
            
            // Rotasi diubah menjadi -90 agar ulat menghadap ke kiri (ke arah area permainan)
            spawnRotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            // --- LOGIKA NORMAL UNTUK ULAT LAIN (Tipe 1 & 3) ---
            float spawnX = Random.Range(spawnAreaCollider.bounds.min.x, spawnAreaCollider.bounds.max.x);
            float spawnY = spawnAreaCollider.bounds.max.y + spawnHeightOffset;
            spawnPosition = new Vector2(spawnX, spawnY);
            spawnRotation = Quaternion.identity;
        }
        
        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, spawnRotation);
        spawnedEnemies.Add(newEnemy);
    }
}