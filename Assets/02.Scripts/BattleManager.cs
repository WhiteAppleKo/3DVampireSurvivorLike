using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class BattleManager : SingletoneBase<BattleManager>
{
    public struct DamageEventStruct
    {
        public int damageAmount;
        public Weapon senderWeapon;
        public Controller sender;
        public Controller receiver;

        public DamageEventStruct(int damage, Weapon weapon, object o, EnemyController enemy)
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
        Debug.Log("BattleManager 싱글톤 생성");
    }
    
    
    public void BroadcastDamageEvent(DamageEventStruct damageEvent)
    {
        Debug.Log("BattleManager 데미지 이벤트");
        onDamageEvent?.Invoke(damageEvent);
    }
}
