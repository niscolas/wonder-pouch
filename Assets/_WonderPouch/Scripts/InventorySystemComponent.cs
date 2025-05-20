using System;
using UnityEngine;

[Serializable]
public struct InventoryItem
{
    public ItemDefinition definition;
    public int currentStack;
}

[Serializable]
public struct InventorySaveData
{
    public InventoryItem[] Slots { get; private set; }

    public InventorySaveData(InventoryItem[] slots)
    {
        Slots = slots;
    }
}

public class InventorySystemComponent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _size;

    [Header("References")]
    [SerializeField] private InventoryPanelComponent _inventoryPanel;

    private static string InventorySaveKey = "Inventory";

    public event Action<int, InventoryItem> SlotUpdated;

    public InventoryItem[] Slots { get => _slots; }

    private InventoryItem[] _slots;

    private void Awake()
    {
        _slots = new InventoryItem[_size];
        if (_inventoryPanel)
        {
            _inventoryPanel.Setup(this, _size);
        }
    }

    public bool AddItem(InventoryItem newItem)
    {
        if (CheckIsStackable(newItem))
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (!CheckCanSlotStackWith(i, newItem))
                {
                    continue;
                }

                _slots[i].currentStack += newItem.currentStack;
                NotifySlotUpdated(i);
                return true;
            }
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (!CheckIsEmptySlot(i))
            {
                continue;
            }

            _slots[i] = newItem;
            NotifySlotUpdated(i);
            return true;
        }

        Debug.LogWarning("Inventory is full!");
        return false;
    }

    public void RemoveItemAt(int slotIndex)
    {
        if (!CheckIsValidSlotIndex(slotIndex))
        {
            return;
        }

        if (!CheckIsEmptySlot(slotIndex))
        {
            MakeSlotEmpty(slotIndex);
            NotifySlotUpdated(slotIndex);
        }
    }

    public void MoveItem(int fromSlotIndex, int toSlotIndex)
    {
        if (!CheckIsValidSlotIndex(fromSlotIndex) || !CheckIsValidSlotIndex(toSlotIndex) ||
            (CheckIsEmptySlot(fromSlotIndex) && CheckIsEmptySlot(toSlotIndex)))
        {
            return;
        }

        if (CheckIsEmptySlot(toSlotIndex))
        {
            _slots[toSlotIndex] = _slots[fromSlotIndex];
            MakeSlotEmpty(fromSlotIndex);
        }
        else if (CheckCanSlotStackWith(toSlotIndex, _slots[fromSlotIndex]))
        {
            _slots[toSlotIndex].currentStack += _slots[fromSlotIndex].currentStack;
            MakeSlotEmpty(fromSlotIndex);
        }
        else
        {
            SwapItems(fromSlotIndex, toSlotIndex);
        }

        NotifySlotUpdated(fromSlotIndex);
        NotifySlotUpdated(toSlotIndex);
    }

    public void ConsumeOrEquipItem(int slotIndex)
    {
        if (!CheckIsValidSlotIndex(slotIndex) || CheckIsEmptySlot(slotIndex))
        {
            return;
        }

        InventoryItem item = _slots[slotIndex];

        if (item.definition.IsConsumable)
        {
            ReduceStack(slotIndex);
            Debug.Log($"Used {item.definition.ItemName}");
        }
        else if (item.definition.IsEquippable)
        {
            ReduceStack(slotIndex);
            Debug.Log($"Equipped {item.definition.ItemName}");
        }

        NotifySlotUpdated(slotIndex);
    }

    public bool CheckIsStackable(InventoryItem item)
    {
        return item.definition.MaxStack > 1;
    }

    public bool CheckIsEmptySlot(int index)
    {
        return _slots[index].definition == null && _slots[index].currentStack == 0;
    }

    public void SaveInventory()
    {
        InventorySaveData data = new InventorySaveData(_slots);
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(InventorySaveKey, jsonData);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey(InventorySaveKey))
        {
            return;
        }

        string jsonData = PlayerPrefs.GetString(InventorySaveKey);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(jsonData);
        _slots = data.Slots;
    }

    private bool CheckCanSlotStackWith(int index, InventoryItem item)
    {
        return _slots[index].definition == item.definition &&
               _slots[index].currentStack + item.currentStack <= _slots[index].definition.MaxStack;
    }

    private bool CheckIsValidSlotIndex(int index)
    {
        return _slots.CheckIsValidIndex(index);
    }

    private void MakeSlotEmpty(int index)
    {
        _slots[index].definition = null;
        _slots[index].currentStack = 0;
    }

    private void SwapItems(int slotIndexA, int slotIndexB)
    {
        InventoryItem temp = _slots[slotIndexA];
        _slots[slotIndexA] = _slots[slotIndexB];
        _slots[slotIndexB] = temp;
    }

    private void ReduceStack(int slotIndex, int reduceAmount = 1)
    {
        if (!CheckIsValidSlotIndex(slotIndex) || CheckIsEmptySlot(slotIndex))
        {
            return;
        }

        _slots[slotIndex].currentStack -= reduceAmount;
        if (_slots[slotIndex].currentStack <= 0)
        {
            MakeSlotEmpty(slotIndex);
        }
    }

    private void NotifySlotUpdated(int index)
    {
        SlotUpdated?.Invoke(index, _slots[index]);
    }
}
