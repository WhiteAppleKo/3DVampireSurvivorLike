using System;
using System.Collections.Generic;
using _02.Scripts.Cotroller;
using _02.Scripts.Managers;
using _02.Scripts.Managers.Choice;
using _02.Scripts.UI;
using Shapes;
using UnityEngine;

public class SubscribeManager : SingletoneBase<SubscribeManager>
{
    [Header("Player Reference")]
    public PlayerController playerController;
    
    [Header("UI Views")]
    public PlayerHPInLine hpInLineUI;
    public CircleEXP circleExpUI;
    public IMEXPPanel expPanel;
    public ChoiceUIView choiceUIView;

    public void GameStart()
    {
        if (playerController == null)
        {
            Debug.LogError("[SubscribeManager] PlayerController가 할당되지 않았습니다.");
            return;
        }

        // [External Binding] Model <-> UI Views
        BindUIViews();
        
        if (expPanel != null) expPanel.GameStart();
    }

    private void OnDisable()
    {
        UnSubScribe();
    }

    private void BindUIViews()
    {
        if (playerController == null || playerController.Model == null) return;

        var model = playerController.Model;

        // 개별 UI 뷰들에게 모델을 직접 바인딩 (DLV UIView Pattern)
        if (hpInLineUI != null) hpInLineUI.Bind(model);
        if (circleExpUI != null) circleExpUI.Bind(model);
        if (expPanel != null) expPanel.Bind(model);
        
        // ChoiceSystem 바인딩 (플레이어 모델 연결)
        var choiceSystem = ChoiceSystem.Instance;
        if (choiceSystem != null)
        {
            choiceSystem.Bind(model);
        }

        // [New] ResultUIManager 바인딩
        if (ResultUIManager.Instance != null)
        {
            ResultUIManager.Instance.Bind(model);
        }

        // 로직 시스템 관련 이벤트 구독
        model.OnLevelUp += OnLevelUp;
        model.OnDead += OnPlayerDead;
    }
    
    private void UnSubScribe()
    {
        if (playerController == null || playerController.Model == null) return;

        playerController.Model.OnLevelUp -= OnLevelUp;
        playerController.Model.OnDead -= OnPlayerDead;
    }
    
    private void OnLevelUp(int newLevel)
    {
        ExpManager.Instance?.PlayerLevelUp();
        
        // 레벨업 사운드 재생 (2D)
        if (playerController.Model.PureData.LevelUpSound != null)
        {
            AudioManager.Instance?.Play2D(playerController.Model.PureData.LevelUpSound);
        }
    }

    private void OnPlayerDead()
    {
        // 사망 사운드 재생 (2D)
        if (playerController.Model.PureData.DeathSound != null)
        {
            AudioManager.Instance?.Play2D(playerController.Model.PureData.DeathSound);
        }
    }
}
