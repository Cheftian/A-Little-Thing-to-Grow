using UnityEngine;
using UnityEngine.Rendering; // Diperlukan jika menggunakan metode blur canggih
using UnityEngine.Rendering.Universal; // Diperlukan jika menggunakan URP
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    [Tooltip("Panel utama yang berisi tombol-tombol pause.")]
    [SerializeField] private GameObject pausePanel;
    [Tooltip("Panel gelap/blur yang akan menutupi layar di belakang.")]
    // [SerializeField] private GameObject backgroundBlurPanel; // Untuk metode sederhana

    // --- (Opsional) Untuk Metode Blur Canggih ---
    [Header("Efek Blur Profesional (Opsional)")]
    // [SerializeField] private Volume postProcessVolume; 
    // private DepthOfField dof;
    // ---------------------------------------------

    private bool isPaused = false;

    void Start()
    {
        // Pastikan semuanya disembunyikan di awal
        pausePanel.SetActive(false);
        // backgroundBlurPanel.SetActive(false); // Untuk metode sederhana

        // --- (Opsional) Setup untuk Metode Blur Canggih ---
        // if (postProcessVolume != null)
        {
            // postProcessVolume.profile.TryGet(out dof);
        }
        // ---------------------------------------------
    }

    void Update()
    {
        // Dengarkan input tombol Escape
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
        // Menghentikan semua pergerakan dan animasi berbasis waktu
        Time.timeScale = 0f;

        // Tampilkan UI
        pausePanel.SetActive(true);
        // backgroundBlurPanel.SetActive(true); // Untuk metode sederhana

        // --- (Opsional) Aktifkan Blur Canggih ---
        // if (dof != null) { dof.active = true; }
        // -----------------------------------------
    }

    public void ResumeGame()
    {
        isPaused = false;
        // Kembalikan waktu ke kecepatan normal
        Time.timeScale = 1f;

        // Sembunyikan UI
        pausePanel.SetActive(false);
        // backgroundBlurPanel.SetActive(false); // Untuk metode sederhana

        // --- (Opsional) Nonaktifkan Blur Canggih ---
        // if (dof != null) { dof.active = false; }
        // ------------------------------------------
    }

    public void ExitToMainMenu(string sceneName)
    {
        // 1. Kembalikan waktu ke kecepatan normal SEBELUM pindah scene
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        // 2. Muat scene Main Menu
        SceneManager.LoadScene(sceneName);
    }
}