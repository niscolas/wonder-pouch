using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Scriptable Objects/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Basic Settings")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _tooltipText;
    [SerializeField] private GameObject _prefab;

    [Header("Stacking")]
    [SerializeField] private int _maxStack;

    [Header("Usage")]
    [SerializeField] private bool _isEquippable;
    [SerializeField] private bool _isConsumable;
}
