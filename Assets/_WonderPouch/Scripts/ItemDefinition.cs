using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Scriptable Objects/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Basic Settings")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;
    [SerializeField] private GameObject _prefab;

    [Header("Stacking")]
    [SerializeField] private int _maxStack = 1;

    [Header("Usage")]
    [SerializeField] private bool _isEquippable;
    [SerializeField] private bool _isConsumable;

    public string ItemName { get => _itemName; }
    public Sprite Icon { get => _icon; }
    public string Description { get => _description; }
    public int MaxStack { get => _maxStack; }
    public bool IsConsumable { get => _isConsumable; }
    public bool IsEquippable { get => _isEquippable; }
}
