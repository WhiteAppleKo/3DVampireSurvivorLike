using System;
using System.Collections.Generic;
using _02.Scripts.Cotroller;
using Shapes;
using Unity.Multiplayer.PlayMode;
using UnityEditor;
using UnityEngine;

public class SubscribeManager : SingletoneBase<SubscribeManager>
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
        SetPlayerHp(1, 1);
        OnChangePlayerExp(1, 1);
    }

    private void OnDisable()
    {
        UnSubScribe();
    }

    private void SubScribe()
    {
        playerController.FinalStats.hp.Events.onValueChanged += SetPlayerHp;
        //playerController.FinalStats.hp.Events.onDecreased += ChangePlayerHp;
        //playerController.FinalStats.hp.Events.onIncreased += ChangePlayerHp;
        
        playerController.FinalStats.playerStats.exp.Events.onValueChanged += OnChangePlayerExp;
        //playerController.FinalStats.playerStats.exp.Events.onIncreased += OnChangePlayerExp;
        //playerController.FinalStats.playerStats.exp.Events.onDecreased += OnChangePlayerExp;
        playerController.FinalStats.playerStats.exp.Events.onMaxReached += PlayerLevelUp;
    }
    
    private void UnSubScribe()
    {
        playerController.FinalStats.hp.Events.onValueChanged -= SetPlayerHp;
        //playerController.FinalStats.hp.Events.onDecreased -= ChangePlayerHp;
        //playerController.FinalStats.hp.Events.onIncreased -= ChangePlayerHp;
        
        playerController.FinalStats.playerStats.exp.Events.onValueChanged -= OnChangePlayerExp;
        //playerController.FinalStats.playerStats.exp.Events.onIncreased -= OnChangePlayerExp;
        //playerController.FinalStats.playerStats.exp.Events.onDecreased += OnChangePlayerExp;
        playerController.FinalStats.playerStats.exp.Events.onMaxReached -= PlayerLevelUp;
    }
    
    private void PlayerLevelUp(int prev, int current)
    {
        ExpManager.Instance.PlayerLevelUp();
    }
    
    private void OnChangePlayerExp(int prev, int current)
    {
        m_ExpRatio = playerController.FinalStats.playerStats.exp.Ratio;
        onPlayerExpChangeEvent?.Invoke(m_ExpRatio);
    }

    private void SetPlayerHp(int prev, int current)
    {
        m_HpRatio = playerController.FinalStats.hp.Ratio;
        onPlayerHpChangeEvent?.Invoke(m_HpRatio);
    }

    private void ChangePlayerHp(int prev, int current)
    {
        m_HpRatio = playerController.FinalStats.hp.Ratio;
        onPlayerHpChangeEvent?.Invoke(m_HpRatio);
        Debug.Log($"{m_HpRatio}");
    }
}
