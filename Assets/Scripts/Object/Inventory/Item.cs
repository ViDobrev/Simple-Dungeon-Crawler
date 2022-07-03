using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item : Object
{
    #region Data
    protected ItemTag tag;
    protected bool stackable;

    protected Inventory parentInventory;
    #endregion Data

    #region Properties
    public abstract string FullName { get; }
    public override Vector2Int Position
    {
        get
        {
            if (IsInsideInventory) return ParentInventory.ParentObject.Position;
            else return location.Position;
        }
    }

    public ItemTag Tag { get => tag; }
    public bool Stackable { get => stackable; }

    public bool IsInsideInventory { get => parentInventory != null; }
    public Inventory ParentInventory { get => parentInventory; }
    #endregion Properties


    #region Methods
    public Item(string name)
        : base(name, Color.magenta, Data.Tiles[name])
    {
        ItemData itemData = Data.Items[name];
        tag = itemData.Tag;
        stackable = itemData.Stackable;
    }
    public Item(Item originalItem)
        : base(originalItem.Name, originalItem.Colour, originalItem.Tile)
    {// Used for item cloning
        tag = originalItem.tag;
        stackable = originalItem.stackable;

        parentInventory = originalItem.parentInventory;
    }

    public void SetInventory(Inventory inventory)
    {
        parentInventory = inventory;
    }
    public abstract string Inspect();
    #endregion Methods
}


public class Weapon : Item
{
    #region Data
    private Attack attack;
    private EquipmentSlot slot;
    private bool twoHanded;

    private MaterialData material;
    private Quality quality;
    #endregion Data

    #region Properties
    public override string FullName { get => $"{Data.QualityNames[quality]} {material.Adjective} {name}".Trim(); }
    public Attack Attack { get => attack; }
    public EquipmentSlot Slot { get => slot; }
    #endregion Properties


    #region Methods
    public Weapon(string name, MaterialData material, Quality quality, Attack attack)
        : base (name)
    {
        ItemData data = Data.Items[name];

        this.attack = attack;
        this.slot = data.EquipmentSlot;
        this.twoHanded = data.TwoHanded;

        this.material = material;
        this.quality = quality;
    }
    public Weapon(Weapon originalWeapon)
        : base (originalWeapon)
    {// Used for item cloning
        attack = new Attack(originalWeapon.attack);
        slot = originalWeapon.slot;
        twoHanded = originalWeapon.twoHanded;

        material = originalWeapon.material;
        quality = originalWeapon.quality;
    }

    public override string Inspect()
    {
        string description;

        description = $"The {FullName} is a ";

        description += twoHanded ? "two handed" : "one handed";
        description += $" weapon. Its attack is {attack}.";

        return description;
    }
    #endregion Methods
}

public class Armour : Item
{
    #region Data
    private int defence;
    private EquipmentSlot slot;

    private MaterialData material;
    private Quality quality;
    #endregion Data

    #region Properties
    public override string FullName { get => $"{Data.QualityNames[quality]} {material.Adjective} {name}".Trim(); }
    public int Defence { get => defence; }
    public EquipmentSlot Slot { get => slot; }
    #endregion Properties


    #region Methods
    public Armour(string name, MaterialData material, Quality quality, int defence)
        : base (name)
    {
        ItemData data = Data.Items[name];

        this.defence = defence;
        this.slot = data.EquipmentSlot;

        this.material = material;
        this.quality = quality;
    }
    public Armour(Armour originalArmour)
        : base(originalArmour)
    {// Used for item cloning
        defence = originalArmour.defence;
        slot = originalArmour.slot;

        material = originalArmour.material;
        quality = originalArmour.quality;
    }

    public override string Inspect()
    {
        string description;

        description = $"The {FullName} is an armour piece which must be worn on your {slot}. It provides you with {defence} defence.";

        return description;
    }
    #endregion Methods
}

public class Food : Item
{
    #region Data
    private FoodData foodData;
    #endregion Data

    #region Properties
    public override string FullName { get => name; }
    public int Nutrition { get => foodData.Nutrition; }
    #endregion Properties


    #region Methods
    public Food(string name)
        : base(name)
    {
        ItemData data = Data.Items[name];
        foodData = data.FoodData;
    }
    public Food(Food originalFood)
        : base(originalFood)
    {// Used for item cloning
        foodData = originalFood.foodData;
    }

    public override string Inspect()
    {
        string description;

        description = $"The {FullName} is a food item with {Nutrition} nutrition.";

        return description;
    }
    #endregion Methods
}





public enum ItemTag : byte
{
    Weapon_Melee,
    Weapon_Range,
    Ammunition,

    Shield,

    Armour,

    Food
}


public enum Quality : sbyte
{
    Awful = -2,
    Bad,
    Normal,
    WellMade,
    Masterwork
}