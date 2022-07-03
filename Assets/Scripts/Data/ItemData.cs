using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemData
{
    #region Data
    private string name;
    private ItemTag tag;
    private bool stackable;

    private MaterialData materialData;
    private FoodData foodData;

    private string attack;
    private int defence;
    private bool twoHanded;
    private EquipmentSlot equipmentSlot;
    #endregion Data

    #region Properties
    public string Name { get => name; }
    public ItemTag Tag { get => tag; }
    public bool Stackable { get => stackable; }

    public MaterialData MaterialData { get => materialData; }
    public FoodData FoodData { get => foodData; }

    public string Attack { get => attack; }
    public int Defence { get => defence; }
    public bool TwoHanded { get => twoHanded; }
    public EquipmentSlot EquipmentSlot { get => equipmentSlot; }
    #endregion Properties


    #region Methods
    public ItemData(string name, ItemTag tag, bool stackable, string attack, int defence, bool twoHanded, EquipmentSlot equipmentSlot)
    {
        this.name = name;
        this.tag = tag;
        this.stackable = stackable;

        this.materialData = null;
        this.foodData = null;

        this.attack = attack;
        this.defence = defence;
        this.twoHanded = twoHanded;
        this.equipmentSlot = equipmentSlot;
    }
    public ItemData(string name, bool stackable, FoodData foodData)
    {
        this.name = name;
        this.tag = ItemTag.Food;
        this.stackable = stackable;

        this.materialData = null;
        this.foodData = foodData;

        this.attack = null;
        this.defence = 0;
        this.twoHanded = false;
        this.equipmentSlot = EquipmentSlot.Held;
    }
    #endregion Methods
}