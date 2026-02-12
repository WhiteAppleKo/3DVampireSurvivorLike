using System;
using System.Collections.Generic;
using _02.Scripts.Cotroller;
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
        
        // ChoiceSystem 바인딩 (싱글톤 호출로 인스턴스 강제 확보)
        var choiceSystem = ChoiceSystem.Instance;
        if (choiceSystem != null)
        {
            choiceSystem.Bind(model);
            
            // ChoiceUIView 바인딩 (선택지 UI)
            if (choiceUIView != null)
            {
                choiceUIView.Bind(choiceSystem);
            }
            else
            {
                Debug.LogWarning("[SubscribeManager] choiceUIView가 할당되지 않았습니다!");
            }
        }

        // 로직 시스템 관련 이벤트 구독
        model.OnLevelUp += OnLevelUp;
    }
    
    private void UnSubScribe()
    {
        if (playerController == null || playerController.Model == null) return;

        playerController.Model.OnLevelUp -= OnLevelUp;
    }
    
    private void OnLevelUp(int newLevel)
    {
        ExpManager.Instance?.PlayerLevelUp();
    }
}
