using System;
using UnityEngine;

public class InventoryPanelComponent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySlotWidgetComponent _slotWidgetPrefab;
    [SerializeField] private ItemTooltipComponent _tooltip;
    [SerializeField] private Transform _inventorySlotWidgetsParent;
    [SerializeField] private Transform _equipmentSlotWidgetsParent;

    [Header("Debug")]
    [SerializeField] private InventorySlotWidgetComponent[] _inventorySlotWidgets;
    [SerializeField] private InventorySlotWidgetComponent[] _equipmentSlotWidgets;

    private InventorySystemComponent _inventorySystem;

    public void Setup(
        InventorySystemComponent inventorySystem,
        int inventorySlotCount,
        EquipmentType[] equipmentSlotTypes)
    {
        if (!inventorySystem)
        {
            return;
        }

        if (!_inventorySlotWidgetsParent)
        {
            _inventorySlotWidgetsParent = transform;
        }

        _inventorySystem = inventorySystem;
        _inventorySystem.InventorySlotUpdated += OnInventorySlotUpdated;
        _inventorySystem.EquipmentSlotUpdated += OnEquipmentSlotUpdated;

        SetupSlotWidgets(inventorySlotCount, equipmentSlotTypes);
    }

    private void OnDestroy()
    {
        if (_inventorySystem)
        {
            _inventorySystem.InventorySlotUpdated -= OnInventorySlotUpdated;
            _inventorySystem.EquipmentSlotUpdated -= OnEquipmentSlotUpdated;
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
            !CheckIsValidInventorySlotWidgetIndex(fromSlotIndex) ||
            !CheckIsValidInventorySlotWidgetIndex(toSlotIndex))
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

    public void UnequipItem(int slotIndex)
    {
        if (!_inventorySystem)
        {
            return;
        }

        _inventorySystem.UnequipItem(slotIndex);
    }

    public void ShowTooltip(int slotIndex)
    {
        if (_tooltip)
        {
            _tooltip.Show(_inventorySystem.InventorySlots[slotIndex], _inventorySlotWidgets[slotIndex].transform.position);
        }
    }

    public void HideTooltip()
    {
        if (_tooltip)
        {
            _tooltip.Hide();
        }
    }

    private void SetupSlotWidgets(int inventorySlotCount, EquipmentType[] equipmentSlotTypes)
    {
        if (!_slotWidgetPrefab)
        {
            return;
        }

        _inventorySlotWidgets = new InventorySlotWidgetComponent[inventorySlotCount];
        for (int i = 0; i < inventorySlotCount; i++)
        {
            _inventorySlotWidgets[i] = Instantiate(_slotWidgetPrefab, _inventorySlotWidgetsParent);
            _inventorySlotWidgets[i].Setup(this, i);
            _inventorySlotWidgets[i].SetInventoryItem(_inventorySystem.InventorySlots[i]);
        }

        _equipmentSlotWidgets = new InventorySlotWidgetComponent[equipmentSlotTypes.Length];
        for (int i = 0; i < equipmentSlotTypes.Length; i++)
        {
            _equipmentSlotWidgets[i] = Instantiate(_slotWidgetPrefab, _equipmentSlotWidgetsParent);
            _equipmentSlotWidgets[i].Setup(this, i);
            _equipmentSlotWidgets[i].SetEquipmentItem(_inventorySystem.EquipmentSlots[i]);
        }
    }

    private void OnInventorySlotUpdated(int slotIndex, InventoryItem newItem)
    {
        if (!CheckIsValidInventorySlotWidgetIndex(slotIndex))
        {
            return;
        }

        _inventorySlotWidgets[slotIndex].SetInventoryItem(newItem);
    }

    private void OnEquipmentSlotUpdated(int slotIndex, EquipmentItem newItem)
    {
        if (!CheckIsValidEquipmentSlotWidgetIndex(slotIndex))
        {
            return;
        }

        _equipmentSlotWidgets[slotIndex].SetEquipmentItem(newItem);
    }

    private bool CheckIsValidInventorySlotWidgetIndex(int index)
    {
        return _inventorySlotWidgets.CheckIsValidIndex(index);
    }

    private bool CheckIsValidEquipmentSlotWidgetIndex(int index)
    {
        return _equipmentSlotWidgets.CheckIsValidIndex(index);
    }
}
