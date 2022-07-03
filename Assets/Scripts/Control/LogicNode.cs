using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LogicNode : MonoBehaviour
{
    #region Data
    private DataNode dataNode;
    private GUINode guiNode;
    private DisplayNode displayNode;
    private UIControl uiController;

    private Player player;
    private Dungeon dungeon;
    #endregion Data


    #region Methods
    private void Initialize()
    {
        dataNode = FindObjectOfType<DataNode>();
        guiNode = FindObjectOfType<GUINode>();
        displayNode = FindObjectOfType<DisplayNode>();
        uiController = FindObjectOfType<UIControl>();

        dataNode.Initialize(this, displayNode, uiController);
        guiNode.Initialize(this, dataNode, displayNode);
        displayNode.Initialize(this, dataNode);
    }
    private void SetDungeon()
    {
        dataNode.SetMap(dungeon);
        guiNode.SetDungeon(dungeon);
        displayNode.SetDungeon(dungeon);

        Pathfinder.SetDungeon(dungeon);
        Data.SetDungeon(dungeon);
    }

    public void StartGame()
    {
        CreatePlayer();

        GenerateNextDungeonLevel();
        uiController.ResetMessageBox();
    }


    private void CreatePlayer()
    {
        player = new Player("Player", Gender.Male, Data.Species["Human"]);
        Data.SetPlayerReference(player);
    }

    private void GenerateDungeon()
    {
        dungeon = DungeonGenerator.GenerateDungeon(dataNode);
        SetDungeon();
        displayNode.DisplayDungeon();
    }
    private void GenerateNextDungeonLevel()
    {
        GenerateDungeon();

        dungeon.EntranceCell.AddPlayer(player);
        uiController.UpdateAllTextBoxes();
        Data.CentreCameraOnPlayer();
    }

    private void GameOver()
    {
        uiController.ActivateGameOverWindow();
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
        Initialize();
        StartGame();
    }
    void Update()
    {
        if (!Data.Player.Alive)
        {
            GameOver();
            return;
        }
        if (!Data.PlayerHasActed) return;
        if (Data.MustGenerateNextDungeonLevel)
        {
            GenerateNextDungeonLevel();
            Data.UIController.UpdateTextBox(UIBoxes.Description);
            Data.SetTurnComplete(false);
            return;
        }
        foreach (Agent agent in Data.Agents) agent.Act();

        Data.UIController.UpdateTextBox(UIBoxes.Description);
        Data.SetTurnComplete(false);
    }
    #endregion Behaviour
}