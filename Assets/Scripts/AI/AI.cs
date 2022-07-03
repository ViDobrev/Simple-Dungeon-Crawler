using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AI
{
    #region Data
    private Agent agent;
    private AIState state;

    private int maxDistanceToEngage = 16; // 4^2
    private Vector2Int previousPlayerPosition = new Vector2Int(-1, -1);
    private Queue<Vector2Int> path;
    #endregion Data

    #region Properties
    #endregion Properties


    #region Methods
    public AI(Agent agent)
    {
        this.agent = agent;
    }

    public void Run()
    {
        PrePerform();
        Perform();
    }
    private void PrePerform()
    {
        if (GetDistanceToPlayer() <= maxDistanceToEngage) state = AIState.Attacking;
        else state = AIState.Wandering;
    }
    private void Perform()
    {
        if (state == AIState.Wandering) Wander();

        else // Attacking
        {
            // If agent is in the adjacent cell as player -> attack
            //if (agent.Position == Data.Player.Position) Attack();
            if (IsWithinAttackArange()) Attack();

            // If player has moved -> calculate new path to player
            else if (Data.Player.Position != previousPlayerPosition)
            {
                CalculatePathToPlayer();
                Perform();
            }

            // Move agent towards the player
            else if (agent.Move(path.Peek()))
                path.Dequeue();
        }
    }

    private void Wander()
    {
        bool shouldMove = Data.aiRng.Next() % 2 == 0;

        if (shouldMove)
        {
            int x = Data.aiRng.Next(-1, 2);
            int y = Data.aiRng.Next(-1, 2);
            Vector2Int directionToMove = new Vector2Int(x, y);

            agent.Move(directionToMove);
        }
    }
    private void Attack()
    {
        InteractionHandler.Attack(agent, Data.Player);
    }

    private bool IsWithinAttackArange()
    {// All attacks currently have a range of 1
        int attackRange = 1;
        Vector2Int difference = agent.Position - Data.Player.Position;

        return Math.Abs(difference.x) <= attackRange && Math.Abs(difference.y) <= attackRange;
    }
    private void CalculatePathToPlayer()
    {
        previousPlayerPosition = Data.Player.Position;
        path = Pathfinder.FindPath(agent.Position, Data.Player.Position);
    }
    private int GetDistanceToPlayer()
    {
        int xDiff = agent.Position.x - Data.Player.Position.x;
        int yDiff = agent.Position.y - Data.Player.Position.y;

        int distance = (xDiff * xDiff) + (yDiff * yDiff);

        return distance;
    }
    #endregion Methods
}


public enum AIState : byte { Wandering, Attacking }