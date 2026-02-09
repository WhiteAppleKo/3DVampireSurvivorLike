using System;
using UnityEngine;

namespace Features.Enemy
{
    public class RuntimeDataEnemy
    {
        public PureDataEnemy PureData { get; private set; }
        
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public float MoveSpeed { get; private set; }
        public float TurnSpeed { get; private set; }
        public int ExpAmount { get; private set; }
        public LayerMask TargetLayer => PureData.TargetLayer;

        public event Action<int, int> OnHpChanged;
        public event Action OnDeath;

        public RuntimeDataEnemy(PureDataEnemy pureData)
        {
            PureData = pureData;
            Reset();
        }

        public void Reset()
        {
            MaxHp = PureData.BaseMaxHp;
            CurrentHp = MaxHp;
            MoveSpeed = PureData.BaseMoveSpeed;
            TurnSpeed = PureData.BaseTurnSpeed;
            ExpAmount = PureData.BaseExpAmount;
        }

        public void ApplyTimeScale(float elapsedTime)
        {
            // 기존 로직 유지: 시간 경과에 따른 HP 증가
            float scale = elapsedTime / 2.0f;
            int hpIncrease = Mathf.FloorToInt(PureData.BaseMaxHp * scale);
            MaxHp = PureData.BaseMaxHp + hpIncrease;
            CurrentHp = MaxHp;
        }

        public void TakeDamage(int amount)
        {
            int prevHp = CurrentHp;
            CurrentHp = Mathf.Max(0, CurrentHp - amount);
            
            OnHpChanged?.Invoke(prevHp, CurrentHp);

            if (CurrentHp <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}
