using System;
using System.Collections.Generic;
using _02.Scripts.Cotroller;
using Shapes;
using Unity.Multiplayer.PlayMode;
using UnityEditor;
using UnityEngine;

public class SubscribeManager : SingletoneBase<SubscribeManager>
{
    public PlayerController playerController; // Controller -> PlayerController로 변경
    public Action<float> onPlayerHpChangeEvent;
    public Action<float> onPlayerExpChangeEvent;
    public IMEXPPanel expPanel;
    
    private float m_HpRatio;
    private float m_ExpRatio;

    public void GameStart()
    {
        if (playerController == null)
        {
            Debug.LogError("[SubscribeManager] PlayerController가 할당되지 않았습니다.");
            return;
        }

        SubScribe();
        if (expPanel != null) expPanel.GameStart();
        
        // 초기값 설정
        UpdateHp(playerController.Model.CurrentHp, playerController.Model.MaxHp);
        UpdateExp(playerController.Model.CurrentExp, playerController.Model.MaxExp);
    }

    private void OnDisable()
    {
        UnSubScribe();
    }

    private void SubScribe()
    {
        if (playerController == null || playerController.Model == null) return;

        // 새로운 DLV 모델의 이벤트 구독
        playerController.Model.OnHpChanged += UpdateHp;
        playerController.Model.OnExpChanged += UpdateExp;
        playerController.Model.OnLevelUp += OnLevelUp;
    }
    
    private void UnSubScribe()
    {
        if (playerController == null || playerController.Model == null) return;

        playerController.Model.OnHpChanged -= UpdateHp;
        playerController.Model.OnExpChanged -= UpdateExp;
        playerController.Model.OnLevelUp -= OnLevelUp;
    }
    
    private void OnLevelUp(int newLevel)
    {
        ExpManager.Instance?.PlayerLevelUp();
    }
    
    private void UpdateExp(int current, int max)
    {
        m_ExpRatio = (float)current / max;
        onPlayerExpChangeEvent?.Invoke(m_ExpRatio);
    }

    private void UpdateHp(int current, int max)
    {
        m_HpRatio = (float)current / max;
        onPlayerHpChangeEvent?.Invoke(m_HpRatio);
    }
}
