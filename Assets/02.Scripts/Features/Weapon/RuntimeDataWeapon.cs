using System;
using System.Collections.Generic;
using _02.Scripts.AutoAttack; // 기존 네임스페이스 참조
using UnityEngine;

namespace Features.Weapon
{
    public class RuntimeDataWeapon
    {
        // 기존 WeaponData를 PureData로 사용
        public WeaponData PureData { get; private set; }

        public float CurrentCooldown { get; private set; }
        
        public float FinalAttackDelay { get; private set; }
        public int FinalDamage { get; private set; }
        public float FinalEffectRange { get; private set; }
        public int FinalProjectileCount { get; private set; }

        public event Action OnStatsChanged;
        public event Action<float> OnCooldownChanged;

        public RuntimeDataWeapon(WeaponData pureData)
        {
            PureData = pureData;
            ResetStats();
        }

        public void ResetStats()
        {
            if (PureData == null) return;

            FinalAttackDelay = PureData.attackDelay;
            FinalDamage = PureData.weaponDamage;
            FinalEffectRange = PureData.effectRange;
            FinalProjectileCount = PureData.projectileCount;
            CurrentCooldown = 0f;

            OnStatsChanged?.Invoke();
        }

        public void Tick(float deltaTime)
        {
            if (CurrentCooldown > 0)
            {
                CurrentCooldown -= deltaTime;
                OnCooldownChanged?.Invoke(CurrentCooldown);
            }
        }

        public void SetCooldown(float value)
        {
            CurrentCooldown = value;
            OnCooldownChanged?.Invoke(CurrentCooldown);
        }

        public void UpdateStats(float delayMult, int damageAdd, float damageMult)
        {
            if (PureData == null) return;
            
            FinalAttackDelay = PureData.attackDelay * delayMult;
            FinalDamage = (int)((PureData.weaponDamage + damageAdd) * damageMult);
            
            OnStatsChanged?.Invoke();
        }
    }
}