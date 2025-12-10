using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    private PlayerController m_PlayerController;
    private Weapon[] m_Weapon;
    private int m_WeaponCount = 0;

    public void GameStart()
    {
        m_Weapon = new Weapon[5];
        m_PlayerController = GetComponentInParent<PlayerController>();
        AddWeapon(m_PlayerController.passiveWeapon);
    }

    public void AddWeapon(Weapon newWeapon)
    {
        m_Weapon[m_WeaponCount] = newWeapon;
        StartCoroutine(co_AutoAttack(m_Weapon[m_WeaponCount]));
        m_WeaponCount++;
    }

    private IEnumerator co_AutoAttack(Weapon weapon)
    {
        while (true)
        {
            yield return new WaitForSeconds(weapon.attackDelay);
            weapon.Attack();
        }
    }
}
