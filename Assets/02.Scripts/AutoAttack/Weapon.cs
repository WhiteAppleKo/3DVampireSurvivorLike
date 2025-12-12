using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 투사체를 관리하는 오브젝트 풀링 클래스입니다.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    public enum e_WeaponType
    {
        Projectile,
        AreaOfEffect,
    }
    public e_WeaponType weaponType;
    public float attackDelay = 1.0f;
    public int damage;
    
    public abstract void WeaponSettingLogic();
    public abstract void AttackLogic();
    
}
