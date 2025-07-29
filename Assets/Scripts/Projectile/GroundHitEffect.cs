using UnityEngine;
using System.Collections.Generic;

public class GroundHitEffect : MonoBehaviour
{
    private int damageAmount;
    private Transform owner;
    private List<Collider2D> alreadyHit = new List<Collider2D>(); // Daftar musuh yang sudah kena

    public void Initialize(int damage, float duration, Transform ownerTransform)
    {
        damageAmount = damage;
        owner = ownerTransform;
        Destroy(gameObject, duration); // Hancurkan efek ini setelah durasi tertentu
    }

    // Gunakan trigger agar musuh bisa lewat dan kena damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika musuh sudah pernah kena damage dari efek ini
        if (alreadyHit.Contains(other))
        {
            return;
        }

        UlatHealth ulat = other.GetComponent<UlatHealth>();
        if (ulat != null)
        {
            ulat.TakeDamage(damageAmount, owner);
            alreadyHit.Add(other); // Tandai musuh ini sudah kena
        }
    }
}