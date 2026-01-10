using System;
using UnityEngine;

[Serializable]
public class BaseStats
{
    public bool usePlayerStats = false;
    
    public ClampInt hp;
    public int maxHp = 100;
    
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

    public void ResetTo(BaseStats other)
    {
        this.usePlayerStats = other.usePlayerStats;
        this.maxHp = other.maxHp;
        this.moveSpeed = other.moveSpeed;
        this.turnSpeed = other.turnSpeed;
        
        // hp 객체의 참조를 유지한 채 값만 업데이트합니다.
        if (this.hp != null && other.hp != null)
        {
            this.hp.UpdateValues(other.hp.Min, other.hp.Max, this.hp.Current);
        }
        else if (this.hp == null && other.hp != null)
        {
             this.hp = new ClampInt(other.hp.Min, other.hp.Max, this.hp.Current);
        }
        
        if (usePlayerStats && this.playerStats != null && other.playerStats != null)
        {
            // PlayerStats도 같은 방식의 복사가 필요할 수 있으나, 현재는 참조 할당
            // 깊은 복사가 필요하다면 PlayerStats에도 ResetTo 구현 필요
            // this.playerStats = new PlayerStats(other.playerStats); // 기존 방식
        }
    }
}
