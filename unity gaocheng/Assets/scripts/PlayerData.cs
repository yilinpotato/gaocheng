using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int score;
    public int exploredNodes;
    public float currentHealth;
    public int sessionSeed;
    public List<string> obtainedItems;

    public PlayerData(int score, int exploredNodes, float currentHealth, int sessionSeed, List<string> obtainedItems)
    {
        this.score = score;
        this.exploredNodes = exploredNodes;
        this.currentHealth = currentHealth;
        this.sessionSeed = sessionSeed;
        this.obtainedItems = obtainedItems;
    }
}