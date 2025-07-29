using UnityEngine;
using UnityEngine.UI;

public class UIInteractionBar : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void OnEnable()
    {
        // Mulai mendengarkan sinyal dari Player
        PlayerInteraction.OnInteractionProgress += UpdateProgress;
        PlayerInteraction.OnInteractionStateChanged += SetBarVisibility;
    }

    private void OnDisable()
    {
        // Berhenti mendengarkan untuk menghindari error
        PlayerInteraction.OnInteractionProgress -= UpdateProgress;
        PlayerInteraction.OnInteractionStateChanged -= SetBarVisibility;
    }

    void Start()
    {
        // Pastikan bar disembunyikan di awal permainan
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }
    
    // Method untuk menampilkan atau menyembunyikan bar
    void SetBarVisibility(bool isVisible)
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(isVisible);
        }
    }

    // Method untuk memperbarui nilai progress bar
    void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
    }
}