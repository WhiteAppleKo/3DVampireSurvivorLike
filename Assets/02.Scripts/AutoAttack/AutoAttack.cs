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
    public Weapon[] weapon;
    public LayerMask layer;
    
    private List<PureDataWeaponAbility> m_PureGlobalAugments = new List<PureDataWeaponAbility>();

    private Features.Weapon.WeaponModifier m_GlobalModifier;
    private int m_WeaponCount = 0;
    private CancellationTokenSource m_Cts;

    public void GameStart()
    {
        weapon = new Weapon[5];
        m_GlobalModifier = new Features.Weapon.WeaponModifier(0, 1, 1);
        foreach(Weapon weaponInChildren in GetComponentsInChildren<Weapon>())
        {
            AddWeapon(weaponInChildren);
        }
    }
    
    /// <summary>
    /// DLV: 무기에 새로운 PureData 증강을 추가합니다.
    /// </summary>
    public void AddPureAugment(PureDataWeaponAbility augment)
    {
        m_PureGlobalAugments.Add(augment);
        for (int i = 0; i < m_WeaponCount; i++)
        {
            if (weapon[i] != null)
            {
                weapon[i].ApplyPureAugment(augment);
            }
        }
    }

    private void OnEnable()
    {
        if (m_Cts != null)
        {
            m_Cts.Cancel();
            m_Cts.Dispose();
        }
        
        m_Cts = new CancellationTokenSource();
        StartAttack();
    }

    private void OnDisable()
    {
        if (m_Cts != null)
        {
            m_Cts.Cancel();
            m_Cts.Dispose();
            m_Cts = null;
        }
    }

    public void StartAttack()
    {
        for (int i = 0; i < m_WeaponCount; i++)
        {
            if (weapon[i] != null)
            {
                Async_AutoAttack(weapon[i], m_Cts.Token).Forget();
            }
        }
    }

    public void AddWeapon(Weapon newWeapon)
    {
        if (m_WeaponCount >= 5)
        {
            Debug.Log("더 이상 무기를 추가할 수 없습니다.");
            return;
        }
        weapon[m_WeaponCount] = newWeapon;
        newWeapon.WeaponAwake();
        newWeapon.SetGlobalAugments(m_GlobalModifier);
        
        // DLV: 기존에 획득한 Pure 증강들 적용
        foreach (var pureAugment in m_PureGlobalAugments)
        {
            newWeapon.ApplyPureAugment(pureAugment);
        }
        
        if (gameObject.activeInHierarchy && m_Cts != null)
        {
            Async_AutoAttack(newWeapon, m_Cts.Token).Forget();
        }
        
        m_WeaponCount++;
    }

    private async UniTaskVoid Async_AutoAttack(Weapon weapon, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            float delay = weapon.Model != null ? weapon.Model.FinalAttackDelay : 1.0f;
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

            if (token.IsCancellationRequested)
            {
                break;
            }
            weapon.AttackLogic();
        }
    }

    public void SaveData()
    {
        // 1. 글로벌 증강 ID 수집
        List<string> augmentsID = new List<string>();
        foreach (var augment in m_PureGlobalAugments)
        {
            if (augment != null) augmentsID.Add(augment.ID);
        }
        
        // 2. 무기 정보 수집
        List<WeaponSaveData> weaponList = new List<WeaponSaveData>();
        for (int i = 0; i < m_WeaponCount; i++)
        {
            if (weapon[i] == null) continue;
            
            // DLV: PureData ID 필수 사용
            string weaponID = weapon[i].PureData.ID;
            
            WeaponSaveData newWeaponSaveData = new WeaponSaveData(weaponID, new List<string>());
            weaponList.Add(newWeaponSaveData);
        }
        
        AutoAttackerSaveData saveData = new AutoAttackerSaveData(augmentsID, weaponList);
        DataHub.Instance.SetWeaponData(saveData);
    }

    public void LoadData()
    {
        // 기존 무기 및 증강 청소
        m_PureGlobalAugments.Clear();
        foreach (var w in weapon)
        {
            if (w != null) Destroy(w.gameObject);
        }
        System.Array.Clear(weapon, 0, weapon.Length);
        m_WeaponCount = 0;

        AutoAttackerSaveData saveData = DataHub.Instance.LoadAutoAttackerSaveData();
        
        if (saveData == null || saveData.weaponList == null || saveData.weaponList.Count == 0)
        {
            if (ExpManager.Instance != null) ExpManager.Instance.ChoiceFirstWeapon();
            return;
        }

        // 1. 글로벌 증강 복구 (DataHub 경유)
        foreach (var id in saveData.globalWeaponAugments)
        {
            var augment = DataHub.Instance.GetWeaponAbilityData(id);
            if (augment != null) m_PureGlobalAugments.Add(augment);
        }
        
        // 2. 무기 복구 (DataHub 경유)
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
