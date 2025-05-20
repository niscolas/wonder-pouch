using UnityEngine;

public class PickableItemComponent : MonoBehaviour
{
    [SerializeField] private InventoryItem _itemData;

    public InventoryItem ItemData { get => _itemData; }

    private void Awake()
    {
        if (_itemData.currentStack == 0)
        {
            _itemData.currentStack = 1;
        }
    }
}
