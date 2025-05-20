using UnityEngine;

public class InventoryPanelComponent : MonoBehaviour
{
    private InventorySystemComponent _inventorySystem;

    public void Setup(InventorySystemComponent inventorySystem)
    {
        _inventorySystem = inventorySystem;
    }
}
