using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class Data
{
    #region Data
    private static DataNode dataNode;
    private static UIControl uiController;
    private static Camera mainCamera;
    private static Player player;
    private static bool playerHasActed;
    private static bool mustGenerateNextDungeonLevel;

    private static Dungeon dungeon;
    private static Tilemap tilemap;

    private static List<Agent> agentsHolder;
    private static List<Agent> animalsHolder;
    private static List<Plant> plantsHolder;

    private static Interactables interactables;

    public static System.Random mapRng;
    public static System.Random agentRng;
    public static System.Random diceRng;
    public static System.Random mathsRng;
    public static System.Random aiRng;

    private static Dictionary<ColourEnum, Color> colourDict = new Dictionary<ColourEnum, Color>()
    {
        {ColourEnum.White,      Color.white     },
        {ColourEnum.Green,      Color.green     },
        {ColourEnum.Yellow,     Color.yellow    },
        {ColourEnum.Blue,       Color.blue      },
        {ColourEnum.Magenta,    Color.magenta   },
        {ColourEnum.Red,        Color.red       },
        {ColourEnum.Grey,       Color.grey      },
        {ColourEnum.Brown,      new Color(0.59f, 0.29f, 0)},
        {ColourEnum.Orange,     new Color(1f   , 0.55f, 0)}
    };
    private static Dictionary<Quality, string> qualityNames = new Dictionary<Quality, string>()
    {
        {Quality.Awful, "Awful" },
        {Quality.Bad, "Bad" },
        {Quality.Normal, "" },
        {Quality.WellMade, "Well Made" },
        {Quality.Masterwork, "Masterwork" }
    };

    private static Dictionary<string, MyTerrain> terrains;
    private static Dictionary<string, MaterialData> materials;
    private static Dictionary<string, FoodData> foods;
    private static Dictionary<string, StructureData> structures;
    private static Dictionary<string, FloorData> floors;
    private static Dictionary<string, ItemData> items;
    private static Dictionary<string, Species> species;
    private static Dictionary<string, PlantSpecies> plantSpecies;

    private static Dictionary<string, string> commandsHelp;

    private static Dictionary<string, Tile> tiles;

    #endregion Data

    #region Properties
    public static UIControl UIController { get => uiController; }
    public static Player Player { get => player; }
    public static bool PlayerHasActed { get => playerHasActed; }
    public static bool MustGenerateNextDungeonLevel { get => mustGenerateNextDungeonLevel; }

    public static Dungeon Dungeon { get => dungeon; }

    public static List<Agent> Agents { get => agentsHolder; }

    public static Interactables Interactables { get => interactables; }

    public static string PathToXmlDataBase { get => Application.dataPath + @"\Database\XML Database\"; }
    public static string Unknown { get => "Unknown"; }

    public static Dictionary<ColourEnum, Color> ColourDict { get => colourDict; }
    public static Dictionary<Quality, string> QualityNames { get => qualityNames; }

    public static Dictionary<string, MyTerrain> Terrains { get => terrains; }
    public static Dictionary<string, MaterialData> Materials { get => materials; }
    //public static Dictionary<string, FoodData> Foods { get => foods; }
    public static Dictionary<string, StructureData> Structures { get => structures; }
    public static Dictionary<string, FloorData> Floors { get => floors; }
    public static Dictionary<string, ItemData> Items { get => items; }
    public static Dictionary<string, Species> Species { get => species; }
    //public static Dictionary<string, PlantSpecies> PlantSpecies { get => plantSpecies;}

    public static Dictionary<string, string> CommandsHelp { get => commandsHelp; }

    public static Dictionary<string, Tile> Tiles { get => tiles; }
    #endregion Properties


    #region Methods
    public static void Initialize(int mainSeed, TileManager tileManager, DataNode _dataNode, UIControl _uiController)
    {
        dataNode = _dataNode;
        uiController = _uiController;

        System.Random initialRng = new System.Random(mainSeed);
        mapRng = new System.Random(initialRng.Next());
        agentRng = new System.Random(initialRng.Next());
        diceRng = new System.Random(initialRng.Next());
        mathsRng = new System.Random(initialRng.Next());
        aiRng = new System.Random();

        agentsHolder = new List<Agent>();
        animalsHolder = new List<Agent>();
        plantsHolder = new List<Plant>();

        interactables = new Interactables();

        LoadAssets(tileManager);
    }
    public static void SetCameraReference(Camera _mainCamera)
    {
        mainCamera = _mainCamera;
    }
    public static void SetDungeon(Dungeon _dungeon)
    {
        dungeon = _dungeon;
        mustGenerateNextDungeonLevel = false;
    }
    public static void SetPlayerReference(Player playerRef)
    {
        player = playerRef;
        playerHasActed = false;
    }
    public static void SetTilemap(Tilemap _tilemap)
    {
        tilemap = _tilemap;
    }
    public static void LoadAssets(TileManager tileManager)
    {
        LoadTiles(tileManager);
        LoadTerrains();
        LoadMaterials();
        LoadFoods();
        LoadItems();
        LoadStructures();
        LoadFloors();
        LoadSpecies();
        LoadPlantSpecies();

        LoadHelpMessages();
    }


    private static void LoadTiles(TileManager tileManager)
    {
        tiles = new Dictionary<string, Tile>();

        foreach (var tilePackArray in tileManager.tilePacks)
        {
            foreach (var tilePack in tilePackArray.tilePacks)
            {
                tiles.Add(tilePack.name, tilePack.tile);
            }
        }
    }
    private static void LoadTerrains()
    {
        terrains = new Dictionary<string, MyTerrain>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Terrains.xml");

        foreach (var terrain in xmlString.Descendants("Terrain"))
        {
            string name = terrain.Descendants("Name").First().Value;
            bool traversible = terrain.Descendants("Traversible").First().Value == "True";
            if (!Enum.TryParse(terrain.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Terrain ({name}). Colour could not be parsed.");
            }

            terrains.Add(name, new MyTerrain(name, traversible, tiles[name], colourDict[colour]));
        }
    }
    private static void LoadMaterials()
    {
        materials = new Dictionary<string, MaterialData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Materials.xml");

        foreach (var material in xmlString.Descendants("Material"))
        {
            string name = material.Descendants("Name").First().Value;
            string[] types = material.Descendants("Type").First().Value.Split(' ');
            bool canBeUsedForWeapon = types.Contains("Weapon");
            bool canBeUsedForArmour = types.Contains("Armour");
            string adjective = material.Descendants("Adjective").First().Value;
            if (!int.TryParse(material.Descendants("Tier").First().Value, out int tier))
            {
                Debug.LogError($"Material ({name}). Tier could not be parsed.");
            }
            if (!int.TryParse(material.Descendants("Modifier").First().Value, out int modifier))
            {
                Debug.LogError($"Material ({name}). Modifier could not be parsed.");
            }

            materials.Add(name, new MaterialData(name, adjective, tier, modifier, canBeUsedForWeapon, canBeUsedForArmour));
        }
    }
    private static void LoadFoods()
    {
        foods = new Dictionary<string, FoodData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Foods.xml");

        foreach (var food in xmlString.Descendants("Food"))
        {
            string name = food.Descendants("Name").First().Value;
            if (!Enum.TryParse(food.Descendants("Type").First().Value, out FoodType type))
            {
                Debug.LogError($"Food ({name}). Type could not be parsed.");
            }
            if (!int.TryParse(food.Descendants("Nutrition").First().Value, out int nutrition))
            {
                Debug.LogError($"Food ({name}). Nutrition could not be parsed.");
            }

            foods.Add(name, new FoodData(nutrition, type));
        }
    }
    private static void LoadItems()
    {// TODO DONT LIKE THIS FUNCTION
        items = new Dictionary<string, ItemData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Items.xml");

        foreach (var item in xmlString.Descendants("Item"))
        {
            string name = item.Descendants("Name").First().Value;
            bool stackable = item.Descendants("Stackable").First().Value == "True";

            // All data for a food item is acquired. If the item is food load it now and continue onto next item
            if (foods.ContainsKey(name))
            {
                items.Add(name, new ItemData(name, stackable, foods[name]));
                continue;
            }

            if (!Enum.TryParse(item.Descendants("Tag").First().Value, out ItemTag tag))
            {
                Debug.LogError($"Item ({name}). Tag could not be parsed.");
            }

            string attack = null;
            var attackNodes = item.Descendants("Attack").ToList();
            if (attackNodes.Count > 0) attack = attackNodes[0].Value;

            int defence = 0;
            var defenceNodes = item.Descendants("Defence").ToList();
            if (defenceNodes.Count > 0)
            {
                if (!int.TryParse(defenceNodes.First().Value, out defence))
                {
                    Debug.LogError($"Item ({name}). Defence could not be parsed.");
                }
            }

            bool twoHanded = false;
            if (item.Descendants("TwoHanded").ToList().Count > 0)
                twoHanded = item.Descendants("TwoHanded").First().Value == "True";

            if (!Enum.TryParse(item.Descendants("EquipmentSlot").First().Value, out EquipmentSlot equipmentSlot))
            {
                Debug.LogError($"Item ({name}). EquipmentSlot could not be parsed.");
            }

            // Load the item data
            items.Add(name, new ItemData(name, tag, stackable, attack, defence, twoHanded, equipmentSlot));
        }
    }
    private static void LoadStructures()
    {
        structures = new Dictionary<string, StructureData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Structures.xml");

        foreach (var structure in xmlString.Descendants("Structure"))
        {
            string name = structure.Descendants("Name").First().Value;
            StructureTag tag = GetXmlValue<StructureTag>(structure, "Tag", "Structure", name);
            int inventorySlots = GetXmlValue<int>(structure, "InventorySlots", "Structure", name);
            bool canBeOpened = GetXmlValue<bool>(structure, "CanBeOpened", "Structure", name);
            ColourEnum colour = GetXmlValue<ColourEnum>(structure, "Colour", "Structure", name);

            // Load the structure data
            structures.Add(name, new StructureData(name, tag, inventorySlots, canBeOpened, colourDict[colour], tiles[name]));
        }
    }
    private static void LoadFloors()
    {
        floors = new Dictionary<string, FloorData>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Floors.xml");

        foreach (var floor in xmlString.Descendants("Floor"))
        {
            string name = floor.Descendants("Name").First().Value;
            if (!Enum.TryParse(floor.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Floor ({name}). Colour could not be parsed.");
            }

            // Load the floor data
            floors.Add(name, new FloorData(name, colourDict[colour], tiles[name]));
        }
    }
    private static void LoadSpecies()
    {
        species = new Dictionary<string, Species>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Species.xml");

        foreach (var speciesNode in xmlString.Descendants("Species"))
        {
            string name = speciesNode.Descendants("Name").First().Value;
            bool intelligent = speciesNode.Descendants("Intelligent").First().Value == "True";

            Dictionary<StatName, int> stats = new Dictionary<StatName, int>();
            var statsNode = speciesNode.Descendants("Stats").First();
            foreach (var stat in statsNode.Descendants("Stat"))
            {
                if (!Enum.TryParse(stat.Value, out StatName statName))
                {
                    Debug.LogError($"Species ({name}). StatName could not be parsed.");
                }
                if (!int.TryParse(stat.Attribute("Level").Value, out int level))
                {
                    Debug.LogError($"Species ({name}). Stat {statName} level could not be parsed.");
                }

                stats[statName] = level;
            }

            string unarmedAttackString = speciesNode.Descendants("UnarmedAttack").First().Value;
            Attack unarmedAttack = new Attack(unarmedAttackString, 0);

            species.Add(name, new Species(name, intelligent, stats, unarmedAttack, tiles[name]));
        }
    }
    private static void LoadPlantSpecies()
    {
        plantSpecies = new Dictionary<string, PlantSpecies>();

        var xmlString = XElement.Load(PathToXmlDataBase + "Plant Species.xml");

        foreach (var speciesNode in xmlString.Descendants("Species"))
        {
            string name = speciesNode.Descendants("Name").First().Value;
            if (!int.TryParse(speciesNode.Descendants("Health").First().Value, out int health))
            {
                Debug.LogError($"Plant Species ({name}). Health could not be parsed.");
            }
            if (!float.TryParse(speciesNode.Descendants("GrowingRate").First().Value, out float growingRate))
            {
                Debug.LogError($"Plant Species ({name}). GrowingRate could not be parsed.");
            }
            ItemData resource = items[speciesNode.Descendants("Resource").First().Value];
            if (!Enum.TryParse(speciesNode.Descendants("GatherMethod").First().Value, out GatherMethod gatherMethod))
            {
                Debug.LogError($"Plant Species ({name}). GatherMethod could not be parsed.");
            }

            Dictionary<PlantStage, int> yieldByStage = new Dictionary<PlantStage, int>();
            foreach (var yieldNode in speciesNode.Descendants("Yield"))
            {
                if (!Enum.TryParse(yieldNode.Attribute("Stage").Value, out PlantStage plantStageName))
                {
                    Debug.LogError($"Plant Species ({name}). LifeCycle. Name could not be parsed.");
                }
                if (!int.TryParse(yieldNode.Value, out int _yield))
                {
                    Debug.LogError($"Plant Species ({name}). LifeCycle. Yield could not be parsed.");
                }

                yieldByStage.Add(plantStageName, _yield);
            }

            if (!Enum.TryParse(speciesNode.Descendants("Colour").First().Value, out ColourEnum colour))
            {
                Debug.LogError($"Plant Species ({name}). Colour could not be parsed.");
            }

            plantSpecies.Add(name, new PlantSpecies(name, health, growingRate, resource, gatherMethod, yieldByStage, colourDict[colour], tiles[name]));
        }
    }
    private static void LoadHelpMessages()
    {
        commandsHelp = new Dictionary<string, string>();

        var xmlString = XElement.Load(PathToXmlDataBase + "help.xml");

        foreach (var commandNode in xmlString.Descendants("Command"))
        {
            string name = commandNode.Descendants("Name").First().Value.ToLower();
            string description = commandNode.Descendants("Description").First().Value;
            description = System.Text.RegularExpressions.Regex.Replace(description, "(\t)", "");
            description = System.Text.RegularExpressions.Regex.Replace(description, "boss", "<color=orange>boss</color>");
            description = System.Text.RegularExpressions.Regex.Replace(description, "stairs", "<color=yellow>stairs</color>");

            commandsHelp[name] = description;
        }
    }
    


    private static T GetXmlValue<T>(XElement mainNode, string valueName, string objectType, string objectName)
    {
        var targetNode = mainNode.Descendants(valueName).FirstOrDefault();
        if (targetNode == null) return default(T);

        try
        {
            T returnValue;
            if (typeof(T).GetTypeInfo().IsEnum)
                returnValue = (T)Enum.Parse(typeof(T), targetNode.Value);
            else
                returnValue = (T)Convert.ChangeType(targetNode.Value, typeof(T));
            return returnValue;
        }
        catch
        {
            Console.WriteLine($"{objectType} ({objectName}). {valueName} could not be parsed.");
            return default(T);
        }
    }



    public static void AddAgent(Agent agent)
    {
        agentsHolder.Add(agent);
    }
    public static void AddPlant(Plant plant)
    {
        plantsHolder.Add(plant);
    }

    public static void RemoveAgent(Agent agent)
    {
        agentsHolder.Remove(agent);
    }
    public static void RemovePlant(Plant plant)
    {
        plantsHolder.Remove(plant);
    }


    public static void UpdateInteractableObjects()
    {
        interactables.Update();
    }



    public static Tile GetTile(MapCell cell)
    {
        if (!cell.Visible) return tiles["Unknown"];

        Tile tile = cell.Tile;
        tile.color = cell.Colour;

        return tile;
    }
    public static void UpdateTile(MapCell cell)
    {
        if (tilemap == null) return;

        Vector3Int position = new Vector3Int(cell.Position.x, cell.Position.y, 0);
        Tile tile = GetTile(cell);
        tilemap.SetTile(position, tile);
    }

    public static void CentreCameraOnPlayer()
    {// TODO Must do this dynamically
        float xOffset = 4.3425f;
        float yOffset = 3.5777f;

        mainCamera.transform.position = new Vector3(player.Position.x + xOffset, player.Position.y - yOffset, mainCamera.transform.position.z);
    }

    public static void SetTurnComplete(bool turnComplete)
    {
        playerHasActed = turnComplete;
    }
    public static void SetMustGenerateNextDungeonLevel()
    {
        mustGenerateNextDungeonLevel = true;
    }
    #endregion Methods
}



public enum ColourEnum: byte { White, Green, Yellow, Blue, Magenta, Red, Grey, Brown, Orange }