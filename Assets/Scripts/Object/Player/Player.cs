using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Object
{
    #region Data
    private Gender gender;
    private Species species;
    private Equipment equipment;
    private int healthMax, healthCurrent;

    private StatsHolder stats;

    private Agent target;

    private bool alive;
    #endregion Data

    #region Properties
    public int HealthMax { get => healthMax; }
    public int HealthCurrent { get => healthCurrent; }

    public Gender Gender { get => gender; }
    public Species Species { get => species; }
    public Equipment Equipment { get => equipment; }
    public Inventory Inventory { get => equipment.Inventory; }
    public Weapon Weapon { get => (Weapon)equipment[EquipmentSlot.Held]; }

    public int Level { get => stats.Level; }
    public StatsHolder Stats { get => stats; }

    public Agent Target { get => target; }

    public bool Alive { get => alive; }
    #endregion Properties


    #region Methods
    public Player(string name, Gender gender, Species species)
        : base(name, Color.green, Data.Tiles["Player"])
    {
        this.gender = gender;
        this.species = species;
        alive = true;
        stats = new StatsHolder(species);
        healthMax = healthCurrent = (species.Health * 2) + (stats[StatName.Vitality]*StatsHolder.HP_PER_VITALITY_POINT);


        MaterialData startingWeaponMaterial = Data.Materials["Stone"];
        Weapon startingWeapon = ItemGenerator.GenerateWeapon(Data.Items["Axe"], startingWeaponMaterial);

        MaterialData startingTunicMaterial = Data.Materials["Leather"];
        Armour startingTunic = ItemGenerator.GenerateArmour(Data.Items["Tunic"], startingTunicMaterial);

        Item berry = ItemGenerator.GenerateItem(Data.Items["Berry"]);

        List<Item> startingItems = new List<Item>() { startingWeapon, startingTunic };
        equipment = new Equipment(25, species, startingItems);
        equipment.Inventory.PlaceItem(berry, 5);
    }

    public bool IncreaseStat(StatName statName)
    {
        bool success = stats.IncreaseStat(statName);
        if (success && statName == StatName.Vitality)
        {
            healthMax += StatsHolder.HP_PER_VITALITY_POINT;
            healthCurrent += StatsHolder.HP_PER_VITALITY_POINT;
        }
        return success;
    }
    public bool Move(Vector2Int direction)
    {// Player moves 1 cell in the given direction. Returns true if move was successful
        MapCell initialLocation = location;
        if (Data.Dungeon.CellAtPosition(location.Position + direction).AddPlayer(this))
        {
            initialLocation.RemovePlayer();
            return true;
        }
        return false;
    }

    public void Heal(int healAmount)
    {
        healthCurrent += healAmount;
        if (healthCurrent > healthMax) healthCurrent = healthMax;
    }
    public void TakeDamage(Damage damage)
    {// Player takes damage and checks if it is fatal
        healthCurrent -= damage.Power;
        Data.UIController.UpdateTextBox(UIBoxes.Health);

        if (CheckFatality())
            Die();
    }
    private bool CheckFatality()
    {// Player checks if its health is below zero, i.e if agent has to die
        if (healthCurrent <= 0)
            return true;

        return false;
    }
    private void Die()
    {
        alive = false;
        Logger.SendMessageToUI("You died!");
    }


    public string Inspect()
    {
        string description;
        description = $"You are a {species.Name}. You are level {stats.Level} with {stats.Experience} exp. Your stats are as follows: ";
        description += stats.GetDescription();
        description += $". You have {stats.StatPoints} available points to increase your stats.";

        return description;
    }

    public void SetTarget(Agent newTarget)
    {
        target = newTarget;
    }
    #endregion Methods
}