using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// Definisikan tipe item di luar kelas agar bisa diakses skrip lain
public enum HeldItemType { None, Water, Fertilizer }

public class PlayerInteraction : MonoBehaviour
{
    // Event/Sinyal untuk berkomunikasi dengan UI
    public static event Action<HeldItemType> OnHeldItemChanged;
    public static event Action<float> OnInteractionProgress; // Mengirim progres (0 - 1)
    public static event Action<bool> OnInteractionStateChanged; // Memberitahu UI kapan harus muncul/hilang

    [Header("Pengaturan Interaksi")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private float interactionDuration = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    // Status Player
    private HeldItemType currentHeldItem = HeldItemType.None;
    private float interactionProgress = 0f;
    private bool isInteracting = false;
    
    // Referensi
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if(GuideManager.Instance != null) GuideManager.Instance.ShowTimedGuide(GuideType.Move);
    }

    void Update()
    {
        Collider2D nearbyObject = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayer);

        // 1. Tentukan apakah ada interaksi yang valid saat ini
        bool canInteract = false;
        if (nearbyObject != null)
        {
            PickupableItem item = nearbyObject.GetComponent<PickupableItem>();
            PlantGrowth plant = nearbyObject.GetComponent<PlantGrowth>();

            // Aturan untuk mengambil item
            if (item != null && currentHeldItem == HeldItemType.None)
            {
                canInteract = true;
            }
            // Aturan untuk berinteraksi dengan tanaman
            else if (plant != null)
            {
                // Bisa interaksi jika membawa sesuatu ATAU jika ini adalah penanaman pertama
                if (currentHeldItem != HeldItemType.None || plant.IsReadyForFirstGrowth())
                {
                    canInteract = true;
                }
            }
        }

        // 2. Tampilkan atau sembunyikan guide berdasarkan kondisi
        if (canInteract && !isInteracting)
        {
            if(GuideManager.Instance != null) GuideManager.Instance.ShowSituationalGuide(GuideType.Interact);
        }
        else
        {
            if(GuideManager.Instance != null) GuideManager.Instance.HideSituationalGuide(GuideType.Interact);
        }

        // 3. Jalankan logika interaksi jika valid dan tombol F ditekan
        if (canInteract && Input.GetKey(KeyCode.F))
        {
            isInteracting = true;
            interactionProgress += Time.deltaTime;
            playerMovement.SetAimingState(true); 
            
            OnInteractionProgress?.Invoke(interactionProgress / interactionDuration);
            OnInteractionStateChanged?.Invoke(true);

            if (interactionProgress >= interactionDuration)
            {
                PerformInteraction(nearbyObject.gameObject);
                ResetInteraction();
            }
        }
        else if (Input.GetKeyUp(KeyCode.F) || !canInteract)
        {
            // Reset jika tombol dilepas atau jika interaksi tidak lagi valid
            if(isInteracting) ResetInteraction();
        }
    }

    private void PerformInteraction(GameObject target)
    {
        // Cek tipe interaksi
        PickupableItem item = target.GetComponent<PickupableItem>();
        PlantGrowth plant = target.GetComponent<PlantGrowth>();
        EndingTrigger endTrigger = target.GetComponent<EndingTrigger>();

        if (item != null && currentHeldItem == HeldItemType.None) // Mengambil item
        {
            currentHeldItem = item.itemType;
            OnHeldItemChanged?.Invoke(currentHeldItem);
            if (item.itemType != HeldItemType.Water)
            {
                Destroy(target);
            }
        }
        else if (plant != null) // Berinteraksi dengan tanaman
        {
            if (currentHeldItem == HeldItemType.Water) // Memberi air
            {
                if (plant.ApplyWater())
                {
                    currentHeldItem = HeldItemType.None;
                    OnHeldItemChanged?.Invoke(currentHeldItem);
                }
            }
            else if (currentHeldItem == HeldItemType.Fertilizer) // Memberi pupuk
            {
                if (plant.ApplyFertilizer())
                {
                    currentHeldItem = HeldItemType.None;
                    OnHeldItemChanged?.Invoke(currentHeldItem);
                }
            }
            else // Meningkatkan level tanaman
            {
                plant.AttemptUpgrade();
            }
        }
        else if (endTrigger != null)
        {
                Debug.Log("Memicu ending, memuat scene: " + endTrigger.endingSceneName);
                // Pastikan waktu kembali normal sebelum pindah scene
                Time.timeScale = 1f;
                // Muat scene ending
                SceneManager.LoadScene(endTrigger.endingSceneName);
        }
    }

    private void ResetInteraction()
    {
        isInteracting = false;
        interactionProgress = 0f;
        playerMovement.SetAimingState(false); // Kembalikan kecepatan gerak normal
        OnInteractionStateChanged?.Invoke(false);
        GuideManager.Instance.HideSituationalGuide(GuideType.Interact);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}