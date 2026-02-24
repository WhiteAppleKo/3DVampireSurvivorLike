using System.Collections.Generic;
using Features.Weapon;
using Features.Augment;
using UnityEngine;

namespace _02.Scripts.AutoAttack
{
    [RequireComponent(typeof(IWeaponVisualizer))]
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected PureDataWeapon pureData;
        public PureDataWeapon PureData => pureData; // PureData 접근자
        
        protected RuntimeDataWeapon model;
        public RuntimeDataWeapon Model => model;    // RuntimeData(Model) 접근자 (DLV 핵심)

        protected IWeaponVisualizer visuals;
        public LayerMask TargetLayer { get; set; }

        protected WeaponVisualTheme VisualTheme; // 주입받은 시각 테마

        private global::Features.Weapon.WeaponModifier m_GlobalAugmentsModifier;

        public virtual void WeaponAwake()
        {
            visuals = GetComponent<IWeaponVisualizer>();
            
            if (pureData != null)
            {
                model = new RuntimeDataWeapon(pureData);
            }
            else
            {
                Debug.LogError($"[Weapon] {name}에 PureData가 할당되지 않았습니다! 기능이 작동하지 않습니다.");
            }

            var autoAttacker = GetComponentInParent<global::AutoAttack>();
            if (autoAttacker != null)
            {
                TargetLayer = autoAttacker.TargetLayer;
                VisualTheme = autoAttacker.VisualTheme; // 부모로부터 테마 주입
            }
            
            // 글로벌 증강 초기화
            m_GlobalAugmentsModifier = new global::Features.Weapon.WeaponModifier(1, 1, 1);
            
            RecalculateStats();
            WeaponSettingLogic();
        }

        public void SetGlobalAugments(global::Features.Weapon.WeaponModifier modifier)
        {
            m_GlobalAugmentsModifier = modifier;
            RecalculateStats();
        }

        /// <summary>
        /// 외부에서 테마를 강제로 갱신할 때 사용합니다.
        /// </summary>
        public void UpdateTheme(WeaponVisualTheme theme)
        {
            VisualTheme = theme;
            OnThemeChanged();
        }

        /// <summary>
        /// 테마가 변경되었을 때 하위 클래스에서 처리할 로직을 구현합니다.
        /// </summary>
        public virtual void OnThemeChanged() { }

        public void ApplyPureAugment(PureDataWeaponAbility ability)
        {
            if (model == null) return;
            model.ApplyPureAugment(ability);
        }

        protected virtual void RecalculateStats()
        {
            if (model == null) return;

            // 1. 글로벌 증강 적용 (누적이 아닌 설정 방식으로 호출)
            model.SetGlobalModifier(
                m_GlobalAugmentsModifier.fixedDamageIncrease,
                m_GlobalAugmentsModifier.percentDamageIncreadse - 1.0f,
                m_GlobalAugmentsModifier.percentAttackDelay - 1.0f
            );
        }


        public abstract void WeaponSettingLogic();

        public virtual void AttackLogic()
        {
            if (model == null) return;

            // Visual 실행
            visuals.PlayAttackAnimation();
            if (pureData != null) visuals.PlayAttackSound(pureData.AttackSound);

            // 쿨타임 설정
            model.SetCooldown(model.FinalAttackDelay);
        }

        protected virtual void Update()
        {
            model?.Tick(Time.deltaTime);
        }
    }
}