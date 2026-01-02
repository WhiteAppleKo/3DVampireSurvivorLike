using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public Weapon[] weapon;
    
    private List<WeaponAbility> m_GlobalAugments = new List<WeaponAbility>();
    private int m_WeaponCount = 0;

    public void GameStart()
    {
        weapon = new Weapon[5];
        foreach(Weapon weaponInChildren in GetComponentsInChildren<Weapon>())
        {
            AddWeapon(weaponInChildren);
        }
    }
    
    /// <summary>
    /// 무기에 새로운 증강을 추가합니다.
    /// </summary>
    public void AddAugment(WeaponAbility augment)
    {
        m_GlobalAugments.Add(augment);
        RecalculateGlobalStats();
    }

    /// <summary>
    /// 무기에서 증강을 제거합니다.
    /// </summary>
    public void RemoveAugment(WeaponAbility augment)
    {
        m_GlobalAugments.Remove(augment);
        RecalculateGlobalStats();
    }
    
    private void RecalculateGlobalStats()
    {
        for (int i = 0; i < m_WeaponCount; i++)
        {
            weapon[i].SetGlobalAugments(m_GlobalAugments);
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
        newWeapon.SetGlobalAugments(m_GlobalAugments);
        StartCoroutine(co_AutoAttack(weapon[m_WeaponCount]));
        m_WeaponCount++;
    }

    private IEnumerator co_AutoAttack(Weapon weapon)
    {
        while (true)
        {
            yield return new WaitForSeconds(weapon.FinalStats.attackDelay);
            weapon.AttackLogic();
        }
    }
}
