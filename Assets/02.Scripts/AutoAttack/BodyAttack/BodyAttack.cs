using System.Collections.Generic;
using _02.Scripts.Cotroller;
using UnityEngine;

namespace _02.Scripts.AutoAttack.BodyAttack
{
    public class BodyAttack : Weapon
    {
        // 성능 최적화를 위한 버퍼 (최대 100마리까지 동시 타격 가능하게 설정)
        //private Collider[] m_ColliderBuffer = new Collider[100];
        private BattleManager.DamageEventStruct m_DamageEvent;
        private Controller m_Controller;
        private BoxCollider m_BoxCollider;
        public override void WeaponSettingLogic()
        {
            m_Controller = GetComponentInParent<Controller>();
            m_BoxCollider = GetComponent<BoxCollider>();
            if (!m_BoxCollider.isTrigger)
            {
                Debug.LogWarning("충돌 감지를 위해 활성화합니다.");
                m_BoxCollider.isTrigger = true;
            }
            m_BoxCollider.size = m_Controller.transform.localScale;
        }

        protected override void RecalculateStats()
        {
            base.RecalculateStats();
        }

        public override void AttackLogic()
        {
            // 베이스 클래스의 AttackLogic을 호출하여 증강의 OnAttack 효과를 발동시킵니다.
            base.AttackLogic();
            
            BoxCollider col = GetComponent<BoxCollider>();
            Vector3 trueSize = Vector3.Scale(col.size, transform.lossyScale) * 0.5f;
            // 위치, 절반 크기, 회전값을 내 몸과 똑같이 맞춰서 검출
            Collider[] hits = Physics.OverlapBox(
                transform.position + transform.TransformDirection(col.center), 
                trueSize, 
                transform.rotation, 
                TargetLayer
            );
            
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent<Controller>(out var enemy))
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
