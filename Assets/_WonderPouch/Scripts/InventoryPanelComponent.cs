using UnityEngine;

public class InventoryPanelComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private InventorySlotWidgetComponent _slotWidgetPrefab;

    private InventorySlotWidgetComponent[] _slotWidgets;
    private InventorySystemComponent _inventorySystem;

    public void Setup(InventorySystemComponent inventorySystem, int slotCount)
    {
        if (!_inventorySystem)
        {
            return;
        }

        _inventorySystem = inventorySystem;
        _slotWidgets = new InventorySlotWidgetComponent[slotCount];
        _inventorySystem.SlotUpdated += OnSlotUpdated;
    }

    private void OnDestroy()
    {
        if (_inventorySystem)
        {
            _inventorySystem.SlotUpdated -= OnSlotUpdated;
        }
    }

    public void EndDragItem(
        InventorySlotWidgetComponent draggedSlotWidget,
        int fromSlotIndex,
        int toSlotIndex)
    {
        if (!_inventorySystem ||
            !draggedSlotWidget ||
            fromSlotIndex == toSlotIndex ||
            !CheckIsValidSlotWidgetIndex(fromSlotIndex) ||
            !CheckIsValidSlotWidgetIndex(toSlotIndex))
        {
            return;
        }

        _inventorySystem.MoveItem(fromSlotIndex, toSlotIndex);
    }

    public void ConsumeOrEquipItem(int slotIndex)
    {
        if (!_inventorySystem)
        {
            return;
        }

        _inventorySystem.ConsumeOrEquipItem(slotIndex);
    }

    private void CreateSlots(int slotCount)
    {
        if (!_slotWidgetPrefab)
        {
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            _slotWidgets[i] = Instantiate(_slotWidgetPrefab, transform);
            _slotWidgets[i].Setup(this, i);
            _slotWidgets[i].SetItem(_inventorySystem.Slots[i]);
        }
    }

    private void OnSlotUpdated(int slotIndex, InventoryItem newItem)
    {
        if (!CheckIsValidSlotWidgetIndex(slotIndex))
        {
            return;
        }

        _slotWidgets[slotIndex].SetItem(newItem);
    }

    private bool CheckIsValidSlotWidgetIndex(int index)
    {
        return _slotWidgets.CheckIsValidIndex(index);
    }

}
