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
            MaxHp = PureData.BaseMaxHp;
            CurrentHp = MaxHp;
            MoveSpeed = PureData.BaseMoveSpeed;

            NotifyAll();
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
