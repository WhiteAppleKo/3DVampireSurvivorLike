using UnityEngine;

public class Controller : MonoBehaviour
{
    public AutoAttack autoAttacker;
    public ClampInt hp;
    public int maxHp = 100;
    
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float turnSpeed = 10.0f;

    protected virtual void Awake()
    {
        hp = new ClampInt(0, maxHp, maxHp);
    }

    protected virtual void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.onDamageEvent += OnDamageReceived;
        }

        hp.Events.onMinReached += Die;
    }

    protected virtual void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.onDamageEvent -= OnDamageReceived;
        }
        hp.Events.onMinReached -= Die;
    }
    
    protected virtual void OnDamageReceived(BattleManager.DamageEventStruct damageEvent)
    {
        
        if (damageEvent.receiver != this) return;
        Debug.Log($"데미지 받음 {gameObject.name}");
        hp.Decrease(damageEvent.damageAmount);
    }
    
    protected virtual void Die(int prev, int current)
    {
        Debug.Log($"{gameObject.name} 사망");
    }
}
