using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite fertilizerSprite;

    private void OnEnable() { PlayerInteraction.OnHeldItemChanged += UpdateSlot; }
    private void OnDisable() { PlayerInteraction.OnHeldItemChanged -= UpdateSlot; }

    void UpdateSlot(HeldItemType itemType)
    {
        switch (itemType)
        {
            case HeldItemType.Water:
                slotImage.sprite = waterSprite;
                break;
            case HeldItemType.Fertilizer:
                slotImage.sprite = fertilizerSprite;
                break;
            default:
                slotImage.sprite = emptySprite;
                break;
        }
    }
}