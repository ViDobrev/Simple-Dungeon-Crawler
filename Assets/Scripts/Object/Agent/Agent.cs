using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : Object
{
    #region Data
    private Gender gender;
    private Species species;
    private int healthMax, healthCurrent;

    private StatsHolder stats;
    private Equipment equipment;

    private AI ai;
    #endregion Data

    #region Properties
    public Species Species { get => species; }
    public bool Intelligent { get => species.Intelligent; }
    public int Level { get => stats.Level; }

    public StatsHolder Stats { get => stats; }
    public Equipment Equipment { get => equipment; }
    public Inventory Inventory { get => equipment.Inventory; }
    public Weapon Weapon { get => (Weapon)equipment[EquipmentSlot.Held]; }
    #endregion Properties


    #region Methods
    public Agent(string name, Gender gender, Species species, int level, Equipment equipment)
        : base(name, Color.red, species.Tile)
    {
        this.gender = gender;
        this.species = species;
        stats = new StatsHolder(species, level);

        healthMax = healthCurrent = (species.Health + (stats[StatName.Vitality] * StatsHolder.HP_PER_VITALITY_POINT)) / 2;


        this.equipment = equipment;
        equipment.SetAgent(this);

        ai = new AI(this);
    }


    public bool Move(Vector2Int direction)
    {// Agent moves 1 cell in the given direction. Returns true if move was successful
        MapCell initialLocation = location;
        bool hasMoved = Data.Dungeon.CellAtPosition(location.Position + direction).AddAgent(this);
        
        if (hasMoved) initialLocation.RemoveAgent();
        return hasMoved;
    }

    public void Heal(int healAmount)
    {
        healthCurrent += healAmount;
        if (healthCurrent > healthMax) healthCurrent = healthMax;
    }
    public bool TakeDamage(Damage damage)
    {// Agent takes damage and checks if it is fatal, returns true if it is
        healthCurrent -= damage.Power;

        if (CheckFatality())
        {
            Die();
            return true;
        }

        return false;
    }
    private bool CheckFatality()
    {// Agent checks if its health is below zero, i.e if agent has to die
        if (healthCurrent <= 0)
            return true;

        return false;
    }
    private void Die()
    {
        location.RemoveAgent();
        Data.RemoveAgent(this);
        //Debug.Log(name + " died!");
        if (this == Data.Dungeon.Boss) Data.Dungeon.UnlockExit();

        MapCell corpseLocation = Utilities.GetClosestCellByCondition(location, (cell) => !cell.HasCorpse && cell.Traversible);
        if (corpseLocation == null) return;

        Corpse corpse = new Corpse(this);
        corpseLocation.AddCorpse(corpse);
    }

    public void Act()
    {
        ai.Run();
    }
    #endregion Methods
}


public enum Gender : byte { Male, Female }