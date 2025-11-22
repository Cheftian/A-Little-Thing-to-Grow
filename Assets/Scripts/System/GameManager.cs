using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    // Singleton agar mudah diakses dari mana saja
    public static GameManager Instance;

    [Header("Referensi UI")]
    [Tooltip("Panel yang akan muncul saat Game Over.")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject inGameHudPanel;

    [Header("Efek Blur (URP)")]
    [Tooltip("Seret objek 'Global Volume' yang ada di scene ke sini.")]
    [SerializeField] private Volume postProcessVolume;

    private DepthOfField depthOfField;
    private bool isGameOver = false;

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
        gameOverPanel.SetActive(false);

        if (inGameHudPanel != null)
        {
            inGameHudPanel.SetActive(true);
        }

        // Ambil efek Depth of Field dari profile
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out depthOfField);
            if (depthOfField != null)
            {
                depthOfField.active = false;
            }
        }
    }

    // Fungsi utama yang akan dipanggil saat Player kalah
    public void TriggerGameOver()
    {
        if (isGameOver) return; // Hentikan jika sudah game over

        isGameOver = true;
        Debug.Log("GAME OVER");

        if (inGameHudPanel != null)
        {
            inGameHudPanel.SetActive(false);
        }

        // Hentikan waktu permainan
        Time.timeScale = 0f;

        // Tampilkan panel Game Over
        gameOverPanel.SetActive(true);

        // Aktifkan efek blur
        if (depthOfField != null)
        {
            depthOfField.active = true;
        }
    }

    // --- Fungsi untuk Tombol di Panel Game Over ---

    public void RestartScene()
    {
        // Kembalikan waktu ke normal sebelum memuat ulang scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu(string sceneName)
    {
        // Kembalikan waktu ke normal sebelum kembali ke menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}