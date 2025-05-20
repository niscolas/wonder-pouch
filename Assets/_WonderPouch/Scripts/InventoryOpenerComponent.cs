using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InventoryOpenerComponent : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent _onShow;
    [SerializeField] private UnityEvent _onHide;

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
            _onShow?.Invoke();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            _onHide?.Invoke();
        }
    }
}
