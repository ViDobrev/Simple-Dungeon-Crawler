using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Inventory
{
    #region Data
    private Object parentObject;
    private List<ItemSlot> storedItems;
    private int itemSlots;
    #endregion Data

    #region Properties
    public Object ParentObject { get => parentObject; }
    public int ItemSlots { get => itemSlots; }
    public int EmptySlots { get => itemSlots - storedItems.Count; }
    public bool IsEmpty { get => storedItems.Count == 0; }
    public bool IsFull { get => storedItems.Count == itemSlots; }

    public List<ItemSlot> StoredItems { get => storedItems;}
    #endregion Properties


    #region Methods
    public Inventory(int itemSlots)
    {
        storedItems = new List<ItemSlot>(itemSlots);
        this.itemSlots = itemSlots;
    }
    public void SetParentObject(Object parentObject)
    {
        this.parentObject = parentObject;
    }

    public bool PlaceItem(ItemSlot itemSlot)
    {
        if (EmptySlots > 0)
        {
            if (itemSlot.Item.Stackable)
            {
                for (int i = 0; i < storedItems.Count; i++)
                {
                    if (storedItems[i].Item.FullName == itemSlot.Item.FullName)
                    {
                        storedItems[i].Amount += itemSlot.Amount;
                        return true;
                    }
                }
            }

            storedItems.Add(itemSlot);
            itemSlot.Item.SetInventory(this);
            return true;
        }

        return false;
    }
    public bool PlaceItem(Item item)
    {
        if (EmptySlots > 0)
        {
            if (item.Stackable)
            {
                for (int i = 0; i < storedItems.Count; i++)
                {
                    if (storedItems[i].Item.FullName == item.FullName)
                    {
                        storedItems[i].Amount++;
                        return true;
                    }
                }
            }

            storedItems.Add(new ItemSlot(item, 1));
            item.SetInventory(this);
            return true;
        }

        return false;
    }
    public bool PlaceItem(Item item, int amount)
    {
        if (EmptySlots > 0)
        {
            if (item.Stackable)
            {
                for (int i = 0; i < storedItems.Count; i++)
                {
                    if (storedItems[i].Item.FullName == item.FullName)
                    {
                        storedItems[i].Amount += amount;
                        return true;
                    }
                }
            }

            storedItems.Add(new ItemSlot(item, amount));
            item.SetInventory(this);
            return true;
        }

        return false;
    }
    public ItemSlot RemoveItem(ItemSlot itemSlot)
    {
        ItemSlot requestedItem = null;
        foreach (ItemSlot storedItem in storedItems)
        {
            if (storedItem.FullName == itemSlot.FullName)
            {
                requestedItem = storedItem;
                break;
            }
        }

        if (requestedItem == null)
        {
            Debug.Log($"Item {itemSlot.FullName} could not be found.");
            return null;
        }

        if (itemSlot.Amount > requestedItem.Amount)
        {
            Debug.Log($"Amount({itemSlot.Amount}) requested is more than the amount of {itemSlot.FullName} in inventory({requestedItem.Amount}).");
            return null;
        }
        else if (itemSlot.Amount == requestedItem.Amount)
        {
            storedItems.Remove(requestedItem);
            return requestedItem;
        }
        else
        {
            ItemSlot itemToReturn = new ItemSlot(ItemGenerator.CloneItem(requestedItem.Item), itemSlot.Amount);
            requestedItem.Amount -= itemSlot.Amount;
            return itemToReturn;
        }
    }
    public ItemSlot RemoveItem(int index, int amount)
    {
        if (!(index >= 0 && index < storedItems.Count)) return null;
        if (amount > storedItems[index].Amount) return null;

        ItemSlot returnItem;
        if (amount == storedItems[index].Amount)
        {
            returnItem = storedItems[index];
            returnItem.Item.SetInventory(null);
            storedItems.RemoveAt(index);
        }
        else
        {
            returnItem = new ItemSlot(ItemGenerator.CloneItem(storedItems[index].Item), amount);
            storedItems[index].Amount -= amount;
        }
        return returnItem;
    }
    public ItemSlot RemoveItem(string itemName, int amount)
    {
        ItemSlot requestedItem = null;
        foreach (ItemSlot storedItem in storedItems)
        {
            if (storedItem.FullName.ToLower() == itemName.ToLower())
            {
                requestedItem = storedItem;
                break;
            }
        }

        if (requestedItem == null)
        {
            Debug.Log($"Item {itemName} could not be found.");
            return null;
        }

        if (amount > requestedItem.Amount)
        {
            Debug.Log($"Amount({amount}) requested is more than the amount of {itemName} in inventory({requestedItem.Amount}).");
            return null;
        }
        else if (amount == requestedItem.Amount)
        {
            storedItems.Remove(requestedItem);
            return requestedItem;
        }
        else
        {
            ItemSlot itemToReturn = new ItemSlot(ItemGenerator.CloneItem(requestedItem.Item), amount);
            requestedItem.Amount -= amount;
            return itemToReturn;
        }
    }

    public ItemSlot PeekItem(string itemName, int amount)
    {
        ItemSlot requestedItem = null;
        foreach (ItemSlot storedItem in storedItems)
        {
            if (storedItem.FullName.ToLower() == itemName.ToLower())
            {
                requestedItem = storedItem;
                break;
            }
        }

        if (requestedItem == null)
        {
            Debug.Log($"Item {itemName} could not be found.");
            return null;
        }

        if (amount > requestedItem.Amount)
        {
            Debug.Log($"Amount({amount}) requested is more than the amount of {requestedItem.FullName} in inventory({requestedItem.Amount}).");
            return null;
        }
        else if (amount == requestedItem.Amount)
        {
            return requestedItem;
        }
        else
        {
            ItemSlot itemToReturn = new ItemSlot(ItemGenerator.CloneItem(requestedItem.Item), amount);
            return itemToReturn;
        }
    }
    public ItemSlot PeekItem(int index, int amount)
    {
        if (index < 0 || index >= storedItems.Count) return null;

        ItemSlot requestedItem = storedItems[index];


        if (amount > requestedItem.Amount)
        {
            Debug.Log($"Amount({amount}) requested is more than the amount of {requestedItem.FullName} in inventory({requestedItem.Amount}).");
            return null;
        }
        else if (amount == requestedItem.Amount)
        {
            return requestedItem;
        }
        else
        {
            ItemSlot itemToReturn = new ItemSlot(ItemGenerator.CloneItem(requestedItem.Item), amount);
            return itemToReturn;
        }
    }

    public bool SwapItem(ref ItemSlot itemOutsideInventory, ref ItemSlot itemInsideInventory)
    {
        for (int i = 0; i < storedItems.Count; i++)
        {
            if (storedItems[i] == itemInsideInventory)
            {
                storedItems[i] = itemOutsideInventory;
                itemOutsideInventory = itemInsideInventory;

                storedItems[i].Item.SetInventory(this);
                itemOutsideInventory.Item.SetInventory(null);

                return true;
            }
        }

        return false;
    }
    #endregion Methods
}


public class ItemSlot
{
    #region Data
    private Item item;
    private int amount;
    #endregion Data

    #region Properties
    public Item Item { get => item; set => item = value; }
    public int Amount { get => amount; set => amount = value; }

    public string Name { get => item.Name; }
    public string FullName { get => item.FullName; }
    public Color Colour { get => item.Colour; }
    public Tile Tile { get => item.Tile; }
    #endregion Properties


    #region Methods
    public ItemSlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    #endregion Methods
}