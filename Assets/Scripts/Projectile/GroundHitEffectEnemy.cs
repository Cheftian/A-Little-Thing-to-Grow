using UnityEngine;

public class GroundHitEffectEnemy : MonoBehaviour
{
    private int damageAmount;
    private bool playerHit = false; // Cukup cek 1x untuk player

    public void Initialize(int damage, float duration)
    {
        damageAmount = damage;
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerHit) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount, transform);
            playerHit = true; // Tandai player sudah kena
        }
    }
}