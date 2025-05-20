using TMPro;
using UnityEngine;
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

    [Header("Debug")]
    [SerializeField] private int _slotIndex;

    private InventoryPanelComponent _inventoryPanel;
    private Transform _iconImageInitialParent;

    public void Setup(InventoryPanelComponent inventoryPanel, int slotIndex)
    {
        _slotIndex = slotIndex;
        _inventoryPanel = inventoryPanel;

        _iconImage.raycastTarget = false;
        _iconImageInitialParent = _iconImage.transform.parent;
    }

    public void SetItem(InventoryItem item)
    {
        if (!item.definition)
        {
            ClearSlot();
            return;
        }

        _iconImage.sprite = item.definition.Icon;
        _iconImage.enabled = true;

        if (item.currentStack > 1)
        {
            _stackText.text = item.currentStack.ToString();
            _stackText.enabled = true;
        }
        else
        {
            _stackText.enabled = false;
        }
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
        if (!_inventoryPanel)
        {
            return;
        }

        _iconImage.transform.SetParent(_inventoryPanel.transform);
        _iconImage.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_iconImage.sprite)
        {
            return;
        }

        _iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _inventoryPanel.ConsumeOrEquipItem(_slotIndex);
        }
    }
}
