using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // Sesuai GDD, damage Ulat 2 adalah 25
    [SerializeField] private int damageAmount = 25; 

    void Start()
    {
        // Hancurkan proyektil setelah 5 detik jika tidak mengenai apa pun
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah yang disentuh adalah Player
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Beri damage pada Player
            playerHealth.TakeDamage(damageAmount, transform);
        }

        // Hancurkan proyektil setelah menyentuh apa pun
        Destroy(gameObject);
    }
}