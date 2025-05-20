using System;
using UnityEngine;

[Serializable]
public struct InventoryItem
{
    public ItemDefinition definition;
    public int currentStack;

    public InventoryItem(ItemDefinition definition, int currentStack) : this()
    {
        this.definition = definition;
        this.currentStack = currentStack;
    }
}

[Serializable]
public struct EquipmentItem
{
    public ItemDefinition definition;
    public EquipmentType type;
}

[Serializable]
public struct InventorySaveData
{
    [SerializeField] private InventoryItem[] _inventorySlots;
    [SerializeField] private EquipmentItem[] _equipmentSlots;

    public InventoryItem[] InventorySlots => _inventorySlots;
    public EquipmentItem[] EquipmentSlots => _equipmentSlots;

    public InventorySaveData(InventoryItem[] inventorySlots, EquipmentItem[] equipmentSlots)
    {
        _inventorySlots = inventorySlots;
        _equipmentSlots = equipmentSlots;
    }
}

public enum EquipmentType
{
    LeftHand,
    RightHand,
    Back,
    BeltLeft,
    BeltRight,
}

public class InventorySystemComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _clearSave;
    [SerializeField] private int _inventorySize = 16;
    [SerializeField] private EquipmentType[] _equipmentSlotTypes;

    [Header("References")]
    [SerializeField] private InventoryPanelComponent _inventoryPanel;

    private static string InventoryItemsSaveKey = "WonderPouch.Inventory";

    public event Action<int, InventoryItem> InventorySlotUpdated;
    public event Action<int, EquipmentItem> EquipmentSlotUpdated;

    public InventoryItem[] InventorySlots { get => _inventorySlots; }
    public EquipmentItem[] EquipmentSlots { get => _equipmentSlots; }

    private InventoryItem[] _inventorySlots;
    private EquipmentItem[] _equipmentSlots;

    private void Awake()
    {
        if (_clearSave)
        {
            PlayerPrefs.DeleteKey(InventoryItemsSaveKey);
        }

        if (_clearSave || !LoadData() || _inventorySlots.IsNullOrEmpty())
        {
            _inventorySlots = new InventoryItem[_inventorySize];
            SetupEquipmentSlots();
        }

        if (_inventoryPanel)
        {
            _inventoryPanel.Setup(
                    this, _inventorySize, _equipmentSlotTypes);
            _inventoryPanel.Hide();
        }
    }

    private void OnDestroy()
    {
        SaveData();
    }

    public bool ToggleVisibility()
    {
        return _inventoryPanel.Toggle();
    }

    public bool AddItem(InventoryItem newItem)
    {
        if (CheckIsStackable(newItem))
        {
            for (int i = 0; i < _inventorySlots.Length; i++)
            {
                if (!CheckCanSlotStackWith(i, newItem))
                {
                    continue;
                }

                _inventorySlots[i].currentStack += newItem.currentStack;
                NotifyInventorySlotUpdated(i);
                return true;
            }
        }

        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (!CheckIsEmptyInventorySlot(i))
            {
                continue;
            }

            _inventorySlots[i] = newItem;
            NotifyInventorySlotUpdated(i);
            return true;
        }

        Debug.LogWarning("Inventory is full!");
        return false;
    }

    public void RemoveItemAt(int slotIndex)
    {
        if (!CheckIsValidInventorySlotIndex(slotIndex))
        {
            return;
        }

        if (!CheckIsEmptyInventorySlot(slotIndex))
        {
            MakeInventorySlotEmpty(slotIndex);
            NotifyInventorySlotUpdated(slotIndex);
        }
    }

    public void MoveItem(int fromSlotIndex, int toSlotIndex)
    {
        if (!CheckIsValidInventorySlotIndex(fromSlotIndex) || !CheckIsValidInventorySlotIndex(toSlotIndex) ||
            (CheckIsEmptyInventorySlot(fromSlotIndex) && CheckIsEmptyInventorySlot(toSlotIndex)))
        {
            return;
        }

        if (CheckIsEmptyInventorySlot(toSlotIndex))
        {
            _inventorySlots[toSlotIndex] = _inventorySlots[fromSlotIndex];
            MakeInventorySlotEmpty(fromSlotIndex);
        }
        else if (CheckCanSlotStackWith(toSlotIndex, _inventorySlots[fromSlotIndex]))
        {
            _inventorySlots[toSlotIndex].currentStack += _inventorySlots[fromSlotIndex].currentStack;
            MakeInventorySlotEmpty(fromSlotIndex);
        }
        else
        {
            SwapItems(fromSlotIndex, toSlotIndex);
        }

        NotifyInventorySlotUpdated(fromSlotIndex);
        NotifyInventorySlotUpdated(toSlotIndex);
    }

    public void ConsumeOrEquipItem(int slotIndex)
    {
        if (!CheckIsValidInventorySlotIndex(slotIndex) || CheckIsEmptyInventorySlot(slotIndex))
        {
            return;
        }

        InventoryItem item = _inventorySlots[slotIndex];

        if (item.definition.IsConsumable)
        {
            ReduceStack(slotIndex);
            NotifyInventorySlotUpdated(slotIndex);
            Debug.Log($"Used {item.definition.ItemName}");

            return;
        }

        if (item.definition.IsEquippable)
        {
            if (TryEquipItem(item))
            {
                MakeInventorySlotEmpty(slotIndex);
                NotifyInventorySlotUpdated(slotIndex);
                Debug.Log($"Equipped {item.definition.ItemName}");
            }

            return;
        }
    }

    public void UnequipItem(int slotIndex)
    {
        if (!CheckIsValidEquipmentSlotIndex(slotIndex) ||
            CheckIsEmptyEquipmentSlot(slotIndex))
        {
            return;
        }

        InventoryItem asInventoryItem = new InventoryItem(
                        _equipmentSlots[slotIndex].definition,
                        1);

        if (AddItem(asInventoryItem))
        {
            MakeEquipmentSlotEmpty(slotIndex);
            NotifyEquipmentSlotUpdated(slotIndex);
        }
    }

    private bool TryEquipItem(InventoryItem item)
    {
        if (!item.definition)
        {
            return false;
        }

        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            if (_equipmentSlots[i].type != item.definition.EquipmentType ||
                _equipmentSlots[i].definition)
            {
                continue;
            }

            _equipmentSlots[i].definition = item.definition;
            NotifyEquipmentSlotUpdated(i);
        }

        return true;
    }

    public bool CheckIsStackable(InventoryItem item)
    {
        return item.definition.MaxStack > 1;
    }

    public bool CheckIsEmptyInventorySlot(int index)
    {
        return !_inventorySlots[index].definition && _inventorySlots[index].currentStack == 0;
    }

    public bool CheckIsEmptyEquipmentSlot(int index)
    {
        return !_equipmentSlots[index].definition;
    }

    public void SaveData()
    {
        InventorySaveData data = new InventorySaveData(_inventorySlots, _equipmentSlots);
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(InventoryItemsSaveKey, jsonData);
        PlayerPrefs.Save();
    }

    public bool LoadData()
    {
        if (!PlayerPrefs.HasKey(InventoryItemsSaveKey))
        {
            return false;
        }

        string jsonData = PlayerPrefs.GetString(InventoryItemsSaveKey);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(jsonData);
        _inventorySlots = data.InventorySlots;
        _equipmentSlots = data.EquipmentSlots;

        return true;
    }

    private void SetupEquipmentSlots()
    {
        _equipmentSlots = new EquipmentItem[_equipmentSlotTypes.Length];
        for (int i = 0; i < _equipmentSlotTypes.Length; i++)
        {
            _equipmentSlots[i].type = _equipmentSlotTypes[i];
        }
    }

    private bool CheckCanSlotStackWith(int index, InventoryItem item)
    {
        return _inventorySlots[index].definition == item.definition &&
               _inventorySlots[index].currentStack + item.currentStack <= _inventorySlots[index].definition.MaxStack;
    }

    private bool CheckIsValidInventorySlotIndex(int index)
    {
        return _inventorySlots.CheckIsValidIndex(index);
    }

    private bool CheckIsValidEquipmentSlotIndex(int index)
    {
        return _equipmentSlots.CheckIsValidIndex(index);
    }

    private void MakeInventorySlotEmpty(int index)
    {
        _inventorySlots[index].definition = null;
        _inventorySlots[index].currentStack = 0;
    }

    private void MakeEquipmentSlotEmpty(int index)
    {
        _equipmentSlots[index].definition = null;
    }

    private void SwapItems(int slotIndexA, int slotIndexB)
    {
        InventoryItem temp = _inventorySlots[slotIndexA];
        _inventorySlots[slotIndexA] = _inventorySlots[slotIndexB];
        _inventorySlots[slotIndexB] = temp;
    }

    private void ReduceStack(int slotIndex, int reduceAmount = 1)
    {
        if (!CheckIsValidInventorySlotIndex(slotIndex) || CheckIsEmptyInventorySlot(slotIndex))
        {
            return;
        }

        _inventorySlots[slotIndex].currentStack -= reduceAmount;
        if (_inventorySlots[slotIndex].currentStack <= 0)
        {
            MakeInventorySlotEmpty(slotIndex);
        }
    }

    private void NotifyInventorySlotUpdated(int index)
    {
        InventorySlotUpdated?.Invoke(index, _inventorySlots[index]);
    }

    private void NotifyEquipmentSlotUpdated(int index)
    {
        EquipmentSlotUpdated?.Invoke(index, _equipmentSlots[index]);
    }
}
