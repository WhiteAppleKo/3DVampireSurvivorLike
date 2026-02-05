using _02.Scripts.Cotroller;
using UnityEngine;

namespace _02.Scripts.Cotroller
{
    // [L] Base Logic System for all Entities
    public abstract class Controller : MonoBehaviour
    {
        [Header("Base References")]
        public BaseStats baseStats = new BaseStats();
        public global::AutoAttack autoAttacker;
        public BaseStats FinalStats { get; protected set; }
        
        public bool isMoveDisable = false;
        public bool isDashing = false;

        protected virtual void Awake()
        {
            // 하위 클래스에서 각자의 Model 및 Stats 초기화
            if (baseStats.hp == null)
            {
                baseStats.hp = new ClampInt(0, baseStats.maxHp, baseStats.maxHp);
            }
            FinalStats = new BaseStats(baseStats);
        }

        protected virtual void OnEnable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent += OnDamageReceived;
            }
            
            // HP 최소 도달 시 Die 호출 (이벤트 기반)
            if (FinalStats?.hp != null)
            {
                FinalStats.hp.Events.onMinReached += Die;
            }
        }

        protected virtual void OnDisable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent -= OnDamageReceived;
            }
        
            if (FinalStats?.hp != null)
            {
                FinalStats.hp.Events.onMinReached -= Die;
            }
        }

        protected virtual void OnDamageReceived(BattleManager.DamageEventStruct damageEvent)
        {
            if (damageEvent.receiver != this) return;
            
            // 로직: 데미지 계산 및 데이터 갱신
            // 실제 구현에서는 각 클래스의 Model.TakeDamage()를 호출하도록 유도하는 것이 좋습니다.
            ApplyDamage(damageEvent.damageAmount);
        }

        protected virtual void ApplyDamage(int amount)
        {
            if (FinalStats?.hp != null)
            {
                FinalStats.hp.Decrease(amount);
            }
        }

        protected abstract void Die(int prev, int current);
    }
}