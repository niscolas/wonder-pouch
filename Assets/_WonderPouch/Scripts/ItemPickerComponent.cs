using UnityEngine;
using DG.Tweening;

public class ItemPickerComponent : MonoBehaviour, InteractionHandler
{
    private PickableItemComponent _nearestPickableItem;
    private InventorySystemComponent _inventorySystem;

    public void Setup(InventorySystemComponent inventorySystem)
    {
        _inventorySystem = inventorySystem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PickableItemComponent pickableItem))
        {
            return;
        }

        _nearestPickableItem = pickableItem;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other || !_nearestPickableItem || other.gameObject != _nearestPickableItem.gameObject)
        {
            return;
        }

        _nearestPickableItem = null;
    }

    public bool TryHandleFirstAvailable()
    {
        if (!_nearestPickableItem || !_nearestPickableItem.ItemData.definition)
        {
            return false;
        }

        if (_inventorySystem.AddItem(_nearestPickableItem.ItemData))
        {
            _nearestPickableItem.transform.DOKill(true);
            Destroy(_nearestPickableItem.gameObject);
        }

        return true;
    }
}
