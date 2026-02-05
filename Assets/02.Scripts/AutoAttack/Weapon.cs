using System.Collections.Generic;
using Features.Weapon;
using UnityEngine;

namespace _02.Scripts.AutoAttack
{
    [RequireComponent(typeof(IWeaponVisualizer))]
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected WeaponData pureData;
        
        protected RuntimeDataWeapon model;
        protected IWeaponVisualizer visuals;
        public AudioSource audioSource;

        // --- 기존 프로퍼티 유지 (호환성용) ---
        public WeaponBaseStats baseStats = new WeaponBaseStats();
        public WeaponBaseStats FinalStats { get; protected set; }
        // ----------------------------------

        protected List<WeaponAbility> m_Augments = new List<WeaponAbility>();
        private WeaponBaseStats.WeaponModifier m_GlobalAugmentsModifier;

        public List<string> GetWeaponLocalAugmentsID()
        {
            List<string> IDList = new List<string>();
            foreach (var augment in m_Augments)
            {
                IDList.Add(augment.abilityID);
            }
            return IDList;
        }

        public List<WeaponAbility> GetWeaponLocalAugments()
        {
            return m_Augments;
        }

        public virtual void WeaponAwake()
        {
            visuals = GetComponent<IWeaponVisualizer>();
            audioSource = GetComponent<AudioSource>();
            
            if (pureData != null)
            {
                model = new RuntimeDataWeapon(pureData);
            }
            
            // 기존 시스템과의 호환성을 위한 초기화
            FinalStats = new WeaponBaseStats(baseStats);
            FinalStats.targetLayer = GetComponentInParent<global::AutoAttack>().layer;
            m_GlobalAugmentsModifier = new WeaponBaseStats.WeaponModifier(1, 1, 1);
            
            WeaponSettingLogic();
        }

        public void SetGlobalAugments(WeaponBaseStats.WeaponModifier modifier)
        {
            m_GlobalAugmentsModifier = modifier;
            RecalculateStats();
        }

        public virtual void AddAugment(WeaponAbility augment)
        {
            m_Augments.Add(augment);
            RecalculateStats();
        }

        public virtual void RemoveAugment(WeaponAbility augment)
        {
            m_Augments.Remove(augment);
            RecalculateStats();
        }

        protected virtual void RecalculateStats()
        {
            if (FinalStats == null) FinalStats = new WeaponBaseStats(baseStats);
            FinalStats.ResetTo(baseStats);

            // DLV Model 업데이트
            if (model != null)
            {
                model.UpdateStats(
                    m_GlobalAugmentsModifier.percentAttackDelay,
                    m_GlobalAugmentsModifier.fixedDamageIncrease,
                    m_GlobalAugmentsModifier.percentDamageIncreadse
                );

                // 호환성: FinalStats에도 적용
                FinalStats.attackDelay = model.FinalAttackDelay;
                FinalStats.damage = model.FinalDamage;
            }
        }

        public abstract void WeaponSettingLogic();

        public virtual void AttackLogic()
        {
            if (model == null) return;

            // Visual 실행
            visuals.PlayAttackAnimation();
            if (pureData != null) visuals.PlayAttackSound(pureData.AttackSound);

            // 증강 효과 실행
            foreach (var augment in m_Augments)
            {
                // augment.OnAttack(this); // 추후 증강 시스템 리팩토링 시 활성화
            }

            // 쿨타임 설정
            model.SetCooldown(model.FinalAttackDelay);
        }

        protected virtual void Update()
        {
            model?.Tick(Time.deltaTime);
        }
    }
}