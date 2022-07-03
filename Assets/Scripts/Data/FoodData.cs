using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodData
{
    #region Data
    private int nutrition;
    private FoodType type;
    #endregion Data

    #region Properties
    public int Nutrition { get => nutrition; }
    public FoodType Type { get => type; }
    #endregion Properties


    #region Methods
    public FoodData(int nutrition, FoodType type)
    {
        this.nutrition = nutrition;
        this.type = type;
    }
    #endregion Methods
}


public enum FoodType : byte
{
    Herbivore,
    Carnivore
}