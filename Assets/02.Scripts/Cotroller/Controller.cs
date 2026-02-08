using _02.Scripts.Cotroller;
using UnityEngine;

namespace _02.Scripts.Cotroller
{
    // [L] Base Logic System for all Entities
    public abstract class Controller : MonoBehaviour
    {
        [Header("Base References")]
        public global::AutoAttack autoAttacker;
        
        public bool isMoveDisable = false;
        public bool isDashing = false;

        protected virtual void Awake()
        {
            // [DLV Refactored] Legacy stats removed. 
            // Child classes should initialize their own Models here.
        }

        protected virtual void OnEnable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent += OnDamageReceived;
            }
        }

        protected virtual void OnDisable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent -= OnDamageReceived;
            }
        }

        protected virtual void OnDamageReceived(BattleManager.DamageEventStruct damageEvent)
        {
            if (damageEvent.receiver != this) return;
            
            // 로직: 데미지 계산 및 데이터 갱신
            // 실제 구현에서는 각 클래스의 Model.TakeDamage()를 호출하도록 유도하는 것이 좋습니다.
            ApplyDamage(damageEvent.damageAmount);
        }

        protected abstract void ApplyDamage(int amount);

        protected abstract void Die(int prev, int current);

        // DLV Interface for shared stats
        public abstract float CurrentMoveSpeed { get; }
    }
}