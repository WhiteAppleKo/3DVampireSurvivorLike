using System;
using _02.Scripts.AutoAttack;
using _02.Scripts.Cotroller;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class BattleManager : SingletoneBase<BattleManager>
{
    //private PlayerController m_Player;
    public PlayerController player;
    public struct DamageEventStruct
    {
        public int damageAmount;
        public Weapon senderWeapon;
        public Controller sender;
        public Controller receiver;

        public DamageEventStruct(int damage, Weapon weapon, Controller attacker, Controller victim)
        {
            damageAmount = damage;
            senderWeapon = weapon;
            sender = attacker;
            receiver = victim;
        }
    }
    
    public Action<DamageEventStruct> onDamageEvent;
    
    protected override void Awake()
    {
        base.Awake();
       // m_Player = FindObjectOfType<PlayerController>();
    }
    
    
    public void ProcessDamage(DamageEventStruct damageEvent)
    {
        // 1. [Logic] 최종 데미지 연산 (향후 크리티컬, 방어력 등 추가 가능)
        int finalDamage = damageEvent.damageAmount;

        // 2. [Logic] 피격자에게 직접 데미지 적용 (O(1) 성능 최적화)
        if (damageEvent.receiver != null)
        {
            damageEvent.receiver.ApplyDamage(finalDamage);
        }

        // 3. [Visual] 연출을 위한 이벤트 전파 (UI, 사운드, 텍스트 매니저 등만 구독)
        var processedEvent = damageEvent;
        processedEvent.damageAmount = finalDamage;
        onDamageEvent?.Invoke(processedEvent);
    }
    
    public void BroadcastExpEvent(int expAmount)
    {
        if (player != null && player.Model != null)
        {
            player.Model.AddExp(expAmount);
        }
    }
}
