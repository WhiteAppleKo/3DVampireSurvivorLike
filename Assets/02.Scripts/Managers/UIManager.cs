using System;
using System.Collections.Generic;
using Shapes;
using UnityEditor;
using UnityEngine;

public class UIManager : SingletoneBase<UIManager>
{
    public Controller playerController;
    public Action<float> onPlayerHpChangeEvent;
    public Action<float> onPlayerExpChangeEvent;
    public IMEXPPanel expPanel;
    
    private float m_HpRatio;
    private float m_ExpRatio;
    public void GameStart()
    {
        SubScribe();
        expPanel.GameStart();
    }

    private void OnDisable()
    {
        UnSubScribe();
    }

    private void SubScribe()
    {
        playerController.baseStats.hp.Events.onValueChanged += ChangePlayerHp;
        playerController.baseStats.playerStats.exp.Events.onValueChanged += onChangePlayerExp;
        playerController.baseStats.playerStats.exp.Events.onMaxReached += PlayerLevelUp;
    }
    
    private void UnSubScribe()
    {
        playerController.baseStats.hp.Events.onValueChanged -= ChangePlayerHp;
        playerController.baseStats.playerStats.exp.Events.onValueChanged -= onChangePlayerExp;
        playerController.baseStats.playerStats.exp.Events.onMaxReached -= PlayerLevelUp;
    }
    
    private void PlayerLevelUp(int prev, int current)
    {
        m_ExpRatio = 0;
        playerController.baseStats.playerStats.level++;
        playerController.baseStats.playerStats.exp.ResetToMin();
    }
    
    private void onChangePlayerExp(int prev, int current)
    {
        m_ExpRatio = playerController.baseStats.playerStats.exp.Ratio;
        onPlayerExpChangeEvent?.Invoke(m_ExpRatio);
    }

    private void ChangePlayerHp(int prev, int current)
    {
        m_HpRatio = playerController.baseStats.hp.Ratio;
        onPlayerHpChangeEvent?.Invoke(m_HpRatio);
    }
}
