using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ItemTooltipComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 _displayOffset = new Vector3(100, 0, 0);
    [SerializeField] private Color _equipmentTextColor = Color.blue;
    [SerializeField] private Color _consumableTextColor = Color.green;
    [SerializeField] private Color _miscTextColor = Color.gray;

    [Header("References")]
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemDescriptionText;
    [SerializeField] private TMP_Text _itemTypeText;

    [Header("Events")]
    [SerializeField] private UnityEvent _onShowPre;
    [SerializeField] private UnityEvent _onHidePost;

    public void Show(InventoryItem item, Vector3 itemUiPosition)
    {
        ItemDefinition itemDefinition = item.definition;
        if (!itemDefinition)
        {
            return;
        }

        transform.position = itemUiPosition + _displayOffset;
        _onShowPre?.Invoke();

        _itemNameText.text = itemDefinition.ItemName;
        _itemDescriptionText.text = itemDefinition.Description;

        if (itemDefinition.IsEquippable)
        {
            _itemTypeText.text = "Equipment";
            _itemTypeText.color = _equipmentTextColor;
        }
        else if (itemDefinition.IsConsumable)
        {
            _itemTypeText.text = "Consumable";
            _itemTypeText.color = _consumableTextColor;
        }
        else
        {
            _itemTypeText.text = "Misc";
            _itemTypeText.color = _miscTextColor;
        }
    }

    public void Hide()
    {
        _onHidePost?.Invoke();
    }
}
