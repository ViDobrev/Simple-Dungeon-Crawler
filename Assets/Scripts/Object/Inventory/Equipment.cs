using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipment
{
    #region Data
    private Dictionary<EquipmentSlot, Item> slots = new Dictionary<EquipmentSlot, Item>()
    {
        {EquipmentSlot.Head,    null},
        {EquipmentSlot.Torso,   null},
        {EquipmentSlot.Back,    null},
        {EquipmentSlot.Hands,   null},
        {EquipmentSlot.Waist,   null},
        {EquipmentSlot.Legs,    null},
        {EquipmentSlot.Feet,    null},

        {EquipmentSlot.Held,    null},
        {EquipmentSlot.Shield,  null}
    };

    private Inventory inventory;
    //private Agent agent;
    #endregion Data

    #region Properties
    public Dictionary<EquipmentSlot, Item> Slots { get => slots; }
    public Item this[EquipmentSlot key]
    {
        get => slots != null ? slots[key] : null;
    }

    public Inventory Inventory { get => inventory; }
    #endregion Properties


    #region Methods
    public Equipment(int inventorySlots, Species species)
    {
        if (!species.Intelligent) slots = null;
        inventory = new Inventory(inventorySlots);
    }
    public Equipment(int inventorySlots, Species species, List<Item> items)
    {
        if (!species.Intelligent) slots = null;
        inventory = new Inventory(inventorySlots);

        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            if (species.Intelligent) EquipItem(item);
        }
    }
    public void SetAgent(Agent agent)
    {
        //this.agent = agent;
        inventory.SetParentObject(agent);
    }

    public bool EquipItem(Item item)
    {// Returns true if the given item was equipped, false if it could not be
        if (item.GetType() == typeof(Weapon))
        {
            Item itemToEquip = item;

            EquipmentSlot slot = (item as Weapon).Slot;
            item = slots[slot];
            slots[slot] = itemToEquip;
            if (item != null) inventory.PlaceItem(item);
            return true;
        }
        if (item.GetType() == typeof(Armour))
        {
            Item itemToEquip = item;

            EquipmentSlot slot = (item as Armour).Slot;
            item = slots[slot];
            slots[slot] = itemToEquip;
            if (item != null) inventory.PlaceItem(item);
            return true;
        }

        return false;
    }
    public Item UnequipItem(string itemName)
    {
        foreach(EquipmentSlot slot in slots.Keys)
        {
            if (slots[slot] != null && slots[slot].FullName.ToLower() == itemName.ToLower())
            {
                Item requestedItem = slots[slot];
                slots[slot] = null;
                return requestedItem;
            }
        }

        Debug.Log($"Item {itemName} not found in equipment list.");
        return null;
    }
    public Item UnequipItem(int index)
    {
        Item requestedItem = slots[slots.Keys.ToArray()[index]];
        slots[slots.Keys.ToArray()[index]] = null;

        return requestedItem;
    }
    #endregion Methods
}



public enum EquipmentSlot : byte
{
    Head,
    Torso,
    Back,
    Hands,
    Waist,
    Legs,
    Feet,

    Held,
    Shield
}