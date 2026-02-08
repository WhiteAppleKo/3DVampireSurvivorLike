using System;
using System.Collections.Generic;
using _02.Scripts.AutoAttack; // 기존 네임스페이스 참조
using UnityEngine;

namespace Features.Weapon
{
    [System.Serializable]
    public class WeaponModifier
    {
        public int fixedDamageIncrease;
        public float percentDamageIncreadse;
        public float percentAttackDelay;

        public WeaponModifier(int a, float b, float c)
        {
            fixedDamageIncrease = a;
            percentDamageIncreadse = b;
            percentAttackDelay = c;
        }

        public void Set(int a, float b, float c)
        {
            fixedDamageIncrease = a;
            percentDamageIncreadse = b;
            percentAttackDelay = c;
        }
    }

    public class RuntimeDataWeapon
    {
            public PureDataWeapon PureData { get; private set; }
    
            public float CurrentCooldown { get; private set; }
            
            public float FinalAttackDelay { get; private set; }
            public int FinalDamage { get; private set; }
            public float FinalEffectRange { get; private set; }
            public int FinalProjectileCount { get; private set; }

        // 증강 누적치 (아이템/스페셜 증강)
        private float _attackDelayMultiplier = 1.0f;
        private int _damageAdded = 0;
        private float _damageMultiplier = 1.0f;
        private float _effectRangeMultiplier = 1.0f;

        // 글로벌 모디파이어 (AutoAttack에서 관리하는 전역 수치)
        private float _globalAttackDelayModifier = 0f;
        private int _globalDamageAdded = 0;
        private float _globalDamageMultiplier = 0f;

        public event Action OnStatsChanged;
        public event Action<float> OnCooldownChanged;

        public RuntimeDataWeapon(PureDataWeapon pureData)
        {
            PureData = pureData;
            ResetStats();
        }

        public void ResetStats()
        {
            if (PureData == null) return;

            _attackDelayMultiplier = 1.0f;
            _damageAdded = 0;
            _damageMultiplier = 1.0f;
            _effectRangeMultiplier = 1.0f;
            
            _globalAttackDelayModifier = 0f;
            _globalDamageAdded = 0;
            _globalDamageMultiplier = 0f;

            RecalculateStats();
            CurrentCooldown = 0f;

            OnStatsChanged?.Invoke();
        }

        public void SetGlobalModifier(int flatDamage, float percentDamage, float percentDelay)
        {
            // 누적이 아닌 "설정" 방식으로 변경하여 중복 합산 방지
            _globalDamageAdded = flatDamage;
            _globalDamageMultiplier = percentDamage;
            _globalAttackDelayModifier = percentDelay;
            RecalculateStats();
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

        public void AddAttackDelayModifier(float amount)
        {
            _attackDelayMultiplier += amount;
            RecalculateStats();
        }

        public void AddDamageModifier(int flatAmount, float percentAmount)
        {
            _damageAdded += flatAmount;
            _damageMultiplier += percentAmount;
            RecalculateStats();
        }

        public void AddRangeModifier(float amount)
        {
            _effectRangeMultiplier += amount;
            RecalculateStats();
        }

        private void RecalculateStats()
        {
            if (PureData == null) return;

            // 최종 수치 계산
            // 공격 속도(Speed)가 증가하면 딜레이(Delay)는 감소해야 하므로 나눗셈으로 변경
            float totalSpeedMultiplier = Mathf.Max(0.1f, _attackDelayMultiplier + _globalAttackDelayModifier);
            FinalAttackDelay = PureData.AttackDelay / totalSpeedMultiplier;
            
            // 딜레이가 너무 작아지지 않게 최소값 보정
            FinalAttackDelay = Mathf.Max(0.05f, FinalAttackDelay);

            FinalDamage = (int)((PureData.Damage + _damageAdded + _globalDamageAdded) * (_damageMultiplier + _globalDamageMultiplier));
            FinalEffectRange = PureData.EffectRange * _effectRangeMultiplier;
            FinalProjectileCount = PureData.ProjectileCount; 
            
            OnStatsChanged?.Invoke();
        }
    }
}
    