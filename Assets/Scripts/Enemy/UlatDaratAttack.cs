using UnityEngine;

public class UlatDaratAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 25; // 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cek apakah yang disentuh adalah objek dengan skrip PlayerHealth
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Panggil fungsi TakeDamage pada Player
            // 'transform' di sini adalah posisi si ulat sebagai sumber serangan
            playerHealth.TakeDamage(damageAmount, transform);
        }
    }
}