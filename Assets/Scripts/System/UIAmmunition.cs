using UnityEngine;
using TMPro; // Gunakan ini jika Tian memakai TextMeshPro

public class UIAmmunition : MonoBehaviour
{
    private TextMeshProUGUI ammoText;

    void Awake()
    {
        // Ambil komponen Text dari objek ini
        ammoText = GetComponent<TextMeshProUGUI>();
    }

    // Saat UI aktif, ia akan mulai "mendengarkan" sinyal dari Player
    private void OnEnable()
    {
        PlayerRangedAttack.OnAmmoChanged += UpdateAmmoText;
    }

    // Saat UI nonaktif, ia berhenti mendengarkan agar tidak error
    private void OnDisable()
    {
        PlayerRangedAttack.OnAmmoChanged -= UpdateAmmoText;
    }

    // Method ini akan terpanggil secara otomatis setiap kali sinyal OnAmmoChanged aktif
    private void UpdateAmmoText(int amount)
    {
        if (ammoText != null)
        {
            // Tampilkan jumlah amunisi
            ammoText.text = amount.ToString();
        }
    }
}