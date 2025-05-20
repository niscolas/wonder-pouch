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

    public void Setup(InventoryPanelComponent inventoryPanel, int slotIndex)
    {
        _slotIndex = slotIndex;
        _inventoryPanel = inventoryPanel;
        _iconImage.raycastTarget = false;
    }

    public void SetItem(InventoryItem item)
    {
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
        // InventoryManager.Instance.ShowTooltip(_slotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // InventoryManager.Instance.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _iconImage.transform.SetAsLastSibling();
        _iconImage.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_iconImage.sprite == null)
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
