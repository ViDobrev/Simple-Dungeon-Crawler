using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class InteractionHandler
{
    private static int minMissChance = 5;  // 5%
    private static int maxMissChance = 90; // 90%
    private static int missChancePerAgilityDifference = 6; // 6%
    private static int expPerLevelDifference = 10;

    #region Methods
    public static void Attack(Player player, Agent npc)
    {// The player attack an NPC
        int missChance;
        int agilityDifference = player.Stats[StatName.Agility] - npc.Stats[StatName.Agility];

        if (agilityDifference <= 0) missChance = agilityDifference * (-1) * missChancePerAgilityDifference + minMissChance;
        else missChance = minMissChance;

        if (missChance < minMissChance) missChance = minMissChance;
        else if (missChance > maxMissChance) missChance = maxMissChance;

        if (Data.diceRng.Next(101) < missChance)
        {
            Logger.SendMessageToUI($"<color=green>You</color> attack the <color=red>{npc.Species.Name}</color> with your <color=green>{player.Weapon.FullName}</color>, but miss.");
            return;
        }

        Weapon weapon = player.Weapon;
        Attack attack;
        if (weapon == null) attack = player.Species.UnarmedAttack;
        else attack = weapon.Attack;

        Damage damage = attack.RollDamage(player.Stats[StatName.Strength]);

        Logger.SendMessageToUI($"<color=green>You</color> attack the <color=red>{npc.Species.Name}</color> with your <color=green>{player.Weapon.FullName}</color> and do <color=green>{damage.Power}</color> points of damage to it.");
        if (npc.TakeDamage(damage))
        {
            if (npc == Data.Player.Target) Data.Player.SetTarget(null);
            int levelDifference = player.Level - npc.Level;
            int expGain = levelDifference > 0 ? levelDifference * expPerLevelDifference : expPerLevelDifference;
            Logger.SendMessageToUI($"The <color=red>{npc.Species.Name}</color> crumbles and dies. You receive <color=green>{expGain}</color> exp.");
            if (npc == Data.Dungeon.Boss) Logger.SendMessageToUI("With the death of this dungeon's boss, the entrance to the next level is unlocked.");
            Data.Player.Stats.AddExperience(expGain);
        }
    }
    public static void Attack(Agent attackingNpc, Player player)
    {// An NPC attacks the player
        int missChance;
        int agilityDifference = attackingNpc.Stats[StatName.Agility] - player.Stats[StatName.Agility];

        if (agilityDifference <= 0) missChance = agilityDifference * (-1) * missChancePerAgilityDifference + minMissChance;
        else missChance = minMissChance;

        if (missChance < minMissChance) missChance = minMissChance;
        else if (missChance > maxMissChance) missChance = maxMissChance;

        if (Data.diceRng.Next(101) < missChance)
        {
            Logger.SendMessageToUI($"The <color=red>{attackingNpc.Species.Name}</color> attacks <color=green>you</color>, but misses.");
            return;
        }

        Weapon weapon = attackingNpc.Weapon;
        Attack attack;
        if (weapon == null) attack = attackingNpc.Species.UnarmedAttack;
        else attack = weapon.Attack;

        Damage damage = attack.RollDamage(attackingNpc.Stats[StatName.Strength]);

        Logger.SendMessageToUI($"The <color=red>{attackingNpc.Species.Name}</color> attacks <color=green>you</color>, dealing <color=red>{damage.Power}</color> damage to you.");
        player.TakeDamage(damage);
    }

    public static void DropItems(List<ItemSlot> itemsToDrop, Vector2Int position)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        int droppedItems = 0;
        // Attempt to drop item at given location
        if (DropItem(itemsToDrop[droppedItems], position)) droppedItems++;

        int direction = 0;
        int steps = 1;
        int iter = 0;
        //Spiral pattern
        while (droppedItems != itemsToDrop.Count)
        {
            for (int i = 0; i < steps; i++)
            {
                position += directions[direction];
                if (DropItem(itemsToDrop[droppedItems], position)) droppedItems++;
                if (droppedItems == itemsToDrop.Count) break;
            }

            direction++;
            if (direction > directions.Length - 1) direction = 0;

            iter++;
            if (iter == 2)
            {
                steps++;
                iter = 0;
            }
        }
    }
    private static bool DropItem(ItemSlot itemToDrop, Vector2Int position)
    {
        MapCell cell = Data.Dungeon.CellAtPosition(position);

        if (cell.AddItem(itemToDrop)) return true;

        return false;
    }
    #endregion Methods
}