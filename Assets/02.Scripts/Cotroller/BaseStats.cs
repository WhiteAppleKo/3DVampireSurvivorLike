using System;
using UnityEngine;

[Serializable]
public class BaseStats
{
    public bool usePlayerStats = false;
    
    public ClampInt hp;
    public int maxHp = 100;
    
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float turnSpeed = 10.0f;
    
    public PlayerStats playerStats;
    
    // 얕은 복사 문제 hp, playerStats 깊은 복사 필요
    public BaseStats(BaseStats other)
    {
        this.usePlayerStats = other.usePlayerStats;
        this.hp = new ClampInt(other.hp.Min, other.hp.Max, other.hp.Current);
        this.maxHp = other.maxHp;
        this.moveSpeed = other.moveSpeed;
        this.turnSpeed = other.turnSpeed;
        
        if (usePlayerStats)
        {
            this.playerStats = new PlayerStats(other.playerStats);
        }
    }
    
    public BaseStats()
    {
        
    }
}
