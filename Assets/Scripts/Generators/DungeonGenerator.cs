using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DungeonGenerator
{
    #region Methods
    public static Dungeon GenerateDungeon(DataNode data)
    {
        DungeonParameters parameters = data.DungeonParameters;

        System.Random rng;
        #if UNITY_EDITOR
        rng = new System.Random(data.MainSeed);
        #else
        rng = new System.Random();
        #endif

        //MapCell[,] dungeonMap = new MapCell[parameters.dungeonSize.x, parameters.dungeonSize.y];
        int[,] map = new int[parameters.dungeonSize.x + parameters.roomDistance, parameters.dungeonSize.y + parameters.roomDistance];
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                map[x, y] = (int)TileType.Empty;
            }
        }

        BoundsInt dungeonSpace = new BoundsInt(Vector3Int.zero, (Vector3Int)parameters.dungeonSize);
        List<BoundsInt> roomsBounds = BinarySpacePartition(dungeonSpace, parameters.minRoomSize.x, parameters.minRoomSize.y, rng);

        foreach (BoundsInt roomBounds in roomsBounds)
        {
            GenerateRoom(map, roomBounds, parameters, rng);
        }

        MapCell[,] dungeonMap = new MapCell[map.GetLength(0), map.GetLength(1)];
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] == (int)TileType.Floor)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    Floor floor = StructureGenerator.GenerateFloor(Data.Floors["Stone Floor"]);
                    dungeonMap[x, y] = new MapCell(position, Data.Terrains["Stone Terrain"]);
                    dungeonMap[x, y].AddFloor(floor);
                }
                else if (map[x, y] == (int)TileType.Wall)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    Structure wall = StructureGenerator.GenerateStructure(Data.Structures["Wall"]);
                    dungeonMap[x, y] = new MapCell(position, Data.Terrains["Grass Terrain"]);
                    dungeonMap[x, y].AddStructure(wall);
                }
                else if (map[x, y] == (int)TileType.Door)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    Structure door = StructureGenerator.GenerateStructure(Data.Structures["Door"]);
                    dungeonMap[x, y] = new MapCell(position, Data.Terrains["Water Terrain"]);
                    dungeonMap[x, y].AddStructure(door);
                }
            }
        }

        Dungeon dungeon = new Dungeon(dungeonMap);

        // Set dungeon entrance and exit points
        SetDungeonEntranceAndExit(dungeon);

        // Spawn the dungeon npcs
        GenerateDungeonNPCs(dungeon);

        // Generate the dungeon chests
        GenerateDungeonChests(dungeon);

        return dungeon;
    }

    private static List<BoundsInt> BinarySpacePartition(BoundsInt dungeonSpace, int minWidth, int minHeight, System.Random rng)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(dungeonSpace);
        while (roomsQueue.Count > 0)
        {
            BoundsInt room = roomsQueue.Dequeue();
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                if (rng.Next(10 + 1) < 5) // Half the time
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitSpaceHorizontally(minHeight, roomsQueue, room, rng);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitSpaceVertically(minWidth, roomsQueue, room, rng);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitSpaceVertically(minHeight, roomsQueue, room, rng);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitSpaceHorizontally(minWidth, roomsQueue, room, rng);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }
    private static void SplitSpaceHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room, System.Random rng)
    {
        int splitPoint = rng.Next(1, room.size.y - 1);

        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, splitPoint, 0));

        Vector3Int room2Min = new Vector3Int(room.min.x, room.min.y + splitPoint, 0);
        Vector3Int room2Size = new Vector3Int(room.size.x, room.size.y - splitPoint, 0);
        BoundsInt room2 = new BoundsInt(room2Min, room2Size);

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
    private static void SplitSpaceVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room, System.Random rng)
    {
        int splitPoint = rng.Next(1, room.size.x - 1);

        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(splitPoint, room.size.y, 0));

        Vector3Int room2Min = new Vector3Int(room.min.x + splitPoint, room.min.y, 0);
        Vector3Int room2Size = new Vector3Int(room.size.x - splitPoint, room.size.y, 0);
        BoundsInt room2 = new BoundsInt(room2Min, room2Size);

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }



    private static void GenerateRoom(int[,] dungeonMap, BoundsInt roomBounds, DungeonParameters parameters, System.Random rng)
    {
        int[,] roomMap = RandomFill(roomBounds, parameters.roomFullness, rng);

        for (int i = 0; i < parameters.smoothIterations; i++)
        {
            roomMap = SmoothRoom(roomMap, roomBounds);
        }

        List<Region> wallRegions = GetAllRegionsOfType(roomMap, TileType.Wall);
        foreach (Region wallRegion in wallRegions)
        {
            if (wallRegion.Size > 4) continue;

            foreach (Vector2Int position in wallRegion.Tiles)
            {
                roomMap[position.x, position.y] = (int)TileType.Floor;
            }
        }

        List<Region> floorRegions = GetAllRegionsOfType(roomMap, TileType.Floor);
        List<Region> regionsToRemove = new List<Region>();
        foreach (Region floorRegion in floorRegions)
        {
            if (floorRegion.Size > 8) continue;

            foreach (Vector2Int position in floorRegion.Tiles)
            {
                roomMap[position.x, position.y] = (int)TileType.Empty;
            }
            regionsToRemove.Add(floorRegion);
        }
        foreach (Region region in regionsToRemove) floorRegions.Remove(region);

        floorRegions[0].SetRegionToMain();
        List<Connection> regionConnections = CreateRegionConnections(floorRegions);
        if (regionConnections != null)
            foreach (Connection connection in regionConnections)
                ConnectTwoRegions(connection, roomMap);

        RemoveUnnecessaryWalls(roomMap);

        // Place the newly generated room in the dungeonMap array
        PlaceRoomInDungeon(dungeonMap, roomMap, roomBounds, parameters.roomDistance);
    }
    private static int[,] RandomFill(BoundsInt roomBounds, int roomFullness, System.Random rng)
    {
        int sizeX = roomBounds.size.x;
        int sizeY = roomBounds.size.y;
        int[,] roomMap = new int[sizeX, sizeY];

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                if (x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1) roomMap[x, y] = (int)TileType.Wall;

                else roomMap[x, y] = rng.Next(101) < roomFullness ? (int)TileType.Floor : (int)TileType.Wall;
            }
        }

        return roomMap;
    }
    private static int[,] SmoothRoom(int[,] roomMap, BoundsInt roomBounds)
    {
        int[,] newState = new int[roomBounds.size.x, roomBounds.size.y];

        // Calculate new state
        for (int y = 0; y < roomBounds.size.y; y++)
        {
            for (int x = 0; x < roomBounds.size.x; x++)
            {
                int surroundingWalls = GetSurroundingWalls(x, y, roomMap);

                if (surroundingWalls > 4) newState[x, y] = (int)TileType.Wall;
                else if (surroundingWalls < 4) newState[x, y] = (int)TileType.Floor;
                else newState[x, y] = roomMap[x, y];
            }
        }

        return newState;
    }
    private static int GetSurroundingWalls(int positionX, int positionY, int[,] room)
    {
        int wallCount = 0;

        for (int y = positionY - 1; y <= positionY + 1; y++)
        {
            for (int x = positionX - 1; x <= positionX + 1; x++)
            {
                if (!IsInMapRange(x, y, room)) wallCount++;

                else if (x != positionX || y != positionY)
                {
                    if (room[x, y] == (int)TileType.Wall) wallCount++;
                }
            }
        }

        return wallCount;
    }

    private static List<Region> GetAllRegionsOfType(int[,] roomMap, TileType type)
    {
        List<Region> regions = new List<Region>();
        bool[,] checkedPositions = new bool[roomMap.GetLength(0), roomMap.GetLength(1)];

        for (int y = 0; y < roomMap.GetLength(1); y++)
        {
            for (int x = 0; x < roomMap.GetLength(0); x++)
            {
                if (roomMap[x, y] == (int)type && !checkedPositions[x, y])
                {
                    Region newRegion = FloodFill(x, y, roomMap, checkedPositions, type);
                    regions.Add(newRegion);
                }
            }
        }

        return regions;
    }
    private static Region FloodFill(int initialX, int initialY, int[,] roomMap, bool[,] checkedPositions, TileType type)
    {
        List<Vector2Int> regionTiles = new List<Vector2Int>();

        Queue<Vector2Int> positionsToCheck = new Queue<Vector2Int>();
        positionsToCheck.Enqueue(new Vector2Int(initialX, initialY));
        checkedPositions[initialX, initialY] = true;

        while (positionsToCheck.Count > 0)
        {
            Vector2Int currentPosition = positionsToCheck.Dequeue();
            regionTiles.Add(currentPosition);

            for (int y = currentPosition.y - 1; y <= currentPosition.y+1; y++)
            {
                for (int x = currentPosition.x - 1; x <= currentPosition.x + 1; x++)
                {
                    if (!IsInMapRange(x, y, roomMap) || roomMap[x, y] != (int)type || checkedPositions[x, y]) continue;
                    //if (x == currentPosition.x && y == currentPosition.y) continue;
                    checkedPositions[x, y] = true;
                    Vector2Int newPosition = new Vector2Int(x, y);
                    positionsToCheck.Enqueue(newPosition);
                }
            }
        }

        return new Region(regionTiles, type, roomMap);
    }

    private static void RemoveUnnecessaryWalls(int[,] roomMap)
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            new Vector2Int( 1,  1),
            Vector2Int.right,
            new Vector2Int( 1, -1),
            Vector2Int.down,
            new Vector2Int(-1, -1),
            Vector2Int.left,
            new Vector2Int(-1,  1)
        };

        for (int y = 0; y < roomMap.GetLength(1); y++)
        {
            for (int x = 0; x < roomMap.GetLength(0); x++)
            {
                if (roomMap[x, y] != (int)TileType.Wall) continue;

                bool shouldRemoveWall = true;
                foreach (Vector2Int direction in directions)
                {
                    int newX = x + direction.x;
                    int newY = y + direction.y;

                    if (newX < 0 || newX >= roomMap.GetLength(0) || newY < 0 || newY >= roomMap.GetLength(1)) continue;

                    if (roomMap[newX, newY] == (int)TileType.Floor)
                    {
                        shouldRemoveWall = false;
                        break;
                    }
                }
                if (shouldRemoveWall) roomMap[x, y] = (int)TileType.Empty;
            }
        }
    }
    private static void PlaceRoomInDungeon(int[,] dungeonMap, int[,] roomMap, BoundsInt roomBounds, int offset)
    {
        Vector2Int roomStartPosition = new Vector2Int(roomBounds.min.x, roomBounds.min.y);

        for (int y = 0; y < roomMap.GetLength(1); y++)
        {
            for (int x = 0; x < roomMap.GetLength(0); x++)
            {
                int globalX = roomStartPosition.x + x + offset;
                int globalY = roomStartPosition.y + y + offset;

                dungeonMap[globalX, globalY] = roomMap[x, y];
            }
        }
    }

    private static List<Connection> CreateRegionConnections(List<Region> regions, bool ensureAllRegionsAreConnected = false)
    {// TODO Function seems too long
        if (regions.Count < 2) return null;
        List<Connection> allConnections = new List<Connection>();

        List<Region> regionsA, regionsB;
        if (ensureAllRegionsAreConnected)
        {
            regionsA = new List<Region>();
            regionsB = new List<Region>();

            foreach (Region region in regions)
            {
                if (region.ConnectsToMainRegion) regionsA.Add(region);
                else regionsB.Add(region);
            }
        }
        else
        {
            regionsA = regions;
            regionsB = regions;
        }

        int lowestDistance = int.MaxValue;
        Connection connection = new Connection();
        bool connectionFound = false;
        foreach (Region regionA in regionsA)
        {
            if (!ensureAllRegionsAreConnected)
            {
                connectionFound = false;
                lowestDistance = int.MaxValue;
            }
            foreach (Region regionB in regionsB)
            {
                if (regionA == regionB) continue;
                if (regionA.IsConnectedTo(regionB))
                {
                    connectionFound = false;
                    break;
                }

                Connection newConnection = GetClosestPointsBetweenTwoRegions(regionA, regionB);
                if (newConnection.distance < lowestDistance)
                {
                    connectionFound = true;
                    lowestDistance = newConnection.distance;
                    connection = newConnection;
                }
            }

            if (connectionFound && !ensureAllRegionsAreConnected)
            {
                allConnections.Add(connection);
                connection.region1.ConnectToRegion(connection.region2);
                connection.region2.ConnectToRegion(connection.region1);
            }
        }

        if (connectionFound && ensureAllRegionsAreConnected)
        {
            allConnections.Add(connection);
            connection.region1.ConnectToRegion(connection.region2);
            connection.region2.ConnectToRegion(connection.region1);

            List<Connection> newConnections = CreateRegionConnections(regions, true);
            foreach (Connection newConnection in newConnections)
                allConnections.Add(newConnection);
        }

        if (!ensureAllRegionsAreConnected)
        {
            List<Connection> newConnections = CreateRegionConnections(regions, true);
            foreach (Connection newConnection in newConnections)
                allConnections.Add(newConnection);
        }

        return allConnections;
    }
    private static Connection GetClosestPointsBetweenTwoRegions(Region region1, Region region2)
    {
        int lowestDistance = int.MaxValue;
        Connection connection = new Connection();

        foreach (Vector2Int tile1 in region1.EdgeTiles)
        {
            foreach (Vector2Int tile2 in region2.EdgeTiles)
            {
                int distance = (tile1.x - tile2.x) * (tile1.x - tile2.x) + (tile1.y - tile2.y) * (tile1.y - tile2.y);
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    connection = new Connection(distance, region1, region2, tile1, tile2);
                }
            }
        }

        return connection;
    }
    private static void ConnectTwoRegions(Connection connection, int[,] roomMap)
    {// TODO THERE'S PROBABLY A BETTER WAY OF DOING THIS
        int dx = connection.tileInRegion2.x - connection.tileInRegion1.x;
        int dy = connection.tileInRegion2.y - connection.tileInRegion1.y;

        int absoluteDx = Mathf.Abs(dx);
        int absoluteDy = Mathf.Abs(dy);

        int largerMagnitude = absoluteDx > absoluteDy ? absoluteDx : absoluteDy;
        int smallerMagnitude = absoluteDx > absoluteDy ? absoluteDy : absoluteDx;

        Vector2Int direction, change;
        if (absoluteDx > absoluteDy)
        {
            direction = new Vector2Int((int)Mathf.Sign(dx), 0);
            change = new Vector2Int(0, dy == 0 ? 0 : (int)Mathf.Sign(dy));
        }
        else if (absoluteDx < absoluteDy)
        {
            direction = new Vector2Int(0, (int)Mathf.Sign(dy));
            change = new Vector2Int(dx == 0 ? 0 : (int)Mathf.Sign(dx), 0);
        }
        else
        {
            direction = new Vector2Int((int)Mathf.Sign(dx), (int)Mathf.Sign(dy));
            change = new Vector2Int(0, 0);
        }

        int stepsBeforeChange;
        if (smallerMagnitude == 0) stepsBeforeChange = -1; // Ensures that it never reaches a change in direction, saves a bit of performance
        else if (smallerMagnitude == 1) stepsBeforeChange = largerMagnitude / (smallerMagnitude + 1);
        else stepsBeforeChange = largerMagnitude / smallerMagnitude;

        int stepsTaken = stepsBeforeChange;
        int changesTaken = 0;
        Vector2Int currentPosition = connection.tileInRegion1;
        Vector2Int previousPosition = currentPosition;

        //bool doorAdded = false;
        
        for (int i = 0; i < largerMagnitude-1; i++)
        {
            previousPosition = currentPosition;
            currentPosition += direction;
            if (stepsTaken == 0 && changesTaken < smallerMagnitude)
            {
                currentPosition += change;
                stepsTaken = stepsBeforeChange;
                changesTaken++;
            }
            stepsTaken--;

            /*if (!doorAdded)
            {
                if (roomMap[currentPosition.x, currentPosition.y] == (int)TileType.Wall)
                {
                    roomMap[currentPosition.x, currentPosition.y] = (int)TileType.Door;
                    doorAdded = true;
                }
                else roomMap[currentPosition.x, currentPosition.y] = (int)TileType.Floor;
            }
            else roomMap[currentPosition.x, currentPosition.y] = (int)TileType.Floor;*/
            roomMap[currentPosition.x, currentPosition.y] = (int)TileType.Floor;
        }
        
        //roomMap[previousPosition.x, previousPosition.y] = (int)TileType.Floor;
    }

    private static void SetDungeonEntranceAndExit(Dungeon dungeon)
    {
        // Add the dungeon start point
        MapCell randomCell = dungeon.GetRandomPosition();
        DungeonEntrance entrance = new DungeonEntrance(EntranceType.Entrance);
        randomCell.AddDungeonEntrance(entrance);
        dungeon.SetEntrance(entrance);

        // Add the dungeon end point
        randomCell = dungeon.GetRandomPosition();
        DungeonEntrance exit = new DungeonEntrance(EntranceType.Exit);
        randomCell.AddDungeonEntrance(exit);
        dungeon.SetEntrance(exit);
    }
    private static void GenerateDungeonNPCs(Dungeon dungeon)
    {
        MapCell randomCell;
        int npcCount = Data.mapRng.Next(10, 20+1); // 10 - 20 npcs

        for (int i = 0; i < npcCount-1; i++)
        {
            randomCell = dungeon.GetRandomPosition();

            Agent newNpc = AgentGenerator.GenerateAgent(false);
            randomCell.AddAgent(newNpc);
            Data.AddAgent(newNpc);
        }

        randomCell = dungeon.GetRandomPosition();

        Agent boss = AgentGenerator.GenerateAgent(true);
        boss.ChangeColour(Data.ColourDict[ColourEnum.Orange]);
        randomCell.AddAgent(boss);
        dungeon.SetBoss(boss);
        Data.AddAgent(boss);
    }
    private static void GenerateDungeonChests(Dungeon dungeon)
    {
        MapCell randomCell;
        Structure chest;

        int normalChestCount = Data.mapRng.Next(2, 5+1); // 2 - 5
        for (int i = 0; i < normalChestCount; i++)
        {
            randomCell = dungeon.GetRandomPosition();
            chest = new Structure(Data.Structures["Chest"]);
            for (int j = 0; j < 5; j++)
            {
                int selector = Data.mapRng.Next(4);
                switch (selector)
                {
                    case 0: chest.Inventory.PlaceItem(ItemGenerator.GenerateRandomWeapon()); break;
                    case 1: chest.Inventory.PlaceItem(ItemGenerator.GenerateRandomArmour()); break;
                    default: chest.Inventory.PlaceItem(ItemGenerator.GenerateItem(Data.Items["Bread"])); break;
                }
            }
            randomCell.AddStructure(chest);
        }


        int ornateChestCount = Data.mapRng.Next(1, 2 + 1); // 1 - 2
        for (int i = 0; i < ornateChestCount; i++)
        {
            randomCell = dungeon.GetRandomPosition();
            chest = new Structure(Data.Structures["Ornate Chest"]);
            for (int j = 0; j < 10; j++)
            {
                int selector = Data.mapRng.Next(4);
                switch (selector)
                {
                    case 0: chest.Inventory.PlaceItem(ItemGenerator.GenerateRandomWeapon()); break;
                    case 1: chest.Inventory.PlaceItem(ItemGenerator.GenerateRandomArmour()); break;
                    default: chest.Inventory.PlaceItem(ItemGenerator.GenerateItem(Data.Items["Bread"])); break;
                }
            }
            randomCell.AddStructure(chest);
        }
    }

    private static bool IsInMapRange(int x, int y, int[,] map)
    {
        return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1);
    }
