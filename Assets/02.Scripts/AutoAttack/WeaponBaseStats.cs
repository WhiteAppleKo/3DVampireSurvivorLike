using System;

[Serializable]
public class WeaponBaseStats
{
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
        this.projectileWeaponStats = other.projectileWeaponStats;
        this.aoeWeaponStats = other.aoeWeaponStats;
    }

    public WeaponBaseStats()
    {
        
    }
}
