using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Floor : Object
{// TODO SHOULD FIX THIS, MIGHT NOT EVEN NEED IT
    #region Data
    #endregion Data

    #region Properties
    #endregion Properties


    #region Methods
    public Floor(FloorData data)
        : base(data.Name, data.Colour, data.Tile)
    {
    }
    #endregion Methods
}