using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    #region Data
    [SerializeField] private LogicNode logicNode;

    [SerializeField] private Image gameOverWindow;

    [SerializeField] private InputField inputField;
    [SerializeField] private TMPro.TextMeshProUGUI messageBox;

    [SerializeField] private TMPro.TextMeshProUGUI descriptionBox;
    [SerializeField] private Text equipmentBox;

    [SerializeField] private Text inventoryBoxTitle;
    [SerializeField] private Text inventoryBox;

    [SerializeField] private Text healthBox;

    private bool messageReceived = false;
    #endregion Data


    #region Methods
    private void FocusOnInputField()
    {
        inputField.ActivateInputField();
    }
    private CommandStruct ProcessCommand(string command)
    {
        string processedCommand = command.ToLower();
        processedCommand = processedCommand.Trim();

        int firstSpaceIndex = processedCommand.IndexOf(' ');
        string commandName;
        if (firstSpaceIndex != -1) commandName = processedCommand.Substring(0, processedCommand.IndexOf(' '));
        else commandName = processedCommand;

        processedCommand = Regex.Replace(processedCommand, commandName, "");
        processedCommand = processedCommand.Trim();
        processedCommand = Regex.Replace(processedCommand, "(, )", ",");
        processedCommand = Regex.Replace(processedCommand, "( ,)", ",");

        return new CommandStruct(Commands.GetCommandFunction(commandName), processedCommand);
    }
    private void ExecuteCommand(CommandStruct command)
    {
        if (command.commandFunction == null)
        {
            string message = "Invalid command!";
            Logger.SendMessageToUI(message);
            return;
        }

        bool playerHasActed = command.commandFunction.Invoke(command.commandString);

        if (playerHasActed) Data.SetTurnComplete(true);
    }
    private void ClearInputField()
    {
        inputField.text = string.Empty;
    }


    public void ReceiveMessage(string message)
    {
        messageBox.text += message + "\n";
        messageReceived = true;
    }

    public void ResetMessageBox()
    {
        messageBox.rectTransform.anchoredPosition = new Vector2(0, 0);

        messageBox.text = "<color=lightblue>Type 'help' to get started.</color>\n";
    }
    public void UpdateAllTextBoxes()
    {
        gameOverWindow.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);

        UpdateDescriptionBox();
        UpdateEquipmentBox();
        UpdateInventoryBox();
        UpdateHealthBox();
    }
    public void UpdateTextBox(UIBoxes boxName)
    {
        switch (boxName)
        {
            case UIBoxes.Description: UpdateDescriptionBox(); break;
            case UIBoxes.Equipment: UpdateEquipmentBox(); break;
            case UIBoxes.Inventory: UpdateInventoryBox(); break;
            case UIBoxes.Health: UpdateHealthBox(); break;
            default: break;
        }
    }
    private void UpdateDescriptionBox()
    {
        string description = string.Empty;

        DungeonEntrance entrance = Data.Player.Location.Entrance;
        ItemSlot itemOnGround = Data.Player.Location.Item;
        Inventory corpseInventory = Data.Player.Location.Corpse != null ? Data.Player.Location.Corpse.Inventory : null;
        Inventory chestInventory = Data.Interactables["chest"];

        string stairsDescription = entrance != null && entrance.IsExit && !entrance.Unlocked ? ", but they are locked." : ".";

        description += entrance != null && entrance.IsExit ? $"The <color=yellow>stairs</color> leading to the next level are next to you{stairsDescription}" : "";
        description += entrance != null && entrance.IsEntrance ? $"The <color=yellow>stairs</color> leading to the previous floor are next to you, but they are closed shut." : "";
        description += itemOnGround != null ? $"There is <color=green>{itemOnGround.Amount}</color>x <color=green>{itemOnGround.FullName}</color> on the ground next to you. " : "" ;
        description += corpseInventory != null ? "There is a <color=orange>corpse</color> on the ground next to you. " : "";
        description += chestInventory != null ? "There is a <color=orange>chest</color> on the ground next to you. " : "";


        Vector2Int playerPosition = Data.Player.Position;
        for (int y = -2; y < 3; y++)
        {
            for (int x = -2; x < 3; x++)
            {
                MapCell cell = Data.Dungeon.CellAtPosition(playerPosition.x + x, playerPosition.y + y);
                if (cell == null) continue;

                if (cell.HasAgent)
                {
                    string direction = string.Empty;
                    Vector2Int positionDifference = cell.Position - playerPosition;

                    direction += positionDifference.y > 0 ? "north" : positionDifference.y < 0 ? "south" : "";
                    direction += positionDifference.x > 0 ? "east" : positionDifference.x < 0 ? "west" : "";

                    description += $"There is a hostile <color=red>{cell.Agent.Species.Name}</color> to the <color=yellow>{direction}</color> of you. ";
                }
            }
        }

        if (description == "") description = "There is nothing noteworthy nearby.";

        descriptionBox.text = description.Trim();
    }
    private void UpdateEquipmentBox()
    {
        int i = 1;
        string text = string.Empty;
        foreach (EquipmentSlot slot in Data.Player.Equipment.Slots.Keys)
        {
            text += $"{i}. {slot} -> ";
            Item equippedItem = Data.Player.Equipment.Slots[slot];
            if (equippedItem != null) text += equippedItem.FullName;
            //else text += "Nothing";

            text += "\n";
            i++;
        }
        equipmentBox.text = text;
    }
    private void UpdateInventoryBox()
    {
        inventoryBoxTitle.text = $"Inventory: {Data.Player.Inventory.StoredItems.Count}/{Data.Player.Inventory.ItemSlots}";

        int i = 1;
        string text = string.Empty;
        foreach (ItemSlot itemSlot in Data.Player.Inventory.StoredItems)
        {
            text += $"{i}. x{itemSlot.Amount} {itemSlot.FullName}";

            text += "\n";
            i++;
        }
        inventoryBox.text = text;
    }
    private void UpdateHealthBox()
    {
        int currentHealth = Data.Player.HealthCurrent;
        int maxHealth = Data.Player.HealthMax;

        string text = $"Health: {currentHealth}/{maxHealth}";
        healthBox.text = text;

        if (currentHealth > maxHealth * 0.6) healthBox.color = Color.green;
        else if (currentHealth >= maxHealth * 0.25) healthBox.color = Data.ColourDict[ColourEnum.Orange];
        else healthBox.color = Color.red;
    }

    private void UpdateMessageBoxScrollPosition()
    {
        float hDelta = messageBox.rectTransform.sizeDelta.y;

        if (hDelta > 0)
        {
            Vector3 currentPosition = messageBox.rectTransform.anchoredPosition;
            messageBox.rectTransform.anchoredPosition = new Vector2(currentPosition.x, hDelta);
        }
    }


    public void ActivateGameOverWindow()
    {
        inputField.gameObject.SetActive(false);
        gameOverWindow.gameObject.SetActive(true);
    }
    public void GameOverButtonYes() => logicNode.StartGame();
    public void GameOverButtonNo()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }


    private void KeyboardInput()
    {
        // Move
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            CommandStruct command = ProcessCommand("go north");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            CommandStruct command = ProcessCommand("go northeast");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            CommandStruct command = ProcessCommand("go east");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            CommandStruct command = ProcessCommand("go southeast");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            CommandStruct command = ProcessCommand("go south");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            CommandStruct command = ProcessCommand("go southwest");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            CommandStruct command = ProcessCommand("go west");
            ExecuteCommand(command);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            CommandStruct command = ProcessCommand("go northwest");
            ExecuteCommand(command);
        }

        // Attack target
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            CommandStruct command = ProcessCommand("attack");
            ExecuteCommand(command);
        }
    }
    #endregion Methods

    #region Behavior
    void Start()
    {
    }
    void Update()
    {
        if (messageReceived)
        {
            UpdateMessageBoxScrollPosition();
            messageReceived = false;
        }

        if (!Data.Player.Alive) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            FocusOnInputField();
            if (inputField.text.Length > 0)
            {
                CommandStruct command = ProcessCommand(inputField.text);
                ExecuteCommand(command);
                ClearInputField();
            }
        }

        else KeyboardInput();
    }
    #endregion Behavior
}


public enum UIBoxes : byte { Description, Equipment, Inventory, Health }