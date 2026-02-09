using System;
using System.Collections.Generic;
using _02.Scripts.Cotroller;
using _02.Scripts.Managers.Choice;
using UnityEngine;

public class ExpManager : SingletoneBase<ExpManager>
{
    [Header("Visual References")]
    [SerializeField] private _02.Scripts.Managers.Spawn.ExpSpawner expSpawner;
    
    private ChoiceSystem m_ChoiceManager;

    protected override void Awake()
    {
        base.Awake();
        m_ChoiceManager = GetComponentInChildren<ChoiceSystem>();
    }

    private void Start()
    {
        if (expSpawner == null) expSpawner = GetComponent<_02.Scripts.Managers.Spawn.ExpSpawner>();
    }

    public void SetExp(int amount, Vector3 pos)
    {
        if (expSpawner != null)
        {
            expSpawner.Spawn(amount, pos);
        }
    }

    public void SetTarget(ExpCristal exp)
    {
        // [DLV Refactoring] 이제 Spawner에서 생성 시 자동으로 Target을 설정하므로,
        // 이 함수는 레거시 호환을 위해 비워두거나 필요 시 보강합니다.
    }

    public void PlayerLevelUp()
    {
        // [DLV Refactoring] 모델에서 레벨업 처리는 완료됨.
        // ChoiceSystem이 Player.OnLevelUp 이벤트를 구독하여 UI를 띄우도록 변경되므로,
        // 여기서는 더 이상 UI 호출을 담당하지 않습니다.
    }

    public void ChoiceFirstWeapon()
    {
        Debug.Log("[ExpManager] ChoiceFirstWeapon 호출됨");
        // 최초 무기 선택이 필요한 경우 ChoiceSystem을 직접 호출할 수 있는 로직만 남깁니다.
        if (m_ChoiceManager != null)
        {
            m_ChoiceManager.SetWeaponChoiceMode();
        }
        else
        {
            Debug.LogError("[ExpManager] m_ChoiceManager가 Null입니다!");
        }
    }
}
