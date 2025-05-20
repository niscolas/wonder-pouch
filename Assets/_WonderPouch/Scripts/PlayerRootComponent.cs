using UnityEngine;

public class PlayerRootComponent : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private ItemPickerComponent _itemPicker;
    [SerializeField] private InventoryOpenerComponent _inventoryOpener;

    public void Setup(InventorySystemComponent inventorySystem)
    {
        if (TryGetComponent(out _itemPicker))
        {
            _itemPicker.Setup(inventorySystem);
        }

        if (TryGetComponent(out _inventoryOpener))
        {
            _inventoryOpener.Setup(inventorySystem);
        }
    }
}
