using System;
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

        public DamageEventStruct(int damage, Weapon weapon, object o, Controller enemy)
        {
            damageAmount = damage;
            senderWeapon = weapon;
            sender = o as Controller;
            receiver = enemy;
        }
    }
    
    public Action<DamageEventStruct> onDamageEvent;
    
    protected override void Awake()
    {
        base.Awake();
       // m_Player = FindObjectOfType<PlayerController>();
    }
    
    
    public void BroadcastDamageEvent(DamageEventStruct damageEvent)
    {
        onDamageEvent?.Invoke(damageEvent);
    }
    
    public void BroadcastExpEvent(int expAmount)
    {
        player.baseStats.playerStats.AddExp(expAmount);
    }
}
