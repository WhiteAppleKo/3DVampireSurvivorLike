using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public Weapon[] weapon;
    
    private PlayerController m_PlayerController;
    private int m_WeaponCount = 0;

    public void GameStart()
    {
        weapon = new Weapon[5];
        m_PlayerController = GetComponentInParent<PlayerController>();
        AddWeapon(m_PlayerController.passiveWeapon);
    }

    public void AddWeapon(Weapon newWeapon)
    {
        if (m_WeaponCount > 5)
        {
            Debug.Log("더 이상 무기를 추가할 수 없습니다.");
            return;
        }
        weapon[m_WeaponCount] = newWeapon;
        newWeapon.WeaponSettingLogic();
        StartCoroutine(co_AutoAttack(weapon[m_WeaponCount]));
        m_WeaponCount++;
    }

    private IEnumerator co_AutoAttack(Weapon weapon)
    {
        while (true)
        {
            yield return new WaitForSeconds(weapon.attackDelay);
            weapon.AttackLogic();
        }
    }
}
