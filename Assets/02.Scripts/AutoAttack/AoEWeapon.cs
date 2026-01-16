using System;
using System.Collections.Generic;
using _02.Scripts.AutoAttack;
using UnityEngine;
using UnityEngine.Serialization;

    // 이 컴포넌트는 SphereCollider를 필요로 합니다.
[RequireComponent(typeof(SphereCollider))]
public class AoEWeapon : Weapon
{
    // 참고: 실제 공격 범위는 이 무기 오브젝트에 부착된 SphereCollider의 반지름(radius)으로 결정됩니다.
    // 증강에 의해 finalStats.areaOfEffectRadius가 변경될 때마다, 해당 콜라이더의 반지름을 업데이트하는 로직이 필요할 수 있습니다.
    // 예: RecalculateStats() 메서드 마지막에 sphereCollider.radius = finalStats.areaOfEffectRadius; 추가

    // 성능 최적화를 위한 버퍼 (최대 100마리까지 동시 타격 가능하게 설정)
    private Collider[] m_ColliderBuffer = new Collider[100];
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
        
        // 초기 반지름 설정
        if (baseStats.aoeWeaponStats != null)
        {
            collider.radius = baseStats.aoeWeaponStats.areaOfEffectRadius;
        }
    }

    protected override void RecalculateStats()
    {
        base.RecalculateStats();
        
        // 증강 등으로 인해 스탯이 변경되면 콜라이더 크기도 업데이트
        var collider = GetComponent<SphereCollider>();
        if (collider != null && FinalStats.aoeWeaponStats != null)
        {
            collider.radius = FinalStats.aoeWeaponStats.areaOfEffectRadius;
        }
    }

    public override void AttackLogic()
    {
        // 베이스 클래스의 AttackLogic을 호출하여 증강의 OnAttack 효과를 발동시킵니다.
        base.AttackLogic();

        // 증강이 적용된 반지름 사용
        float radius = FinalStats.aoeWeaponStats.areaOfEffectRadius;
        
        // 현재 위치 기준 반경 내의 모든 콜라이더 검출 (LayerMask를 지정하면 더 효율적임)
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, m_ColliderBuffer);
        
        for (int i = 0; i < count; i++)
        {
            Collider col = m_ColliderBuffer[i];
            
            // 자기 자신은 제외 (필요시)
            if (col.gameObject == m_Controller.gameObject) continue;

            if (col.TryGetComponent<EnemyController>(out var enemy))
            {
                if (enemy.gameObject.activeInHierarchy)
                {
                    // 증강이 적용된 최종 데미지(finalStats.damage)를 사용합니다.
                    m_DamageEvent = new BattleManager.DamageEventStruct(FinalStats.damage, this, m_Controller, enemy);
                    BattleManager.Instance.BroadcastDamageEvent(m_DamageEvent);
                }
            }
        }
    }
}
