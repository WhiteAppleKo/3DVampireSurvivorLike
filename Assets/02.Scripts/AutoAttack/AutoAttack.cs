using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Save;
using Features.Augment;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AutoAttack : MonoBehaviour, ISaveable
{
    public Weapon[] weapon;
    public LayerMask layer;
    
    private List<WeaponAbility> m_GlobalAugments = new List<WeaponAbility>();
    private List<PureDataWeaponAbility> m_PureGlobalAugments = new List<PureDataWeaponAbility>();

    private WeaponBaseStats.WeaponModifier m_GlobalModifier;
    private int m_WeaponCount = 0;
    private CancellationTokenSource m_Cts;

    public void GameStart()
    {
        weapon = new Weapon[5];
        m_GlobalModifier = new WeaponBaseStats.WeaponModifier(0, 1, 1);
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

    /// <summary>
    /// 무기에 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(WeaponAbility augment)
    {
        m_GlobalAugments.Add(augment);
        RecalculateGlobalAugemnt();
    }

    /// <summary>
    /// 무기에서 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(WeaponAbility augment)
    {
        m_GlobalAugments.Remove(augment);
        RecalculateGlobalAugemnt();
    }
    
    private void RecalculateGlobalAugemnt()
    {
        for (int i = 0; i < m_GlobalAugments.Count; i++)
        {
            m_GlobalAugments[i].Apply(m_GlobalModifier);
        }
        
        for (int i = 0; i < m_WeaponCount; i++)
        {
            weapon[i].SetGlobalAugments(m_GlobalModifier);
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
        
        /* 코루틴 코드
        StopAllCoroutines();
        StartAttack();*/
    }

    private void OnDisable()
    {
        if (m_Cts != null)
        {
            m_Cts.Cancel();
            m_Cts.Dispose();
            m_Cts = null;
        }
        // 코루틴 코드
        /*StopAllCoroutines();*/
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
        if (m_WeaponCount > 5)
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
        //무한 루프
        while (!token.IsCancellationRequested)
        {
            //딜레이 : TimeSpan 사용을 권장
            //weapon.FinalStats.AttackDelay는 float (초 단위)
            await UniTask.Delay(TimeSpan.FromSeconds(weapon.FinalStats.attackDelay), cancellationToken: token);

            if (token.IsCancellationRequested)
            {
                break;
            }
            weapon.AttackLogic();
        }
    }

    public List<WeaponAbility> GetWeaponGlobalAugments()
    {
        return m_GlobalAugments;
    }
    /*private IEnumerator co_AutoAttack(Weapon weapon)
    {
        while (true)
        {
            yield return new WaitForSeconds(weapon.FinalStats.attackDelay);
            weapon.AttackLogic();
        }
    }*/
    public void SaveData()
    {
        List<string> augmentsID = new List<string>();
        foreach (var augment in m_GlobalAugments)
        {
            augmentsID.Add(augment.abilityID);
        }
        
        List<WeaponSaveData> weaponList = new List<WeaponSaveData>();
        foreach (var weaponData in weapon)
        {
            if (weaponData == null)
            {
                continue;
            }
            WeaponSaveData newWeaponSaveData = new WeaponSaveData(
                weaponData.FinalStats.weaponID,
                weaponData.GetWeaponLocalAugmentsID());
            
            weaponList.Add(newWeaponSaveData);
        }
        
        AutoAttackerSaveData saveData = new AutoAttackerSaveData(
            augmentsID,
            weaponList);
        SaveManager.Instance.SetWeaponData(saveData);
        Debug.Log("AutoAttackSaveDAta");
    }

    public void LoadData()
    {
        m_GlobalAugments.Clear();
        AutoAttackerSaveData saveData = SaveManager.Instance.LoadAutoAttackerSaveData();
        
        // 데이터가 아예 없거나, 저장된 무기가 하나도 없을 때 첫 무기 선택 실행
        if (saveData == null || saveData.weaponList == null || saveData.weaponList.Count == 0)
        {
            if (ExpManager.Instance != null)
            {
                ExpManager.Instance.ChoiceFirstWeapon();
            }
            return;
        }

        m_GlobalAugments = SaveManager.Instance.GetWeaponAbilities(saveData.globalWeaponAugments);
        
        // WeaponDatabase에서 saveData.weaponList.Count만큼 id 검색해서 찾아와
        // 그 과정에서 Prefab을 하위로 생성하고 weaponList의 WeaponSaveData를 읽어서 내부를 채워
        for (int i = 0; i < saveData.weaponList.Count; i++)
        {
            WeaponSaveData weaponSaveData = saveData.weaponList[i];
            WeaponData loadWeaponData = SaveManager.Instance.GetWeaponData(weaponSaveData.weaponID);
            GameObject createWeapon = Instantiate(loadWeaponData.weaponPrefab,transform);
            Weapon weaponComponent = createWeapon.GetComponent<Weapon>();
            AddWeapon(weaponComponent);

            // TODO 스페셜 증강 가져오는 로직
            for (int j = 0; j < weaponSaveData.localWeaponAugments.Count; j++)
            {
                
            }
        }
        RecalculateGlobalAugemnt();
    }
}
