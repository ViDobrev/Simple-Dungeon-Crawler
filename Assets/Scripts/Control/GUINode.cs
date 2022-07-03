using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GUINode : MonoBehaviour
{
    #region Data
    private LogicNode logicNode;
    private DataNode dataNode;
    private DisplayNode displayNode;
    private Dungeon dungeon;

    [SerializeField] private Camera mainCamera;
    [SerializeField] [Range(0.01f, 0.1f)] private float scrollSensitivity;
    #endregion Data


    #region Methods
    public void Initialize(LogicNode logicNode, DataNode dataNode, DisplayNode displayNode)
    {
        this.logicNode = logicNode;
        this.dataNode = dataNode;
        this.displayNode = displayNode;

        Data.SetCameraReference(mainCamera);
    }
    public void SetDungeon(Dungeon dungeon)
    {
        this.dungeon = dungeon;
    }


    private void MouseInput()
    {/*
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int adjustedX = (int)((worldPosition.x - (worldPosition.x % displayNode.GridCellSizeX)) / displayNode.GridCellSizeX);

            Vector2Int gridPosition = new Vector2Int(adjustedX, Mathf.FloorToInt(worldPosition.y));

            MapCell cell = dungeon.CellAtPosition(gridPosition);
            if (cell == null) return;

            //Debug.Log(cell.Position);
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            mainCamera.orthographicSize -= Input.mouseScrollDelta.y;
            if (mainCamera.orthographicSize < 10) mainCamera.orthographicSize = 10;
        }*/
    }
    private void KeyboardInput()
    {/*
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.y += scrollSensitivity;
            mainCamera.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.y -= scrollSensitivity;
            mainCamera.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.x -= scrollSensitivity;
            mainCamera.transform.position = newPos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 newPos = mainCamera.transform.position;
            newPos.x += scrollSensitivity;
            mainCamera.transform.position = newPos;
        }*/
    }
    #endregion Methods

    #region Behaviour
    void Start()
    {
        
    }
    void Update()
    {
        MouseInput();
        KeyboardInput();
    }
    #endregion Behaviour
}