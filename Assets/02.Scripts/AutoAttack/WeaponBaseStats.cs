using System;

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
    public float attackDelay = 1.0f;
    public int damage = 10;
    
    // ProjectileWeapon Stats
    public ProjectileWeaponStats projectileWeaponStats;
    
    // AoEWeapon Stats
    public AoEWeaponStats aoeWeaponStats;
    
    // 이 클래스를 복사하는 생성자
    public WeaponBaseStats(WeaponBaseStats other)
    {
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
}
