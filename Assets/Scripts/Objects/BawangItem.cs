using UnityEngine;

public class BawangItem : MonoBehaviour
{
    [Tooltip("Jumlah amunisi yang didapat dari item ini.")]
    [SerializeField] private int ammoValue = 1;
    [Tooltip("Berapa lama (detik) item ini akan ada sebelum hilang.")]
    [SerializeField] private float lifespan = 8f;

    void Start()
    {
        // Hancurkan item ini secara otomatis setelah beberapa detik
        Destroy(gameObject, lifespan);
    }

    // Gunakan trigger agar player bisa melewatinya untuk mengambil
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika yang menyentuh adalah Player
        if (other.CompareTag("Player"))
        {
            // Cari komponen PlayerRangedAttack pada Player
            PlayerRangedAttack playerAttack = other.GetComponent<PlayerRangedAttack>();
            if (playerAttack != null)
            {
                // Panggil fungsi untuk menambah amunisi
                playerAttack.AddAmmo(ammoValue);
                // Hancurkan item bawang ini karena sudah diambil
                Destroy(gameObject);
            }
        }
    }
}