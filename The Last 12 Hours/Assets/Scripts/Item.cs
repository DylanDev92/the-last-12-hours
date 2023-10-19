using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    // Auto Consumables
    Ammo = 1,

    // Manual Consumables
    Battery = 1000,
    Bandage, 

    // Equipment
    Flashlight = 2000,
    Axe,
    Knife,
    Gun
}
public class Item : Interactable
{
    public ItemType Type;
    public bool IsEquipment
    {
        get => (Type >= ItemType.Flashlight);
    }
    public bool IsConsumable
    {
        get
        {
            if (Type < ItemType.Battery) return true; // Auto Consumables
            else if (Type < ItemType.Flashlight) return true; // Manual Consumables
            else return false;
        }
    }

    public int Amount { get; set; }

    public string Name;

    /*
     * TO ADD
     * 
     * additional item info/fields likeeeee 
     * bandage healing amount
     * weapon range
     * weapon damage
     * ammo count -> count of item? 
     * isStacking?? 
     * 
     */

    public bool IsAutoConsume
    {
        get => (Type < ItemType.Battery); // Auto Consumables
    }

    public void Use()
    {
        // Here will be the Use logic of the item.
    }

    // For picking Item
    public override void Interact()
    {
        Player.Instance.Inventory.AddItems(this);

        this.gameObject.SetActive(false);
    }
}
