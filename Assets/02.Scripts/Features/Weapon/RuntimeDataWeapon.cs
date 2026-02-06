using System;
using System.Collections.Generic;
using _02.Scripts.AutoAttack; // 기존 네임스페이스 참조
using UnityEngine;

namespace Features.Weapon
{
        public class RuntimeDataWeapon
        {
            public PureDataWeapon PureData { get; private set; }
    
            public float CurrentCooldown { get; private set; }
            
            public float FinalAttackDelay { get; private set; }
            public int FinalDamage { get; private set; }
            public float FinalEffectRange { get; private set; }
            public int FinalProjectileCount { get; private set; }

        // 증강 누적치
        private float _attackDelayMultiplier = 1.0f; // 공격 속도 증가 -> Delay 감소
        private int _damageAdded = 0;
        private float _damageMultiplier = 1.0f;
        private float _effectRangeMultiplier = 1.0f;

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

            RecalculateStats();
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

        public void AddAttackDelayModifier(float amount)
        {
            // 공격 속도 증가 = 딜레이 감소 (amount가 음수면 감소, 양수면 증가)
            // 기획 의도에 따라 amount가 "공격속도 증가율"인지 "딜레이 감소율"인지 파악 필요.
            // 여기서는 단순히 Multiplier에 합산.
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

            FinalAttackDelay = PureData.AttackDelay * _attackDelayMultiplier;
            // 딜레이가 너무 작아지지 않게 최소값 보정 (선택 사항)
            FinalAttackDelay = Mathf.Max(0.05f, FinalAttackDelay);

            FinalDamage = (int)((PureData.Damage + _damageAdded) * _damageMultiplier);
            FinalEffectRange = PureData.EffectRange * _effectRangeMultiplier;
            FinalProjectileCount = PureData.ProjectileCount; // 투사체 개수 증강이 있다면 여기도 수정 필요
            
            OnStatsChanged?.Invoke();
        }
    }
}
    