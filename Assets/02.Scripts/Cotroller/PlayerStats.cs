using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public int level = 1;
    public ClampInt exp;

    private float m_ExpRatio;
    
    public void AddExp(int amount)
    {
        exp.Increase(amount);
    }
    public PlayerStats(PlayerStats other)
    {
        this.level = other.level;
        this.exp = other.exp;
    }
    
    public PlayerStats()
    {
        
    }
}
