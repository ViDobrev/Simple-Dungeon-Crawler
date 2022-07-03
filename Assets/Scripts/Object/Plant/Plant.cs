using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plant : Object
{
    #region Data
    private PlantSpecies species;
    private int healthMax, healthCurrent;
    private float grown;
    #endregion Data

    #region Properties
    public PlantSpecies Species { get => species; }
    public PlantStage Stage
    {
        get
        {
            for (int i = PlantSpecies.LifeCycle.Length - 1; i > 0; i--)
            {
                if (grown >= PlantSpecies.LifeCycle[i]) return (PlantStage)i;
            }

            return PlantStage.Sprout;
        }
    }
    #endregion Properties


    #region Methods
    public Plant(PlantSpecies species, float grown, Vector2Int position, MapCell parentCell)
        : base(species.Name, species.Colour, species.Tile)
    {
        this.species = species;
        healthMax = healthCurrent = species.Health;
        this.grown = grown;
    }

    public void Heal(int healAmount)
    {
        healthCurrent += healAmount;
        if (healthCurrent > healthMax) healthCurrent = healthMax;
    }
    public void TakeDamage(int damage)
    {// Plant takes damage and checks if it is fatal
        healthCurrent -= damage;

        if (CheckFatality())
            Die();
    }
    private bool CheckFatality()
    {// Plant checks if its health is below zero, i.e if plant has to die
        if (healthCurrent <= 0)
            return true;

        return false;
    }
    private void Die()
    {
        location.RemovePlant();
        Data.RemovePlant(this);

        List<ItemSlot> itemsToDrop = new List<ItemSlot>();
        ItemSlot resource = new ItemSlot(ItemGenerator.GenerateItem(species.Resource), species.Yield[Stage]);

        itemsToDrop.Add(resource);

        InteractionHandler.DropItems(itemsToDrop, location.Position);

        //Debug.Log(name + " died!");
    }


    public void Grow()
    {
        grown += species.GrowingRate * Time.deltaTime;
    }
    #endregion Methods
}