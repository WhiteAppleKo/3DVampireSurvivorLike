using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public int level = 1;
    public int augmentationLevel = 1;
    public ClampInt exp;
    public Action levelUpEvent;
    
    private float m_ExpRatio;

    #region 생성자
    public PlayerStats(PlayerStats other)
    {
        this.level = other.level;
        this.exp = other.exp;
    }
    
    public PlayerStats()
    {
        
    }
    #endregion
    
    #region 증강 관련
    public void GetAugmentattionLevel()
    {
        augmentationLevel++;
    }
    #endregion
    
    #region 레벨 관련
    public void AddExp(int amount)
    {
        exp.Increase(amount);
    }
    public void LevelUp()
    {
        level++;
        levelUpEvent?.Invoke();
    }
    #endregion
}
