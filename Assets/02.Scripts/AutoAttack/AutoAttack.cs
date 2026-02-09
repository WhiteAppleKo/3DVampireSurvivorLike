using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Save;
using Features.Augment;
using Features.Weapon;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AutoAttack : MonoBehaviour, ISaveable
{
    private RuntimeDataInventory m_Inventory;
    public RuntimeDataInventory Inventory => m_Inventory;

    // 공격 대상 레이어 (플레이어 모델로부터 결정됨)
    public LayerMask TargetLayer { get; private set; }

    // [V] 실제 무기 인스턴스 관리
    private Dictionary<RuntimeDataWeapon, Weapon> m_WeaponInstances = new Dictionary<RuntimeDataWeapon, Weapon>();
    private CancellationTokenSource m_Cts;

    public void GameStart()
    {
        m_Inventory = new RuntimeDataInventory();
        
        // 1. 플레이어로부터 레이어 설정 시도
        var player = GetComponentInParent<PlayerController>();
        if (player != null && player.Model != null)
        {
            TargetLayer = player.Model.TargetLayer;
        }
        else
        {
            // 2. 플레이어가 아니면 몬스터로부터 레이어 설정 시도
            var enemy = GetComponentInParent<Features.Enemy.EnemyLogicSystem>();
            if (enemy != null && enemy.Model != null)
            {
                TargetLayer = enemy.Model.TargetLayer;
            }
        }
        
        // 씬 시작 시 이미 붙어있는 무기들 등록
        foreach(Weapon weaponInChildren in GetComponentsInChildren<Weapon>())
        {
            RegisterWeapon(weaponInChildren);
        }
    }
    
    public void AddPureAugment(PureDataWeaponAbility augment)
    {
        if (m_Inventory == null) return;
        m_Inventory.AddGlobalAugment(augment);
    }

    private void OnEnable()
    {
        ResetCts();
        StartAllAttacks();
    }

    private void OnDisable()
    {
        CancelCts();
    }

    private void ResetCts()
    {
        CancelCts();
        m_Cts = new CancellationTokenSource();
    }

    private void CancelCts()
    {
        if (m_Cts != null)
        {
            m_Cts.Cancel();
            m_Cts.Dispose();
            m_Cts = null;
        }
    }

    public void StartAllAttacks()
    {
        if (m_Cts == null) return;
        foreach (var weapon in m_WeaponInstances.Values)
        {
            Async_AutoAttack(weapon, m_Cts.Token).Forget();
        }
    }

    public void AddWeapon(Weapon newWeapon)
    {
        if (m_Inventory == null || !m_Inventory.CanAddWeapon)
        {
            Debug.Log("더 이상 무기를 추가할 수 없습니다.");
            return;
        }

        RegisterWeapon(newWeapon);
        
        if (gameObject.activeInHierarchy && m_Cts != null)
        {
            Async_AutoAttack(newWeapon, m_Cts.Token).Forget();
        }
    }

    private void RegisterWeapon(Weapon weapon)
    {
        weapon.WeaponAwake();
        // Weapon은 이미 RuntimeDataWeapon(model)을 가지고 있음
        if (weapon.Model != null)
        {
            m_Inventory.AddWeaponModel(weapon.Model);
            m_WeaponInstances[weapon.Model] = weapon;
        }
    }

    private async UniTaskVoid Async_AutoAttack(Weapon weapon, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            float delay = weapon.Model != null ? weapon.Model.FinalAttackDelay : 1.0f;
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

            if (token.IsCancellationRequested) break;
            
            weapon.AttackLogic();
        }
    }

    public void SaveData()
    {
        if (m_Inventory == null) return;

        // 1. 글로벌 증강 ID 수집
        List<string> augmentsID = new List<string>();
        foreach (var augment in m_Inventory.GlobalAugments)
        {
            augmentsID.Add(augment.ID);
        }
        
        // 2. 무기 정보 수집
        List<WeaponSaveData> weaponList = new List<WeaponSaveData>();
        foreach (var weapon in m_WeaponInstances.Values)
        {
            weaponList.Add(new WeaponSaveData(weapon.PureData.ID, new List<string>()));
        }
        
        AutoAttackerSaveData saveData = new AutoAttackerSaveData(augmentsID, weaponList);
        DataHub.Instance.SetWeaponData(saveData);
    }

    public void LoadData()
    {
        // 1. 기존 데이터 및 인스턴스 청소
        if (m_Inventory != null) m_Inventory.Clear();
        foreach (var w in m_WeaponInstances.Values)
        {
            if (w != null) Destroy(w.gameObject);
        }
        m_WeaponInstances.Clear();

        AutoAttackerSaveData saveData = DataHub.Instance.LoadAutoAttackerSaveData();
        
        if (saveData == null || saveData.weaponList == null || saveData.weaponList.Count == 0)
        {
            if (ExpManager.Instance != null) ExpManager.Instance.ChoiceFirstWeapon();
            return;
        }

        // 2. 글로벌 증강 먼저 복구 (나중에 무기 추가 시 소급 적용됨)
        foreach (var id in saveData.globalWeaponAugments)
        {
            var augment = DataHub.Instance.GetWeaponAbilityData(id);
            if (augment != null) m_Inventory.AddGlobalAugment(augment);
        }
        
        // 3. 무기 인스턴스 복구
        foreach (var wSave in saveData.weaponList)
        {
            var pWeapon = DataHub.Instance.GetWeaponData(wSave.weaponID);
            if (pWeapon != null && pWeapon.Prefab != null)
            {
                var wObj = Instantiate(pWeapon.Prefab, transform);
                var wComp = wObj.GetComponent<Weapon>();
                AddWeapon(wComp);
            }
        }
    }
}
