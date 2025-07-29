using UnityEngine;
using System;

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
    }

    void Update()
    {
        // Mendeteksi objek terdekat yang bisa diajak interaksi
        Collider2D nearbyObject = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayer);

        if (nearbyObject != null && Input.GetKey(KeyCode.F))
        {
            // Mulai/lanjutkan interaksi
            isInteracting = true;
            interactionProgress += Time.deltaTime;
            playerMovement.SetAimingState(true); // Meminjam state aiming untuk menghentikan gerakan
            
            // Kirim sinyal ke progress bar
            OnInteractionProgress?.Invoke(interactionProgress / interactionDuration);
            OnInteractionStateChanged?.Invoke(true);

            // Jika progres sudah penuh
            if (interactionProgress >= interactionDuration)
            {
                PerformInteraction(nearbyObject.gameObject);
                ResetInteraction();
            }
        }
        else if (Input.GetKeyUp(KeyCode.F) || nearbyObject == null)
        {
            // Batalkan interaksi jika tombol F dilepas atau menjauh
            if(isInteracting) ResetInteraction();
        }
    }
    
    private void PerformInteraction(GameObject target)
    {
        // Cek tipe interaksi
        PickupableItem item = target.GetComponent<PickupableItem>();
        PlantGrowth plant = target.GetComponent<PlantGrowth>();

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
    }

    private void ResetInteraction()
    {
        isInteracting = false;
        interactionProgress = 0f;
        playerMovement.SetAimingState(false); // Kembalikan kecepatan gerak normal
        OnInteractionStateChanged?.Invoke(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}