using UnityEngine;

public class PlayerSystemComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySystemComponent _inventorySystem;
    [SerializeField] private PlayerRootComponent _playerRoot;

    private void Awake()
    {
        if (_playerRoot)
        {
            _playerRoot.Setup(_inventorySystem);
        }
    }
}
