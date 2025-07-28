using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Pengaturan Proyektil")]
    [Tooltip("Jumlah damage yang diberikan oleh proyektil ini.")]
    [SerializeField] private int damageAmount = 50; // Sesuai GDD, serangan jarak jauh memberi 50 damage

    private Transform owner; // Untuk menyimpan informasi siapa yang menembak (Player)

    // Method publik untuk diatur oleh penembak
    public void SetOwner(Transform ownerTransform)
    {
        owner = ownerTransform;
    }

    // Hancurkan proyektil setelah beberapa detik agar tidak memenuhi scene
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Logika saat proyektil menyentuh sesuatu
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah yang disentuh adalah objek dengan skrip UlatHealth
        UlatHealth ulat = collision.gameObject.GetComponent<UlatHealth>();
        if (ulat != null)
        {
            // Jika iya, panggil fungsi TakeDamage pada ulat tersebut
            // 'owner' (Player) diteruskan sebagai sumber damage untuk knockback
            ulat.TakeDamage(damageAmount, owner);
        }

        // Hancurkan proyektil setelah menyentuh apa pun (ulat, tanah, dll)
        // Di sini Tian bisa menambahkan efek ledakan sebelum hancur
        Destroy(gameObject);
    }
}