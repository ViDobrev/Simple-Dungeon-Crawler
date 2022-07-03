using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public static class ItemGenerator
{
    private static int maxTier;

    private static int[] qualities = { -2, -1, 0, 1, 2 };
    private static int[] qualityProbability = { 5, 20, 50, 20, 5 };

    #region Methods
    public static void Initialize()
    {
        maxTier = 0;

        foreach (MaterialData material in Data.Materials.Values)
        {
            if (material.Tier > maxTier) maxTier = material.Tier;
        }
    }

    public static Item GenerateItem(ItemData itemData)
    {
        if (itemData.Tag == ItemTag.Food)
        {
            return new Food(itemData.Name);
        }


        Debug.LogError($"ItemGenerator: Item ({itemData.Name}) could not be generated.");
        return null;
    }
    public static Item CloneItem(Item itemToCopy)
    {
        if (itemToCopy.GetType() == typeof(Weapon)) return new Weapon((Weapon)itemToCopy);
        if (itemToCopy.GetType() == typeof(Armour)) return new Armour((Armour)itemToCopy);
        if (itemToCopy.GetType() == typeof(Food)) return new Food((Food)itemToCopy);

        return null;
        //return new Item(itemToCopy);
    }


    public static Weapon GenerateWeapon(ItemData itemData, MaterialData material)
    {

        if ((itemData.Tag == ItemTag.Weapon_Melee) || (itemData.Tag == ItemTag.Weapon_Range))
        {
            Quality quality = (Quality)Utilities.RandomNumberByPropbability(qualities, qualityProbability);
            Weapon weapon = CreateWeapon(itemData, material, quality);
            return weapon;
        }

        Debug.LogError($"ItemGenerator: Item ({itemData.Name}) could not be generated.");
        return null;
    }
    public static Weapon GenerateRandomWeapon()
    {
        int playerLevel = Data.Player.Level;
        int tier = Data.mapRng.Next(playerLevel - 1, playerLevel + 2);
        if (tier > maxTier) tier = maxTier;
        if (tier < 1) tier = 1;

        var materials = (from MaterialData material in Data.Materials.Values
                         where (material.Tier == tier) && material.CanBeUsedForWeapon
                         select material).ToArray();

        var weapons = (from ItemData item in Data.Items.Values
                       where (item.Tag == ItemTag.Weapon_Melee) || (item.Tag == ItemTag.Weapon_Range)
                       select item).ToArray();


        MaterialData chosenMaterial = materials[Data.mapRng.Next(materials.Length)];
        ItemData chosenWeapon = weapons[Data.mapRng.Next(weapons.Length)];
        Quality quality = (Quality)Utilities.RandomNumberByPropbability(qualities, qualityProbability);

        Weapon weapon = CreateWeapon(chosenWeapon, chosenMaterial, quality);
        return weapon;
    }
    public static Armour GenerateArmour(ItemData itemData, MaterialData material)
    {

        if (itemData.Tag == ItemTag.Armour)
        {
            Quality quality = (Quality)Utilities.RandomNumberByPropbability(qualities, qualityProbability);
            Armour armour = CreateArmour(itemData, material, quality);
            return armour;
        }

        Debug.LogError($"ItemGenerator: Item ({itemData.Name}) could not be generated.");
        return null;
    }
    public static Armour GenerateRandomArmour()
    {
        int playerLevel = Data.Player.Level;
        int tier = Data.mapRng.Next(playerLevel - 1, playerLevel + 2);
        if (tier > maxTier) tier = maxTier;
        if (tier < 1) tier = 1;

        var materials = (from MaterialData material in Data.Materials.Values
                         where (material.Tier == tier) && material.CanBeUsedForArmour
                         select material).ToArray();

        var armours = (from ItemData item in Data.Items.Values
                       where item.Tag == ItemTag.Armour || item.Tag == ItemTag.Shield
                       select item).ToArray();


        MaterialData chosenMaterial = materials[Data.mapRng.Next(materials.Length)];
        ItemData chosenArmour = armours[Data.mapRng.Next(armours.Length)];
        Quality quality = (Quality)Utilities.RandomNumberByPropbability(qualities, qualityProbability);

        Armour armour = CreateArmour(chosenArmour, chosenMaterial, quality);
        return armour;
    }
    /*public static Armour GenerateRandomShield()
    {

        int playerLevel = Data.Player.Level;
        int tier = Data.mapRng.Next(playerLevel - 1, playerLevel + 2);
        if (tier > maxTier) tier = maxTier;
        if (tier < 1) tier = 1;

        var materials = (from MaterialData material in Data.Materials.Values
                         where (material.Tier == tier) && material.CanBeUsedForArmour
                         select material).ToArray();

        MaterialData chosenMaterial = materials[Data.mapRng.Next(materials.Length)];
        ItemData shieldData = Data.Items["Shield"];
        Quality quality = (Quality)Utilities.RandomNumberByPropbability(qualities, qualityProbability);

        Armour shield = CreateArmour(shieldData, chosenMaterial, quality);
        return shield;
    }*/



    private static Weapon CreateWeapon(ItemData itemData, MaterialData material, Quality quality)
    {
        if (!material.CanBeUsedForWeapon) return null;

        Attack attack = (itemData.Attack != null) ? new Attack(itemData.Attack, (int)quality + material.Modifier) : null;

        Weapon weapon = new Weapon(itemData.Name, material, quality, attack);
        return weapon;
    }
    private static Armour CreateArmour(ItemData itemData, MaterialData material, Quality quality)
    {
        if (!material.CanBeUsedForArmour) return null;

        int defence = itemData.Defence + material.Modifier;

        Armour armour = new Armour(itemData.Name, material, quality, defence);
        return armour;
    }
    #endregion Methods
}