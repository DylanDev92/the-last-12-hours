using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Inventory : IEnumerable<Item>
{
    private List<Item> _items;

    public Inventory()
    {
        _items = new List<Item>();
    }

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    public IEnumerator<Item> GetEnumerator() => _items.GetEnumerator();
    public Item Get(ItemType type) => this.FirstOrDefault(item => item.type == type);
    public bool ContainsType(ItemType type) => Get(type)?.type != null;

    public event Action<Item> OnAddItem;
    public event Action<Item> OnRemoveItem;
    public event Action<Item> OnUpdateItem;
    public event Action OnChange;

    public void Clear()
    {
        _items.Clear();
        OnChange?.Invoke();
    }

    // Add items to the list
    public void Add(Item item)
    {
        item.amount = Math.Max(1, item.amount); // sanity helper

        if (ContainsType(item.type))
        {
            var existing = Get(item.type);
            existing.amount += item.amount;
            OnUpdateItem?.Invoke(existing);
        }
        else
        {
            _items.Add(item);
            OnAddItem?.Invoke(item);
        }

        Debug.Log($"Item {item.type} was added to the inventory, amount={item.amount}");
        OnChange?.Invoke();
    }

    public void Remove(ItemType type, int amount = 1)
    {
        if (!ContainsType(type)) return;

        var existing = Get(type);

        if (existing.amount > amount)
        {
            existing.amount -= amount;
            OnUpdateItem?.Invoke(existing);
        }
        else
        {
            _items.Remove(existing);
            OnRemoveItem?.Invoke(existing);
        }

        OnChange?.Invoke();
    }
}