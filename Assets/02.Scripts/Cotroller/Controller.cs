using UnityEngine;

namespace _02.Scripts.Cotroller
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        public BaseStats baseStats = new BaseStats();
        public global::AutoAttack autoAttacker;
        public BaseStats FinalStats { get; protected set; }
        public bool isCharging = false;
        public bool isDashing = false;
    
        protected virtual void Awake()
        {
            baseStats.hp = new ClampInt(0, baseStats.maxHp, baseStats.maxHp);
            FinalStats = new BaseStats(baseStats);
        }

        protected virtual void OnEnable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent += OnDamageReceived;
            }

            FinalStats.hp.Events.onMinReached += Die;
        }

        protected virtual void OnDisable()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent -= OnDamageReceived;
            }
        
            if (FinalStats != null && FinalStats.hp != null)
            {
                FinalStats.hp.Events.onMinReached -= Die;
            }
        }
    
        protected virtual void OnDamageReceived(BattleManager.DamageEventStruct damageEvent)
        {
        
            if (damageEvent.receiver != this) return;
            Debug.Log(FinalStats.hp.Current);
            FinalStats.hp.Decrease(damageEvent.damageAmount);
            Debug.Log(FinalStats.hp.Current);
        }
    
        protected virtual void Die(int prev, int current)
        {
        
        }
    }
}
