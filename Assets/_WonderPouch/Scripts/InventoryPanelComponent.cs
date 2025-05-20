using UnityEngine;

public class InventoryPanelComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySlotWidgetComponent _slotWidgetPrefab;
    [SerializeField] private Transform _slotWidgetsParent;
    [SerializeField] private ItemTooltipComponent _tooltip;

    private InventorySlotWidgetComponent[] _slotWidgets;
    private InventorySystemComponent _inventorySystem;

    public void Setup(InventorySystemComponent inventorySystem, int slotCount)
    {
        if (!inventorySystem)
        {
            return;
        }

        if (!_slotWidgetsParent)
        {
            _slotWidgetsParent = transform;
        }

        _inventorySystem = inventorySystem;
        _inventorySystem.SlotUpdated += OnSlotUpdated;

        CreateSlots(slotCount);
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

    public void ShowTooltip(int slotIndex)
    {
        if (_tooltip)
        {
            _tooltip.Show(_inventorySystem.Slots[slotIndex], _slotWidgets[slotIndex].transform.position);
        }
    }

    public void HideTooltip()
    {
        if (_tooltip)
        {
            _tooltip.Hide();
        }
    }

    private void CreateSlots(int slotCount)
    {
        if (!_slotWidgetPrefab)
        {
            return;
        }

        _slotWidgets = new InventorySlotWidgetComponent[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            _slotWidgets[i] = Instantiate(_slotWidgetPrefab, _slotWidgetsParent);
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
