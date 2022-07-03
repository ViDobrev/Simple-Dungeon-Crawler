using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapCell
{
    #region Data
    private Vector2Int position;
    private bool visible;

    private DungeonEntrance entrance;
    private MyTerrain terrain;
    private Floor floor;
    private ItemSlot item;
    private Plant plant;
    private Structure structure;
    private Corpse corpse;
    private Agent agent;

    private Player player;
    #endregion Data

    #region Properties
    public Vector2Int Position { get => position; }
    public bool Visible { get => visible; }
    public bool Traversible { get => HasStructure ? !structure.Traversible : terrain.Traversible; }
    public bool CanBeMovedTo { get => Traversible && !HasAgent; }

    public bool HasEntrance { get => entrance != null; }
    public bool HasItem { get => item != null; }
    public bool HasPlant { get => plant != null; }
    public bool HasStructure { get => structure != null; }
    public bool HasFloor { get => floor != null; }
    public bool HasCorpse { get => corpse != null; }
    public bool HasAgent { get => agent != null; }
    public bool HasPlayer { get => player != null; }


    public DungeonEntrance Entrance { get => entrance; }
    public MyTerrain Terrain { get => terrain; }
    public Floor Floor { get => floor; }
    public ItemSlot Item { get => item; }
    public Plant Plant { get => plant; }
    public Structure Structure { get => structure; }
    public Corpse Corpse { get => corpse; }
    public Agent Agent { get => agent; }

    public string UppermostName
    {
        get => HasPlayer    ? player.Name    :
               visible      ?
               HasAgent     ? agent.Name     :
               HasEntrance  ? entrance.Name  :
               HasStructure ? structure.Name :
               HasPlant     ? plant.Name     :
               HasCorpse    ? corpse.Name    :
               HasItem      ? item.Name      :
               HasFloor     ? floor.Name     :
               terrain.Name : Data.Unknown;
    }
    public Tile Tile
    {
        get => HasPlayer    ? player.Tile    :
               HasAgent     ? agent.Tile     :
               HasEntrance  ? entrance.Tile  :
               HasStructure ? structure.Tile :
               HasPlant     ? plant.Tile     :
               HasCorpse    ? corpse.Tile    :
               HasItem      ? item.Tile      :
               HasFloor     ? floor.Tile     :
               terrain.Tile;
    }
    public Color Colour
    {
        get => HasPlayer    ? player.Colour    :
               HasAgent     ? agent.Colour     :
               HasEntrance  ? entrance.Colour  :
               HasStructure ? structure.Colour :
               HasPlant     ? plant.Colour     :
               HasCorpse    ? corpse.Colour    :
               HasItem      ? item.Colour      :
               HasFloor     ? floor.Colour     :
               terrain.Colour;
    }
    #endregion Properties


    #region Methods
    public MapCell(Vector2Int position, MyTerrain terrain)
    {
        this.position = position;
        this.terrain = terrain;

        visible = true;
    }
    
    public bool AddItem(ItemSlot item)
    {// Returns true if item was attached successfully
        if (HasItem)
        {
            if (this.item.FullName == item.FullName && this.item.Item.Stackable)
            {
                this.item.Amount += item.Amount;
                return true;
            }
        }
        else if (!HasItem && !HasStructure && !HasPlant && Traversible)
        {
            this.item = item;
            item.Item.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddPlant(Plant plant)
    {// Returns true if plant was attached successfully
        if (!HasPlant && !HasStructure && Traversible)
        {
            this.plant = plant;
            this.plant.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddStructure(Structure structure)
    {// Returns true if structure was attached successfully
        if (!HasStructure && !HasPlant && Traversible)
        {
            this.structure = structure;
            this.structure.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddFloor(Floor floor)
    {// Returns true if floor was attached successfully
        if (!HasStructure && !HasPlant && !HasFloor && Traversible)
        {
            this.floor = floor;
            this.floor.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddCorpse(Corpse corpse)
    {// Returns true if corpse was attached successfully
        if (!HasCorpse && Traversible)
        {
            this.corpse = corpse;
            corpse.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddAgent(Agent agent)
    {// Returns true if agent was attached successfully
        if (!HasAgent && !HasPlayer && Traversible)
        {
            this.agent = agent;
            agent.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }
    public bool AddPlayer(Player player)
    {
        if (!HasPlayer && !HasAgent && Traversible)
        {
            this.player = player;
            player.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }

    public bool AddDungeonEntrance(DungeonEntrance entrance)
    {// Returns true if entrance was attached successfully
        if (!HasEntrance && Traversible)
        {
            this.entrance = entrance;
            entrance.SetLocation(this);
            UpdateTile();
            return true;
        }
        return false;
    }


    public ItemSlot GetItem(int amount)
    {
        if (item == null)
        {
            Debug.Log("There is no item at this location.");
            return null;
        }

        if (amount > item.Amount)
        {
            Debug.Log("Requested amount is more than what is found at this location.");
            return null;
        }
        else if (amount == item.Amount)
        {
            ItemSlot _item = item;
            RemoveItem();
            return _item;
        }
        else //(amount < item.Amount)
        {
            ItemSlot newSlot = new ItemSlot(ItemGenerator.CloneItem(item.Item), amount);
            item.Amount -= amount;
            return newSlot;
        }
    }
    public void RemoveItem()
    {
        item = null;
        UpdateTile();
    }
    public void RemovePlant()
    {
        plant = null;
        UpdateTile();
    }
    public void RemoveStructure()
    {
        structure = null;

        UpdateTile();
        //if (visible && Data.Dungeon != null) Data.Map.MakeNeighbouringCellsVisible(this);
    }
    public void RemoveFloor()
    {
        floor = null;
        UpdateTile();
    }
    public void RemoveCorpse()
    {
        corpse = null;
        UpdateTile();
    }
    public void RemoveAgent()
    {
        agent = null;
        UpdateTile();
    }
    public void RemovePlayer()
    {
        player = null;
        UpdateTile();
    }


    private void UpdateTile()
    {
        if (visible)
            Data.UpdateTile(this);
    }
    #endregion Methods
}