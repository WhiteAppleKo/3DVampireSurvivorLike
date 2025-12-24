using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectileCountUpAugment", menuName = "Augments/Projectile Count Up Augment")]
public class ProjectileCountUpAugment : ScriptableObject, IAugment
{
    [Tooltip("증가시킬 투사체 개수")]
    public int projectileIncrease = 1;

    public void ModifyStats(WeaponBaseStats stats)
    {
        // 무기의 최종 투사체 개수를 증가시킵니다.
        stats.projectileWeaponStats.projectileCount += projectileIncrease;
    }

    public void OnAttack(Weapon weapon)
    {
        // 이 증강은 공격 시 특별한 효과가 없습니다.
    }

    public void EnemyDead(Weapon weapon)
    {
        // 이 증강은 적이 죽었을 때 특별한 효과가 없습니다.
    }
}
