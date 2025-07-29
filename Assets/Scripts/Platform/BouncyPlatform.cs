using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{
    [Header("Pengaturan Efek")]
    [Tooltip("Seberapa jauh platform akan turun saat diinjak.")]
    [SerializeField] private float downwardDistance = 0.2f;
    [Tooltip("Seberapa cepat platform bergerak turun dan kembali ke atas.")]
    [SerializeField] private float moveSpeed = 3f;

    // Variabel untuk menyimpan posisi
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // Menyimpan posisi asli platform saat permainan dimulai
        originalPosition = transform.position;
        targetPosition = originalPosition;
    }

    void Update()
    {
        // Setiap frame, gerakkan platform secara mulus menuju posisi targetnya
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Fungsi ini terpanggil saat Player pertama kali menyentuh/menginjak platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah yang menyentuh adalah Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Atur posisi target menjadi sedikit di bawah posisi asli
            targetPosition = originalPosition - new Vector3(0, downwardDistance, 0);
        }
    }

    // Fungsi ini terpanggil saat Player meninggalkan platform
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Cek apakah yang pergi adalah Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Atur posisi target kembali ke posisi semula
            targetPosition = originalPosition;
        }
    }
}