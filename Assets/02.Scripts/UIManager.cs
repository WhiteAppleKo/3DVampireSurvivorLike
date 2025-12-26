using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : SingletoneBase<UIManager>
{
    public Controller playerController;
    public Action<float> onPlayerHpChangeEvent;
    private float m_Ratio;
    public void GameStart()
    {
        SubScribe();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnSubScribe();
    }

    private void SubScribe()
    {
        playerController.hp.Events.onValueChanged += ChangePlayerHp;
    }
    
    private void UnSubScribe()
    {
        playerController.hp.Events.onValueChanged -= ChangePlayerHp;
    }

    private void ChangePlayerHp(int prev, int current)
    {
        m_Ratio = playerController.hp.Ratio;
        onPlayerHpChangeEvent?.Invoke(m_Ratio);
    }
}
