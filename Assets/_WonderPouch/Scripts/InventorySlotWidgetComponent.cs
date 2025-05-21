using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotWidgetComponent : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _stackText;

    [Header("Events")]
    [SerializeField] private UnityEvent _onBeginDragUnityEvent;
    [SerializeField] private UnityEvent _onEndDragUnityEvent;
    [SerializeField] private UnityEvent _onUseUnityEvent;

    [Header("Debug")]
    [SerializeField] private int _slotIndex;
    [SerializeField] private bool _isEquipmentSlot;

    private InventoryPanelComponent _inventoryPanel;
    private Transform _iconImageInitialParent;

    public void Setup(InventoryPanelComponent inventoryPanel, int slotIndex)
    {
        _slotIndex = slotIndex;
        _inventoryPanel = inventoryPanel;

        _iconImage.raycastTarget = false;
        _iconImageInitialParent = _iconImage.transform.parent;
    }

    public void SetInventoryItem(InventoryItem item)
    {
        CommonSetItemLogic(item.definition);

        if (item.currentStack > 1)
        {
            _stackText.text = item.currentStack.ToString();
            _stackText.enabled = true;
        }
        else
        {
            _stackText.enabled = false;
        }

        _isEquipmentSlot = false;
    }

    public void SetEquipmentItem(EquipmentItem item)
    {
        CommonSetItemLogic(item.definition);

        _stackText.enabled = false;
        _isEquipmentSlot = true;
    }

    public void ClearSlot()
    {
        _iconImage.sprite = null;
        _iconImage.enabled = false;
        _stackText.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_inventoryPanel)
        {
            return;
        }

        _inventoryPanel.ShowTooltip(_slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_inventoryPanel)
        {
            return;
        }

        _inventoryPanel.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_inventoryPanel || _isEquipmentSlot)
        {
            return;
        }

        _iconImage.transform.SetParent(_inventoryPanel.transform);
        _iconImage.color = new Color(1, 1, 1, 0.5f);
        _onBeginDragUnityEvent?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_iconImage.sprite || _isEquipmentSlot)
        {
            return;
        }

        _iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isEquipmentSlot)
        {
            return;
        }

        GameObject droppedOn = eventData.pointerCurrentRaycast.gameObject;
        int targetSlotIndex = -1;

        if (droppedOn != null &&
            droppedOn.TryGetComponent(out InventorySlotWidgetComponent targetSlotWidget))
        {
            targetSlotIndex = targetSlotWidget._slotIndex;
        }

        _inventoryPanel.EndDragItem(
                this,
                _slotIndex,
                targetSlotIndex);

        _iconImage.transform.SetParent(_iconImageInitialParent);
        _iconImage.transform.localPosition = Vector3.zero;
        _iconImage.color = Color.white;
        _onEndDragUnityEvent?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }

        if (!_isEquipmentSlot)
        {
            _inventoryPanel.ConsumeOrEquipItem(_slotIndex);
        }
        else
        {
            _inventoryPanel.UnequipItem(_slotIndex);
        }

        _onUseUnityEvent?.Invoke();
    }

    private void CommonSetItemLogic(ItemDefinition itemDefinition)
    {
        if (!itemDefinition)
        {
            ClearSlot();
            return;
        }

        _iconImage.sprite = itemDefinition.Icon;
        _iconImage.enabled = true;
    }
}
