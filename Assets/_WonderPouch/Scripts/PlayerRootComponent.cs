using UnityEngine;

public class PlayerRootComponent : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private ItemPickerComponent _itemPicker;

    public void Setup(InventorySystemComponent inventorySystem)
    {
        if (TryGetComponent(out _itemPicker))
        {
            _itemPicker.Setup(inventorySystem);
        }
    }
}
