using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Kelas kecil ini berfungsi sebagai "paket" data untuk setiap narasi
[System.Serializable]
public class NarrationBlock
{
    [Tooltip("Hanya sebagai pengingat di Inspector, tidak akan ditampilkan di game.")]
    public string description; 
    public Sprite objectiveSprite;
    [TextArea(3, 5)] // Membuat kolom teks lebih besar di Inspector
    public string narrationText;
}

public class NarrationManager : MonoBehaviour
{
    // Singleton agar mudah diakses dari mana saja
    public static NarrationManager Instance;

    [Header("Referensi Komponen UI")]
    [SerializeField] private GameObject narrationPanel; // Panel induk untuk semua elemen
    [SerializeField] private Image objectiveImage;
    [SerializeField] private TextMeshProUGUI narrationText;

    [Header("Pengaturan Tampilan")]
    [SerializeField] private float displayDuration = 7f; // Berapa lama narasi akan tampil

    [Header("Data Narasi & Objektif")]
    [SerializeField] private NarrationBlock[] narrationBlocks;

    private Coroutine displayCoroutine;

    private void Awake()
    {
        // Pengaturan Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Pastikan panel disembunyikan di awal
        narrationPanel.SetActive(false);
    }

    // Fungsi utama yang akan dipanggil oleh skrip lain
    public void ShowNarration(int index)
    {
        // Cek apakah index valid
        if (index < 0 || index >= narrationBlocks.Length)
        {
            Debug.LogWarning("Index narasi tidak valid!");
            return;
        }

        // Hentikan narasi sebelumnya jika masih berjalan
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        // Mulai tampilkan narasi yang baru
        displayCoroutine = StartCoroutine(DisplayRoutine(narrationBlocks[index]));
    }

    private IEnumerator DisplayRoutine(NarrationBlock block)
    {
        // Atur konten UI
        objectiveImage.sprite = block.objectiveSprite;
        narrationText.text = block.narrationText;

        // Tampilkan panel
        narrationPanel.SetActive(true);

        // Tunggu beberapa detik
        yield return new WaitForSeconds(displayDuration);

        // Sembunyikan panel
        narrationPanel.SetActive(false);
    }
}