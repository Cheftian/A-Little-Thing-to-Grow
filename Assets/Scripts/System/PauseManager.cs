using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    [Tooltip("Panel utama yang berisi tombol-tombol pause.")]
    [SerializeField] private GameObject pausePanel;
    [Tooltip("Panel HUD yang aktif saat bermain (berisi jumlah bawang, keranjang, dll.).")]
    [SerializeField] private GameObject inGameHudPanel; // <-- VARIABEL BARU DI SINI

    [Header("Efek Blur (URP)")]
    [Tooltip("Seret objek 'Global Volume' yang ada di scene ke sini.")]
    [SerializeField] private Volume postProcessVolume;

    private DepthOfField depthOfField;
    private bool isPaused = false;

    void Start()
    {
        NarrationManager.Instance.ShowNarration(0);
        // Pastikan panel pause nonaktif dan HUD game aktif di awal
        pausePanel.SetActive(false);
        if (inGameHudPanel != null)
        {
            inGameHudPanel.SetActive(true);
        }

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out depthOfField);
            if (depthOfField != null)
            {
                depthOfField.active = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        
        // --- SEMBUNYIKAN HUD SAAT PAUSE ---
        if (inGameHudPanel != null)
        {
            inGameHudPanel.SetActive(false);
        }

        // Aktifkan efek blur
        if (depthOfField != null)
        {
            depthOfField.active = true;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        // --- TAMPILKAN KEMBALI HUD SAAT RESUME ---
        if (inGameHudPanel != null)
        {
            inGameHudPanel.SetActive(true);
        }

        // Nonaktifkan efek blur
        if (depthOfField != null)
        {
            depthOfField.active = false;
        }
    }

    public void ExitToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}