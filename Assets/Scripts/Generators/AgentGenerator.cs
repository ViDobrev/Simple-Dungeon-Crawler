using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public static class AgentGenerator
{
    #region Methods
    public static Agent GenerateAgent(bool generateBoss)
    {
        string name;
        Species species;
        Gender gender;
        int level;

        System.Random rng = new System.Random(Data.agentRng.Next());


        Species[] speciesList;

        if (generateBoss)
        {
            speciesList = (from Species _species in Data.Species.Values
                           where _species.Intelligent
                           select _species).ToArray();

            level = Data.agentRng.Next(Data.Player.Level, Data.Player.Level + 4); // Up to 3 levels above player
        }
        else
        {
            speciesList = Data.Species.Values.ToArray();
            level = Data.agentRng.Next(Data.Player.Level-1, Data.Player.Level + 2); // From 1 level below the player to 1 level above them
            if (level < 1) level = 1;
        }

        species = speciesList[rng.Next(0, speciesList.Length)];

        int genders = System.Enum.GetNames(typeof(Gender)).Length;
        gender = (Gender)rng.Next(0, genders);

        name = species.Name;

        Equipment equipment = null;
        List<Item> items = new List<Item>();
        if (species.Intelligent)
        {
            items.Add(ItemGenerator.GenerateRandomWeapon());
            items.Add(ItemGenerator.GenerateRandomArmour());
        }
        int nMiscItems = Data.agentRng.Next(3); // Up to 2 other items
        for (int i = 0; i < nMiscItems; i++)
        {
            items.Add(ItemGenerator.GenerateItem(Data.Items["Bread"]));
        }

        equipment = new Equipment(5, species, items);

        return new Agent(name, gender, species, level, equipment);
    }


    private static string GenerateName(Gender gender, System.Random rng)
    {
        string name;

        if (gender == Gender.Male)
        {
            string path = Application.dataPath + @"\Database\Names_Male.txt";
            List<string> names = new List<string>(File.ReadAllLines(path));
            name = names[rng.Next(0, names.Count)];
        }
        else
        {
            string path = Application.dataPath + @"\Database\Names_Male.txt";
            List<string> names = new List<string>(File.ReadAllLines(path));
            name = names[rng.Next(0, names.Count)];
        }

        return name;
    }
    #endregion Methods
}