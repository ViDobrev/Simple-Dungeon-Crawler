using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public static class Commands
{
    #region Data
    private static Dictionary<string, CommandFunction> commands = new Dictionary<string, CommandFunction>()
    {
        {"move", Move}, {"go", Move},
        {"pick", Pick}, {"get", Pick}, {"grab", Pick}, {"take", Pick},
        {"equip", Equip}, {"unequip", Unequip},
        {"drop", Drop},
        {"target", Target},
        {"attack", Attack},
        {"open", Open}, {"close", Close},
        {"inspect", Inspect},
        {"climb", Climb},
        {"eat", Eat},
        {"increase", IncreaseStat},
        {"help", Help}
    };

    private static Dictionary<string, Vector2Int> moveDirections = new Dictionary<string, Vector2Int>()
    {
        {"north", Vector2Int.up},              {"n",  Vector2Int.up},               // N
        {"northeast", new Vector2Int(1, 1)},   {"ne", new Vector2Int(1, 1)},        // NE
        {"east", Vector2Int.right},            {"e",  Vector2Int.right},            // E
        {"southeast", new Vector2Int(1, -1)},  {"se", new Vector2Int(1, -1)},       // SE
        {"south", Vector2Int.down},            {"s",  Vector2Int.down},             // S
        {"southwest", new Vector2Int(-1, -1)}, {"sw", new Vector2Int(-1, -1)},      // SW
        {"west", Vector2Int.left},             {"w",  Vector2Int.left},             // W
        {"northwest", new Vector2Int(-1, 1)},  {"nw", new Vector2Int(-1, 1)}        // NW
    };
    private static Dictionary<string, string> directionNames = new Dictionary<string, string>()
    {
        {"north", "north"},         {"n",  "north"},            // N
        {"northeast", "northeast"}, {"ne", "northeast"},        // NE
        {"east", "east"},           {"e",  "east"},             // E
        {"southeast", "southeast"}, {"se", "southeast"},        // SE
        {"south", "south"},         {"s",  "south"},            // S
        {"southwest", "southwest"}, {"sw", "southwest"},        // SW
        {"west", "west"},           {"w",  "west"},             // W
        {"northwest", "northwest"}, {"nw", "northwest"}         // NW
    };
    #endregion Data

    #region Properties
    #endregion Properties


    #region Methods
    public static CommandFunction GetCommandFunction(string command)
    {
        if (commands == null || !commands.ContainsKey(command)) return null;

        return commands[command];
    }



    private static bool Move(string command)
    {// Example: 'north', 'down'
        string direction = command;

        if (command == "down" || command == "up") return Climb(command); //go down/up, means go down/up the stairs to the next dungeon floor

        if (!moveDirections.ContainsKey(direction))
        {
            Logger.SendMessageToUI($"Invalid move direction ({direction}). Type \"help move\" for command information.");
            return false;
        }

        direction = directionNames[direction];
        if (!Data.Player.Move(moveDirections[direction]))
        {
            Logger.SendMessageToUI($"Cannot move <color=yellow>{direction}</color>. Spot is occupied/intraversible.");
            return false;
        }

        Logger.SendMessageToUI($"You move to the <color=yellow>{direction}</color>.");
        Data.CentreCameraOnPlayer();
        Data.UpdateInteractableObjects();
        return true;
    }
    private static bool Pick_old(string command)
    {// Example: '1 wooden sword from chest', '1 3 from chest'
        // amount name/index from container

        //TODO make this better
        int amount;
        string itemName;
        int itemIndex;
        string containerName = null;

        // Check command syntax
        if (Regex.IsMatch(command, "^([0-9])+ (([0-9])|([a-z\\s]))+( from )([a-z])+$")) // amount name/index from container
        {
            amount = int.Parse(Regex.Replace(command, " (.)*$", ""));
            itemName = Regex.Replace(command, "^([0-9])+ ", "");
            itemName = Regex.Replace(itemName, "( from )(.)*$", "");
            if (!int.TryParse(itemName, out itemIndex)) itemIndex = -1;
            containerName = Regex.Replace(command, "^(.)*(from )", "");
        }
        else if (Regex.IsMatch(command, "^(([0-9])|([a-z\\s]))+( from )([a-z])+$")) // name/index from container
        {
            amount = 1;
            itemName = Regex.Replace(command, "( from)(.)*$", "");
            if (!int.TryParse(itemName, out itemIndex)) itemIndex = -1;
            containerName = Regex.Replace(command, "^(.)*(from )", "");
        }
        else if (Regex.IsMatch(command, "^([0-9])+ (([0-9])|([a-z\\s]))+$")) // amount name/index
        {
            amount = int.Parse(Regex.Replace(command, " (.)*$", ""));
            itemName = Regex.Replace(command, "^([0-9])+ ", "");
            if (!int.TryParse(itemName, out itemIndex)) itemIndex = -1;
        }
        else if (Regex.IsMatch(command, "^([0-9a-z\\s])+$")) // name/index
        {
            amount = 1;
            itemName = command;
            if (!int.TryParse(itemName, out itemIndex)) itemIndex = -1;
        }
        else
        {
            Logger.SendMessageToUI("Invalid pick command arguments. Type \"help pick\" for command information.");
            return false;
        }

        if (Data.Player.Inventory.IsFull)
        {
            Logger.SendMessageToUI("Not enough space in your inventory.");
            return false;
        }

        ItemSlot item;
        if (containerName == null)
        {
            ItemSlot itemOnGround = Data.Player.Location.Item;
            if (itemIndex > -1 && itemIndex != 0)
            {
                Logger.SendMessageToUI($"There is no item at index {itemIndex} on the ground at your location.");
                return false;
            }
            else if (itemOnGround == null || itemOnGround.FullName.ToLower() != itemName)
            {
                Logger.SendMessageToUI($"There is no {itemName} on the ground at your location");
                return false;
            }

            item = Data.Player.Location.GetItem(amount);
            if (item == null)
            {
                Logger.SendMessageToUI($"The requested amount is more than what is available.");
                return false;
            }
        }
        else
        {
            Inventory inventory = Data.Interactables[containerName];
            if (inventory == null)
            {
                Logger.SendMessageToUI($"No <color=orange>{containerName}</color> could be found at your location.");
                return false;
            }
            if (itemIndex > -1) item = inventory.RemoveItem(itemIndex - 1, amount);
            else item = inventory.RemoveItem(itemName, amount);

            if (item == null)
            {
                string indexMessage = string.Empty;
                if (itemIndex > -1) indexMessage = $"at index";
                Logger.SendMessageToUI($"Item {indexMessage}(<color=green>{itemName}</color>) could not be found in <color=orange>{inventory.ParentObject.Name}</color>, or requested amount is more than what is available.");
                return false;
            }
        }

        Data.Player.Inventory.PlaceItem(item);
        string containerAddition = containerName != null ? $"from the <color=orange>{containerName}</color>" : "" ;
        Logger.SendMessageToUI($"You pick the <color=green>{item.Name}</color> {containerAddition} and place it int your inventory.");
        Data.UIController.UpdateTextBox(UIBoxes.Inventory);
        return true;
    }
    private static bool Pick(string command)
    {// Example: '1 wooden sword'
        int amount;
        string itemName;
        string itemOriginMessage = string.Empty;

        // Syntax check
        if (Regex.IsMatch(command, "^([0-9])+ ([a-z\\s])+$"))
        {
            amount = int.Parse(Regex.Replace(command, " (.)*$", ""));
            itemName = Regex.Replace(command, "^(.)* ", "");
        }
        else if (Regex.IsMatch(command, "^([a-z\\s])+$"))
        {
            amount = 1;
            itemName = command;
        }
        else
        {
            Logger.SendMessageToUI("Invalid pick command arguments. Type \"help pick\" for command information.");
            return false;
        }

        if (Data.Player.Inventory.IsFull)
        {
            Logger.SendMessageToUI("Your inventory is full.");
            return false;
        }

        MapCell location = Data.Player.Location;
        ItemSlot requestedItem = null;
        Inventory inventory = null;
        ItemSlot itemOnGround = location.Item;

        if (itemOnGround != null)
        {
            if (itemOnGround.FullName.ToLower() == itemName)
            {
                requestedItem = location.GetItem(amount);
                itemOriginMessage = ", from the ground,";
            }
        }
        else
        {
            if (location.HasCorpse) inventory = location.Corpse.Inventory;
            else if (location.HasStructure && location.Structure.HasInventory && location.Structure.Opened) inventory = location.Structure.Inventory;

            if (inventory != null)
            {
                requestedItem = inventory.RemoveItem(itemName, amount);
                itemOriginMessage = $", from the {inventory.ParentObject.Name},";
            }
        }

        if (requestedItem == null)
        {
            Logger.SendMessageToUI($"The {itemName} could not be found at your location, or you requested an amount more than what is available.");
            return false;
        }




        Data.Player.Inventory.PlaceItem(requestedItem);
        Logger.SendMessageToUI($"You pick the <color=green>{requestedItem.FullName}</color>{itemOriginMessage} and place it int your inventory.");
        Data.UIController.UpdateTextBox(UIBoxes.Inventory);
        return true;
    }
    private static bool Equip(string command)
    {// Example: 'wooden sword', '2'
        string commandType = string.Empty;
        if (Regex.IsMatch(command, "^([a-z\\s])+$")) commandType = "name";
        else if (Regex.IsMatch(command, "^([0-9])+$")) commandType = "index";
        else Logger.SendMessageToUI("Invalid equip command argument. Type \"help equip\" for command information.");


        ItemSlot itemToEquip = commandType == "name" ? Data.Player.Inventory.PeekItem(command, 1) : Data.Player.Inventory.PeekItem(int.Parse(command)-1, 1);
        if (itemToEquip == null)
        {
            Logger.SendMessageToUI($"Item (<color=green>{command}</color>) could not be found in your inventory.");
            return false;
        }

        Item replacedItem;
        if (itemToEquip.Item.GetType() == typeof(Weapon)) replacedItem = Data.Player.Equipment[(itemToEquip.Item as Weapon).Slot];
        else if (itemToEquip.Item.GetType() == typeof(Armour)) replacedItem = Data.Player.Equipment[(itemToEquip.Item as Armour).Slot];
        else
        {
            Logger.SendMessageToUI($"The {itemToEquip.FullName} is not an equippable item.");
            return false;
        }

        Data.Player.Inventory.RemoveItem(itemToEquip);
        Data.Player.Equipment.EquipItem(itemToEquip.Item);
        string replacingMessage = replacedItem == null ? "." : $", replacing your <color=green>{replacedItem.FullName}</color> and putting it in your inventory.";
        Logger.SendMessageToUI($"You equip the <color=green>{itemToEquip.FullName}</color>{replacingMessage}");

        Data.UIController.UpdateTextBox(UIBoxes.Equipment);
        Data.UIController.UpdateTextBox(UIBoxes.Inventory);
        return true;
    }
    private static bool Unequip(string command)
    {// Example: 'wooden sword', '2'
        string commandType = string.Empty;
        if (Regex.IsMatch(command, "^([a-z\\s])+$")) commandType = "name";
        else if (Regex.IsMatch(command, "^([0-9])+$")) commandType = "index";
        else Logger.SendMessageToUI("Invalid unequip command argument. Type \"help unequip\" for command information.");

        if (Data.Player.Inventory.IsFull)
        {
            Logger.SendMessageToUI("Your inventory is full.");
            return false;
        }

        Item unequippedItem = commandType == "name" ? Data.Player.Equipment.UnequipItem(command) : Data.Player.Equipment.UnequipItem(int.Parse(command)-1);
        if (unequippedItem == null)
        {
            Logger.SendMessageToUI("Your inventory is full.");
            return false;
        }

        Data.Player.Inventory.PlaceItem(unequippedItem);
        Logger.SendMessageToUI($"You unequip your <color=green>{unequippedItem.FullName}</color> and place it in your inventory.");

        Data.UIController.UpdateTextBox(UIBoxes.Equipment);
        Data.UIController.UpdateTextBox(UIBoxes.Inventory);
        return true;
    }
    private static bool Drop(string command)
    {// Example: '1 wooden sword in chest'
        string itemName;
        int amount;
        string containerName = null;
        if (Regex.IsMatch(command, "^([0-9])+ ([a-z\\s])+( in )([a-z])+$")) //'1 wooden sword in chest'
        {
            itemName = Regex.Replace(command, "^([0-9])+ ", "");
            itemName = Regex.Replace(itemName, "( in )(.)*$", "");
            amount = int.Parse(Regex.Replace(command, " (.)*$", ""));
            containerName = Regex.Replace(command, "^(.)*(in )", "");
        }
        else if (Regex.IsMatch(command, "^([a-z\\s])+( in )([a-z])+$")) //'wooden sword in chest'
        {
            itemName = Regex.Replace(command, "( in )(.)*$", "");
            amount = 1;
            containerName = Regex.Replace(command, "^(.)*(in )", "");
        }
        else if (Regex.IsMatch(command, "^([0-9])+ ([a-z\\s])+$")) //'1 wooden sword'
        {
            itemName = Regex.Replace(command, "^([0-9])+ ", "");
            amount = int.Parse(Regex.Replace(command, " (.)*$", ""));
        }
        else if (Regex.IsMatch(command, "^([a-z\\s])+$")) //'wooden sword'
        {
            itemName = command;
            amount = 1;
        }
        else
        {
            Logger.SendMessageToUI("Invalid drop command arguments. Type \"help drop\" for command information.");
            return false;
        }

        ItemSlot itemToDrop = Data.Player.Inventory.PeekItem(itemName, amount);
        if (itemToDrop == null)
        {
            Logger.SendMessageToUI($"Item (<color=green>{itemName}</color>) could not be found in your inventory.");
            return false;
        }

        if (containerName == null)
        {
            if (!Data.Player.Location.AddItem(itemToDrop))
            {
                Logger.SendMessageToUI("You can't drop an item at this location, as it is already occupied.");
                return false;
            }
        }
        else
        {
            Inventory inventory = Data.Interactables[containerName];
            if (inventory == null)
            {
                Logger.SendMessageToUI($"No <color=orange>{containerName}</color> could be found at your location.");
                return false;
            }
            if (inventory.IsFull)
            {
                Logger.SendMessageToUI("Specified container is already full.");
                return false;
            }

            inventory.PlaceItem(itemToDrop);
        }

        Data.Player.Inventory.RemoveItem(itemToDrop);
        string containerString = containerName == null ? "on the ground" : $"inside the <color=orange>{containerName}</color>";
        Logger.SendMessageToUI($"You drop <color=green>{amount}</color> of your <color=green>{itemToDrop.FullName}</color> {containerString}");
        Data.UIController.UpdateTextBox(UIBoxes.Inventory);
        return true;
    }
    private static bool Target(string command)
    {// Example: 'north'

        MapCell cellWithEnemy;
        Agent enemy;
        string direction = command;
        if (!moveDirections.ContainsKey(direction))
        {
            Logger.SendMessageToUI($"Invalid direction ({direction}). Type \"help target\" for command information.");
            return false;
        }

        direction = directionNames[direction];

        cellWithEnemy = Data.Dungeon.CellAtPosition(Data.Player.Position + moveDirections[direction]);
        if (cellWithEnemy == null)
        {
            Logger.SendMessageToUI($"There is nothing to the <color=yellow{direction}</color> of you.");
            return false;
        }

        enemy = cellWithEnemy.Agent;
        if (enemy == null)
        {
            Logger.SendMessageToUI($"There is no enemy to the <color=yellow>{direction}</color> of you.");
            return false;
        }

        Data.Player.SetTarget(enemy);
        Logger.SendMessageToUI($"You target the <color=red>{enemy.Species.Name}</color> to the <color=yellow>{direction}</color> of you.");
        return false;
    }
    private static bool Attack(string command)
    {// Example: 'north', ''
        if (command == "") return AttackTarget();

        MapCell cellWithEnemy;
        Agent enemy;
        string direction = command;
        if (!moveDirections.ContainsKey(direction))
        {
            Logger.SendMessageToUI($"Invalid attack direction ({direction}). Type \"help attack\" for command information.");
            return false;
        }

        direction = directionNames[direction];

        cellWithEnemy = Data.Dungeon.CellAtPosition(Data.Player.Position + moveDirections[direction]);
        if (cellWithEnemy == null)
        {
            Logger.SendMessageToUI($"There is nothing to the <color=yellow>{direction}</color> of you.");
            return false;
        }

        enemy = cellWithEnemy.Agent;
        if (enemy == null)
        {
            Logger.SendMessageToUI($"There is no enemy to the <color=yellow>{direction}</color> of you.");
            return false;
        }

        InteractionHandler.Attack(Data.Player, enemy);
        return true;
    }
    private static bool Open(string command)
    {// Example: 'chest'
        string structureName = command;
        if (!Regex.IsMatch(command, "^([a-z\\s])+$"))
        {
            Logger.SendMessageToUI($"There is no <color=orange>{structureName}</color> at your location.");
            return false;
        }

        Structure openableStructure = Data.Interactables.GetOpenableStructure(structureName);
        if (openableStructure == null)
        {
            Logger.SendMessageToUI($"There is no <color=orange>{structureName}</color> at your location.");
            return false;
        }

        if (!openableStructure.Open())
        {
            Logger.SendMessageToUI($"The <color=orange>{openableStructure.Name}</color> is either already opened or cannot be opened at all.");
            return false;
        }

        Logger.SendMessageToUI($"You open the <color=green>{openableStructure.Name}</color>.");
        return true;
    }
    private static bool Close(string command)
    {// Example: 'chest'
        string structureName = command;
        if (!Regex.IsMatch(command, "^([a-z\\s])+"))
        {
            Logger.SendMessageToUI($"There is no <color=orange>{structureName}</color> at your location.");
            return false;
        }

        Structure openableStructure = Data.Interactables.GetOpenableStructure(structureName);
        if (openableStructure == null)
        {
            Logger.SendMessageToUI($"There is no <color=orange>{structureName}</color> at your location.");
            return false;
        }

        if (!openableStructure.Close())
        {
            Logger.SendMessageToUI($"The <color=orange>{openableStructure.Name}</color> is either already closed or cannot be closed at all.");
            return false;
        }

        Logger.SendMessageToUI($"You close the <color=orange>{openableStructure.Name}</color>.");
        return true;
    }
    private static bool Inspect(string command)
    {// Example: 'chest', 'wooden sword', 'wooden sword from chest', 'self'
        // TODO
        if (!Regex.IsMatch(command, "^([a-z\\s])+$"))
        {
            Logger.SendMessageToUI("Invalid inspect command argument. Type \"help inspect\" for command information.");
            return false;
        }

        string argument = command;
        if (argument == "self") return InspectSelf();

        Inventory inventory = Data.Interactables[argument];
        if (inventory != null) return InspectContainer(argument, inventory);

        string itemName = Regex.Replace(argument, "(( from)|( in))(.)*$", "");
        string containerName = Regex.Replace(argument, "^(.)*((from)|(in)) ", "");

        if (itemName == containerName) containerName = null;
        inventory = Data.Interactables[containerName];
        Item itemToInspect;
        if (inventory == null)
        {
            ItemSlot _item = Data.Player.Inventory.PeekItem(itemName, 1);
            itemToInspect = _item != null ? _item.Item : null;
            if (_item == null)
            {
                foreach (EquipmentSlot slot in Data.Player.Equipment.Slots.Keys)
                {
                    if (Data.Player.Equipment[slot] != null && Data.Player.Equipment[slot].FullName.ToLower() == itemName)
                    {
                        itemToInspect = Data.Player.Equipment[slot];
                        break;
                    }
                }
            }
        }
        else
        {
            ItemSlot _item = inventory.PeekItem(itemName, 1);
            itemToInspect = _item != null ? _item.Item : null;
        }

        if (itemToInspect != null) return InspectItem(itemToInspect);


        Logger.SendMessageToUI($"No {argument} was found.");
        return false;
    }
    private static bool Climb(string command)
    {// Example: 'down'
        if (command != "down" && command != "up")
        {
            Logger.SendMessageToUI("Invalid climb command argument. Type \"help climb\" for command information.");
            return false;
        }


        DungeonEntrance stairs = Data.Player.Location.Entrance;
        
        if (command == "up")
        {
            if (stairs == null || !stairs.IsEntrance)
            {
                Logger.SendMessageToUI("There are no stairs going up at your location.");
                return false;
            }

            Logger.SendMessageToUI("You can't go up the stairs as their entrance is closed shut.");
            return false;
        }


        if (stairs == null || !stairs.IsExit)
        {
            Logger.SendMessageToUI("There are no stairs going down at your location.");
            return false;
        }

        if (!stairs.Unlocked)
        {
            Logger.SendMessageToUI("The entrance to the stairs is locked. Defeating this floor's boss will unlock it.");
            return false;
        }

        Logger.SendMessageToUI("You climb down the stairs to the next dungeon floor.");
        Data.SetMustGenerateNextDungeonLevel();
        return true;
    }
    private static bool Eat(string command)
    {// Example: 'bread', '2'
        string commandType = string.Empty;
        if (Regex.IsMatch(command, "^([a-z\\s])+$")) commandType = "name";
        else if (Regex.IsMatch(command, "^([0-9])+$")) commandType = "index";
        else Logger.SendMessageToUI("Invalid eat command argument. Type \"help eat\" for command information.");

        string foodName = command;
        ItemSlot foodItemSlot = commandType == "name" ? Data.Player.Inventory.RemoveItem(foodName, 1) : Data.Player.Inventory.RemoveItem(int.Parse(command)-1, 1);
        if (foodItemSlot == null)
        {
            Logger.SendMessageToUI($"There is no <color=green>{foodName}</color> in your inventory.");
            return false;
        }

        Item food = foodItemSlot.Item;
        if (food.Tag != ItemTag.Food)
        {
            Logger.SendMessageToUI($"<color=green>{food.FullName}</color> is not edible.");
            return false;
        }

        int healthBeforeHeal = Data.Player.HealthCurrent;
        Data.Player.Heal((food as Food).Nutrition);
        Logger.SendMessageToUI($"You eat the <color=green>{food.FullName}</color> and regain <color=green>{Data.Player.HealthCurrent - healthBeforeHeal}</color> health.");

        Data.UIController.UpdateTextBox(UIBoxes.Health);
        Data.UIController.UpdateTextBox(UIBoxes.Inventory);
        return true;
    }
    private static bool IncreaseStat(string command)
    {// Example: 'strength'
        if (!Enum.TryParse(command, true, out StatName statName))
        {
            Logger.SendMessageToUI($"Invalid stat name ({command}).");
            return false;
        }

        if (!Data.Player.IncreaseStat(statName))
        {
            Logger.SendMessageToUI($"You do not have enough points to increase your <color=yellow>{statName}</color>.");
            return false;
        }


        string messageAddition = $"You have <color=green>{Data.Player.Stats.StatPoints}</color> more stat points to allocate.";
        Logger.SendMessageToUI($"You increase your <color=yellow>{statName}</color> by 1. Your <color=yellow>{statName}</color> is now <color=green>{Data.Player.Stats[statName]}</color>. {messageAddition}");
        Data.UIController.UpdateTextBox(UIBoxes.Health);
        return false;
    }

    private static bool Help(string command)
    {// Example: '', 'move'

        if (!Data.CommandsHelp.ContainsKey(command))
        {
            Logger.SendMessageToUI("There is no command with this name.");
            return false;
        }

        Logger.SendMessageToUI("\n<color=lightblue>"+Data.CommandsHelp[command]+"</color>");
        return false;
    }


    private static bool AttackTarget()
    {
        Agent target = Data.Player.Target;
        if (target == null)
        {
            Logger.SendMessageToUI($"You don't have a target to attack.");
            return false;
        }

        int attackRange = 1;
        Vector2Int distance = Data.Player.Position - target.Position;
        if (Math.Abs(distance.x) > attackRange || Math.Abs(distance.y) > attackRange)
        {
            Logger.SendMessageToUI($"Your target is not within your attack range.");
            return false;
        }

        InteractionHandler.Attack(Data.Player, target);
        return true;
    }

    private static bool InspectSelf()
    {
        string description = Data.Player.Inspect();

        Logger.SendMessageToUI(description);
        return false;
    }
    private static bool InspectContainer(string argument, Inventory inventory)
    {
        if (inventory.ParentObject.GetType() == typeof(Structure) && (inventory.ParentObject as Structure).CanBeOpened && !(inventory.ParentObject as Structure).Opened)
        {
            Logger.SendMessageToUI($"The <color=orange>{argument}</color> at your location is closed. You must open it first.");
            return false;
        }

        string message;
        if (inventory.StoredItems.Count > 0)
            message = $"The <color=orange>{argument}</color> contains: <color=green>{inventory.StoredItems[0].Amount}</color>x <color=green>{inventory.StoredItems[0].FullName}</color>";
        else message = "There is nothing of value on this corpse.";

        for (int i = 1; i < inventory.StoredItems.Count; i++)
        {
            message += $", <color=green>{inventory.StoredItems[i].Amount}</color>x <color=green>{inventory.StoredItems[i].FullName}</color>";
        }

        Logger.SendMessageToUI(message);
        return false;
    }
    private static bool InspectItem(Item itemToInspect)
    {
        string description = itemToInspect.Inspect();

        Logger.SendMessageToUI(description);
        return false;
    }
    #endregion Methods
}


public delegate bool CommandFunction(string command);
public struct CommandStruct
{
    public CommandFunction commandFunction;
    public string commandString;

    public CommandStruct(CommandFunction commandFunction, string commandString)
    {
        this.commandFunction = commandFunction;
        this.commandString = commandString;
    }
}