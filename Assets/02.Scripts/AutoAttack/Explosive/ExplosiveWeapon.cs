using System.Collections.Generic;
using _02.Scripts.Cotroller;
using _02.Scripts.Managers;
using UnityEngine;

namespace _02.Scripts.AutoAttack.Explosive
{
    /// <summary>
    /// HexGridRenderer의 Charge 기능을 사용하여 지연 폭발을 일으키는 무기입니다.
    /// </summary>
    public class ExplosiveWeapon : Weapon
    {
        [Header("Explosive Settings")]
        [SerializeField, ColorUsage(true, true)] 
        private Color m_ChargeColor = Color.white * 3f;
        
        [SerializeField, ColorUsage(true, true)]
        private Color m_ExplosionColor = Color.yellow * 5f;

        [SerializeField, Range(0.1f, 2.0f)]
        private float m_ExplosionDuration = 0.4f;

        private Controller m_Controller;
        private HexGridRenderer m_HexGridRenderer;

        public override void WeaponSettingLogic()
        {
            m_Controller = GetComponentInParent<Controller>();
            
            // 전역 HexGridRenderer 참조 (싱글톤이 아니므로 씬에서 찾거나 캐싱 필요)
            m_HexGridRenderer = FindObjectOfType<HexGridRenderer>();
            
            if (m_HexGridRenderer == null)
            {
                Debug.LogError($"[ExplosiveWeapon] {name}이 HexGridRenderer를 찾을 수 없습니다!");
            }
        }

        public override void AttackLogic()
        {
            if (m_HexGridRenderer == null) return;

            // 베이스 클래스의 AttackLogic을 호출하여 애니메이션 및 공통 효과 발동
            base.AttackLogic();

            // 1. 폭발 위치와 범위 결정
            Vector3 explosionCenter = transform.position;
            int range = (int)Model.FinalEffectRange;
            float duration = Model.FinalAttackDelay;

            // 2. 주입받은 테마 색상 적용 (없을 경우 인스펙터 기본값 사용)
            Color themeColor = (VisualTheme.themeColor != Color.clear) ? VisualTheme.themeColor : m_ChargeColor;

            // 3. HexGridRenderer를 통해 차징 시작
            m_HexGridRenderer.StartCharge(
                explosionCenter, 
                range, 
                duration, 
                TargetLayer, 
                (targets) => OnDetonate(targets, explosionCenter), // 중심점 전달
                themeColor
            );
            
            Debug.Log($"[ExplosiveWeapon] {pureData.Name} 폭발 차징 시작 (시간: {duration}s, 범위: {range})");
        }

        /// <summary>
        /// 차징이 완료되었을 때 호출되는 폭발 로직입니다.
        /// </summary>
        private void OnDetonate(List<Collider> targets, Vector3 center)
        {
            // 1. 시각적 폭발 효과 재생 (Shapes)
            if (m_HexGridRenderer != null)
            {
                // 실제 판정 범위인 그리드 칸 수(int range)를 그대로 전달하여 시각 효과 범위를 일치시킴
                int range = (int)Model.FinalEffectRange;
                Color explosionColor = (VisualTheme.effectColor != Color.clear) ? VisualTheme.effectColor : m_ExplosionColor;
                m_HexGridRenderer.StartExplosion(center, range, explosionColor, m_ExplosionDuration);
            }

            // 2. 데미지 처리
            int damage = Model.FinalDamage;
            if (targets != null && targets.Count > 0)
            {
                foreach (var col in targets)
                {
                    if (col.TryGetComponent<Controller>(out var enemy))
                    {
                        if (enemy == m_Controller) continue;
                        if (enemy.gameObject.activeInHierarchy)
                        {
                            var damageEvent = new BattleManager.DamageEventStruct(damage, this, m_Controller, enemy);
                            BattleManager.Instance.ProcessDamage(damageEvent);
                        }
                    }
                }
            }
            
            // 3. 폭발 효과음 재생 (풀링 사용)
            if (pureData.AttackSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(pureData.AttackSound, center);
            }
            
            Debug.Log($"[ExplosiveWeapon] {pureData.Name} 폭발! 타격 수: {(targets != null ? targets.Count : 0)}");
        }
    }
}
