using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryOpenerComponent : MonoBehaviour
{
    private InventorySystemComponent _inventorySystem;

    public void Setup(InventorySystemComponent inventorySystem)
    {
        _inventorySystem = inventorySystem;
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        if (!context.performed || !_inventorySystem)
        {
            return;
        }

        if (_inventorySystem.ToggleVisibility())
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
