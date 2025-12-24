using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 이 컴포넌트는 SphereCollider를 필요로 합니다.
[RequireComponent(typeof(SphereCollider))]
public class AoEWeapon : Weapon
{
    // 참고: 실제 공격 범위는 이 무기 오브젝트에 부착된 SphereCollider의 반지름(radius)으로 결정됩니다.
    // 증강에 의해 finalStats.areaOfEffectRadius가 변경될 때마다, 해당 콜라이더의 반지름을 업데이트하는 로직이 필요할 수 있습니다.
    // 예: RecalculateStats() 메서드 마지막에 sphereCollider.radius = finalStats.areaOfEffectRadius; 추가

    private List<EnemyController> m_EnemiesInRange = new List<EnemyController>();
    private BattleManager.DamageEventStruct m_DamageEvent;
    private Controller m_Controller;
    
    public override void WeaponSettingLogic()
    {
        m_Controller = GetComponentInParent<Controller>();
        baseStats.aoeWeaponStats = new AoEWeaponStats();
        // 콜라이더가 트리거로 설정되어 있는지 확인
        var collider = GetComponent<SphereCollider>();
        if (!collider.isTrigger)
        {
            Debug.LogWarning("AoEWeapon의 SphereCollider가 isTrigger=true가 아닙니다. 충돌 감지를 위해 활성화합니다.");
            collider.isTrigger = true;
        }
    }
    public override void AttackLogic()
    {
        // 베이스 클래스의 AttackLogic을 호출하여 증강의 OnAttack 효과를 발동시킵니다.
        base.AttackLogic();

        Debug.Log($"이 컴포넌트의 클래스 이름은 '{this.GetType().Name}' 입니다.");
        
        // 리스트를 복사하여 순회중에 리스트가 변경되어도 에러가 발생하지 않도록 함
        var enemiesToAttack = new List<EnemyController>(m_EnemiesInRange);
        
        foreach (var enemy in enemiesToAttack)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                Debug.Log($"적 {enemy.name} 이(가) AoE 공격을 받았습니다.");
                // 증강이 적용된 최종 데미지(finalStats.damage)를 사용합니다.
                m_DamageEvent = new BattleManager.DamageEventStruct(finalStats.damage, this, m_Controller, enemy);
                BattleManager.Instance.BroadcastDamageEvent(m_DamageEvent);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyController>(out var enemyController))
        {
            if (!m_EnemiesInRange.Contains(enemyController))
            {
                m_EnemiesInRange.Add(enemyController);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EnemyController>(out var enemyController))
        {
            m_EnemiesInRange.Remove(enemyController);
        }
    }
}

