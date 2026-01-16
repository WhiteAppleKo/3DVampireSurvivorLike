using System;
using _02.Scripts.AutoAttack;

[Serializable]
public class WeaponBaseStats
{
    [Serializable]
    public class WeaponModifier
    {
        public int fixedDamageIncrease;
        public float percentDamageIncreadse;
        public float percentAttackDelay;

        public WeaponModifier (int a, float b, float c)
        {
            fixedDamageIncrease = a;
            percentDamageIncreadse = b;
            percentAttackDelay = c;
        }

        public void Set(int a, float b, float c)
        {
            fixedDamageIncrease = a;
            percentDamageIncreadse = b;
            percentAttackDelay = c;
        }
    }

    public string weaponID;
    public string weaponNamge;
    public float attackDelay = 1.0f;
    public int damage = 10;
    
    // ProjectileWeapon Stats
    public ProjectileWeaponStats projectileWeaponStats;
    
    // AoEWeapon Stats
    public AoEWeaponStats aoeWeaponStats;
    
    // 이 클래스를 복사하는 생성자
    public WeaponBaseStats(WeaponBaseStats other)
    {
        this.weaponID = other.weaponID;
        this.weaponNamge = other.weaponNamge;
        this.attackDelay = other.attackDelay;
        this.damage = other.damage;

        if (other.projectileWeaponStats != null)
        {
            this.projectileWeaponStats = new ProjectileWeaponStats();
            projectileWeaponStats.Set(other.projectileWeaponStats);
        }
        
        if (other.aoeWeaponStats != null)
        {
            this.aoeWeaponStats = new AoEWeaponStats();
            aoeWeaponStats.Set(other.aoeWeaponStats);
        }
    }

    public WeaponBaseStats()
    {
        
    }
    
    public void ResetTo(WeaponBaseStats other)
    {
        this.attackDelay = other.attackDelay;
        this.damage = other.damage;

        if (other.projectileWeaponStats != null)
        {
            this.projectileWeaponStats.Set(other.projectileWeaponStats);
        }

        if (other.aoeWeaponStats != null)
        {
            this.aoeWeaponStats.Set(other.aoeWeaponStats);
        }
    }

    public virtual void WeaponDataLoadLogic(WeaponData data)
    {
        weaponID = data.weaponID;
        weaponNamge = data.weaponName;
        attackDelay = data.attackDelay;
        damage = data.weaponDamage;

        if (projectileWeaponStats != null)
        {
            projectileWeaponStats.findTargetRange = data.effectRange;
            projectileWeaponStats.projectileCount = data.projectileCount;
        }

        if (aoeWeaponStats != null)
        {
            aoeWeaponStats.areaOfEffectRadius = data.effectRange;
        }
    }
}