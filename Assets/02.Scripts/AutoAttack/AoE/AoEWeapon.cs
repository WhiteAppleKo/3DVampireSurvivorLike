using System.Collections.Generic;
using _02.Scripts.Cotroller;
using UnityEngine;

// 이 컴포넌트는 SphereCollider를 필요로 합니다.
namespace _02.Scripts.AutoAttack.AoE
{
    [RequireComponent(typeof(SphereCollider))]
    public class AoEWeapon : Weapon
    {
        // 성능 최적화를 위한 버퍼 (최대 100마리까지 동시 타격 가능하게 설정)
        //private Collider[] m_ColliderBuffer = new Collider[100];
        private BattleManager.DamageEventStruct m_DamageEvent;
        private Controller m_Controller;
        private HexGridRenderer m_HexGridRenderer;
    
        public override void WeaponSettingLogic()
        {
            m_Controller = GetComponentInParent<Controller>();
            // 콜라이더가 트리거로 설정되어 있는지 확인
            var collider = GetComponent<SphereCollider>();
            if (!collider.isTrigger)
            {
                Debug.LogWarning("AoEWeapon의 SphereCollider가 isTrigger=true가 아닙니다. 충돌 감지를 위해 활성화합니다.");
                collider.isTrigger = true;
            }
        
            // 초기 반지름 설정 (DLV)
            if (Model != null)
            {
                collider.radius = Model.FinalEffectRange;
            }

            m_HexGridRenderer = GetComponent<HexGridRenderer>();
        }

        protected override void RecalculateStats()
        {
            base.RecalculateStats();
            
            if (m_HexGridRenderer == null) m_HexGridRenderer = GetComponent<HexGridRenderer>();
            
            if (m_HexGridRenderer != null)
            {
                // DLV Refactoring: Use Model.FinalEffectRange
                m_HexGridRenderer.aoeRange = (int)Model.FinalEffectRange;
            }
        }

        public override void AttackLogic()
        {
            // 베이스 클래스의 AttackLogic을 호출하여 증강의 OnAttack 효과를 발동시킵니다.
            base.AttackLogic();

            // 증강이 적용된 반지름 사용 (DLV)
            float radius = Model.FinalEffectRange;
        
            List<Collider> scanedCollider = m_HexGridRenderer.ScanTargets(gameObject.transform.position, (int)radius, TargetLayer);
            // 현재 위치 기준 반경 내의 모든 콜라이더 검출 (LayerMask를 지정하면 더 효율적임)
            // int count = Physics.OverlapSphereNonAlloc(transform.position, radius, m_ColliderBuffer);
        
            for (int i = 0; i < scanedCollider.Count; i++)
            {
                //Collider col = m_ColliderBuffer[i];
                Collider col = scanedCollider[i];

                if (col.TryGetComponent<Controller>(out var enemy))
                {
                    if (enemy == m_Controller)
                    {
                        continue;
                    }
                    if (enemy.gameObject.activeInHierarchy)
                    {
                        // 증강이 적용된 최종 데미지(Model.FinalDamage)를 사용합니다.
                        int damage = Model.FinalDamage;
                        m_DamageEvent = new BattleManager.DamageEventStruct(damage, this, m_Controller, enemy);
                        BattleManager.Instance.ProcessDamage(m_DamageEvent);
                    }
                }
            }
        }
    }
}
