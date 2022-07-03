using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataNode : MonoBehaviour
{
    #region Data
    private LogicNode logicNode;
    private DisplayNode displayNode;
    private Dungeon dungeon;

    [SerializeField] private int seed;
    [SerializeField] private DungeonParameters dungeonParameters;

    [SerializeField] private TileManager tileManager;
    #endregion Data

    #region Properties
    public int MainSeed { get => seed; }
    public DungeonParameters DungeonParameters { get => dungeonParameters; }
    #endregion Properties


    #region Methods
    public void Initialize(LogicNode logicNode, DisplayNode displayNode, UIControl uIController)
    {
        this.logicNode = logicNode;
        this.displayNode = displayNode;

        Data.Initialize(MainSeed, tileManager, this, uIController);
        tileManager = null;
        ItemGenerator.Initialize();
    }
    public void SetMap(Dungeon dungeon)
    {
        this.dungeon = dungeon;
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
    }
    void Update()
    {
    }
    public void OnValidate()
    {
    }
    #endregion Behaviour
}