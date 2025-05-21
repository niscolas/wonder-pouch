using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class InventoryPanelComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private Ease _fadeInEase = Ease.InSine;
    [SerializeField] private Ease _fadeOutEase = Ease.OutSine;

    [Header("References")]
    [SerializeField] private CanvasGroup _visualRoot;
    [SerializeField] private InventorySlotWidgetComponent _slotWidgetPrefab;
    [SerializeField] private ItemTooltipComponent _tooltip;
    [SerializeField] private Transform _inventorySlotWidgetsParent;
    [SerializeField] private Transform _equipmentSlotWidgetsParent;

    [Header("Events")]
    [SerializeField] private UnityEvent _onShowPre;
    [SerializeField] private UnityEvent _onHidePost;

    [Header("Debug")]
    [SerializeField] private InventorySlotWidgetComponent[] _inventorySlotWidgets;
    [SerializeField] private InventorySlotWidgetComponent[] _equipmentSlotWidgets;

    public bool IsVisible => _visualRoot.gameObject.activeSelf;
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

    public bool Toggle()
    {
        bool isVisible = IsVisible;
        bool targetVisibility = !isVisible;
        if (isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }

        return targetVisibility;
    }

    public void Show()
    {
        _onShowPre?.Invoke();

        _visualRoot.gameObject.SetActive(true);
        _visualRoot
            .DOFade(1, _fadeDuration)
            .From(0)
            .SetEase(_fadeInEase);
    }

    public void Hide()
    {
        _visualRoot
            .DOFade(0, _fadeDuration)
            .SetEase(_fadeOutEase)
            .OnComplete(() =>
            {
                _visualRoot.gameObject.SetActive(false);
            });

        _onHidePost?.Invoke();
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
