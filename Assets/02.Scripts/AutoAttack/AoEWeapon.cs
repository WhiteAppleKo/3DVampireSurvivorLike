using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AoEWeapon : Weapon
{
    public float areaOfEffectRadius = 5.0f;

    private List<EnemyController> m_EnemiesInRange;
    private BattleManager.DamageEventStruct m_DamageEvent;
    private Controller m_Controller;
    
    public override void WeaponSettingLogic()
    {
        m_EnemiesInRange = new List<EnemyController>();
        m_Controller = GetComponentInParent<Controller>();
    }
    public override void AttackLogic()
    {
        Debug.Log($"이 컴포넌트의 클래스 이름은 '{this.GetType().Name}' 입니다.");
        foreach (var enemy in m_EnemiesInRange)
        {
            if (enemy != null)
            {
                Debug.Log($"적 {enemy.name} 이(가) AoE 공격을 받았습니다.");
                m_DamageEvent = new BattleManager.DamageEventStruct(damage, this, m_Controller, enemy);
                BattleManager.Instance.BroadcastDamageEvent(m_DamageEvent);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemyController = other.GetComponent<EnemyController>();
        m_EnemiesInRange.Add(enemyController);
    }
}
