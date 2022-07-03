using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PlantGenerator
{
    #region Methods
    public static Plant GeneratePlant(PlantSpecies species, float grown, Vector2Int position, MapCell parentCell)
    {
        if (grown < 0)
        {
            Debug.LogError("PlantGenerator.GeneratePlant(): grown paremeter < 0.");
            return null;
        }

        Plant plant = new Plant(species, grown, position, parentCell);
        return plant;
    }
    public static Plant GeneratePlant(PlantSpecies species, float grown)
    {
        if (grown < 0)
        {
            Debug.LogError("PlantGenerator.GeneratePlant(): grown paremeter < 0.");
            return null;
        }

        Plant plant = new Plant(species, grown, Vector2Int.zero, null);
        return plant;
    }
    public static Plant GeneratePlant(PlantSpecies species)
    {
        Plant plant = new Plant(species, 100f, Vector2Int.zero, null);
        return plant;
    }
    #endregion Methods
}