#endregion  Methods
}


#region Dungeon Parameters
[System.Serializable]
public struct DungeonParameters
{
    public Vector2Int dungeonSize;
    public Vector2Int minRoomSize;

    [Range(1, 100)]
    public int roomFullness;
    public int smoothIterations;

    public int roomDistance;
}
#endregion Dungeon Parameters


public class Region
{
#region Dara
    private List<Vector2Int> tiles;
    private List<Vector2Int> edgeTiles;

    private List<Region> connectedRegions;
    private bool connectsToMainRegion;
#endregion Data

#region Properties
    public List<Vector2Int> Tiles { get => tiles; }
    public int Size { get => tiles.Count; }
    public List<Vector2Int> EdgeTiles { get => edgeTiles; }

    public bool ConnectsToMainRegion { get => connectsToMainRegion; }
#endregion Properties


#region Methods
    public Region(List<Vector2Int> tiles, TileType type, int[,] roomMap)
    {
        this.tiles = tiles;
        connectedRegions = new List<Region>();
        connectsToMainRegion = false;

        if (type == TileType.Floor)
        {
            edgeTiles = new List<Vector2Int>();
            foreach(Vector2Int tile in tiles)
            {
                bool checkDone = false;
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    for (int x = tile.x - 1; x < tile.x + 1; x++)
                    {
                        if (x < 0 || x >= roomMap.GetLength(0) || y < 0 || y >= roomMap.GetLength(1)) continue;
                        if (roomMap[x, y] == (int)TileType.Wall)
                        {
                            edgeTiles.Add(tile);
                            checkDone = true;
                            break;
                        }
                    }
                    if (checkDone) break;
                }
            }
        }
    }
    public void SetRegionToMain()
    {
        connectsToMainRegion = true;
    }
    public void PropagateConnectionToMainRegion()
    {
        if (!connectsToMainRegion)
        {
            connectsToMainRegion = true;

            foreach (Region connectedRegion in connectedRegions)
            {
                connectedRegion.PropagateConnectionToMainRegion();
            }
        }
    }


    public void ConnectToRegion(Region regionToConnect)
    {
        connectedRegions.Add(regionToConnect);
        if (regionToConnect.ConnectsToMainRegion) PropagateConnectionToMainRegion();
    }
    public bool IsConnectedTo(Region otherRegion)
    {
        return connectedRegions.Contains(otherRegion);
    }
#endregion Methods
}


#region Connection
public struct Connection
{
    public int distance;

    public Region region1;
    public Region region2;

    public Vector2Int tileInRegion1;
    public Vector2Int tileInRegion2;

    public Connection(int distance, Region region1, Region region2, Vector2Int tileInRegion1, Vector2Int tileInRegion2)
    {
        this.distance = distance;
        
        this.region1 = region1;
        this.region2 = region2;

        this.tileInRegion1 = tileInRegion1;
        this.tileInRegion2 = tileInRegion2;
    }
}
#endregion Connection

public enum TileType : sbyte { Empty = -1, Wall, Floor, Door }