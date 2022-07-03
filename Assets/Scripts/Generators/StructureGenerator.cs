using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class StructureGenerator
{
    #region Methods
    public static Structure GenerateStructure(StructureData structureData)
    {
        Structure structure = new Structure(structureData);
        return structure;
    }
    public static Floor GenerateFloor(FloorData floorData)
    {
        Floor floor = new Floor(floorData);
        return floor;
    }
    #endregion Methods
}