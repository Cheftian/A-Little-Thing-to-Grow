using UnityEngine;

public class NarrationTrigger : MonoBehaviour
{
    [Tooltip("Index narasi dari array di NarrationManager yang ingin dipicu.")]
    [SerializeField] private int narrationIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika yang masuk adalah Player
        if (other.CompareTag("Player"))
        {
            // Panggil NarrationManager untuk menampilkan narasi
            NarrationManager.Instance.ShowNarration(narrationIndex);
            
            // Nonaktifkan trigger ini agar tidak terpanggil berulang kali
            gameObject.SetActive(false);
        }
    }
}