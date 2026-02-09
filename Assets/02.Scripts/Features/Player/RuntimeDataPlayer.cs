using System;
using Features.Player;
using UnityEngine;

namespace Features.Player
{
    public class RuntimeDataPlayer
    {
        public PureDataPlayer PureData { get; private set; }

        // 가변 상태
        public int CurrentLevel { get; private set; }
        public int CurrentExp { get; private set; }
        public int MaxExp { get; private set; }
        public int CurrentHp { get; private set; }
        public int MaxHp { get; private set; }
        public float MoveSpeed { get; private set; }
        public LayerMask TargetLayer => PureData.TargetLayer;

        // 증강 누적치
        private int _maxHpAdded = 0;
        private float _moveSpeedMultiplier = 1.0f;

        // 이벤트
        public event Action<int, int> OnHpChanged; // (current, max)
        public event Action<int, int> OnExpChanged; // (current, max)
        public event Action<int> OnLevelUp;

        public RuntimeDataPlayer(PureDataPlayer pureData)
        {
            PureData = pureData;
            Reset();
        }

        public void Reset()
        {
            CurrentLevel = 1;
            CurrentExp = 0;
            MaxExp = PureData.BaseExpToLevelUp;
            
            _maxHpAdded = 0;
            _moveSpeedMultiplier = 1.0f;
            
            RecalculateStats();
            CurrentHp = MaxHp; // Reset 시 체력 풀 회복

            NotifyAll();
        }

        public void AddMaxHpModifier(int amount)
        {
            _maxHpAdded += amount;
            RecalculateStats();
            // 최대 체력이 늘어난 만큼 현재 체력도 채워준다 (선택 사항)
            CurrentHp += amount; 
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void AddMoveSpeedModifier(float multiplierAdd)
        {
            _moveSpeedMultiplier += multiplierAdd;
            RecalculateStats();
        }

        private void RecalculateStats()
        {
            MaxHp = PureData.BaseMaxHp + _maxHpAdded;
            MoveSpeed = PureData.BaseMoveSpeed * _moveSpeedMultiplier;
        }

        public void AddExp(int amount)
        {
            CurrentExp += amount;
            while (CurrentExp >= MaxExp)
            {
                LevelUp();
            }
            OnExpChanged?.Invoke(CurrentExp, MaxExp);
        }

        private void LevelUp()
        {
            CurrentExp -= MaxExp;
            CurrentLevel++;
            MaxExp += PureData.ExpIncreasePerLevel;
            OnLevelUp?.Invoke(CurrentLevel);
        }

        public void TakeDamage(int damage)
        {
            CurrentHp = Math.Max(0, CurrentHp - damage);
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void Heal(int amount)
        {
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        public void UpdateStats(int hpAdd, float speedMult)
        {
            MaxHp = PureData.BaseMaxHp + hpAdd;
            MoveSpeed = PureData.BaseMoveSpeed * speedMult;
            
            // HP 상한 증가 시 현재 HP도 비례해서 늘려줄지 등 정책 결정 필요
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
        }

        private void NotifyAll()
        {
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
            OnExpChanged?.Invoke(CurrentExp, MaxExp);
        }
    }
}
