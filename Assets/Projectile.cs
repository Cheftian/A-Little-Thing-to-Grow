using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Hancurkan proyektil setelah beberapa detik agar tidak memenuhi scene
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Hancurkan proyektil saat menyentuh sesuatu
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Di sini Tian bisa menambahkan efek ledakan atau logika damage
        Destroy(gameObject);
    }
}