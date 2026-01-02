using UnityEngine;

[CreateAssetMenu(fileName = "New DamageUpAugment", menuName = "Augments/Damage Up Augment")]
public class DamageUpAugment : ScriptableObject, IWeaponAugment
{
    [Tooltip("증가시킬 데미지 양")]
    public int damageIncrease = 20;

    public void ModifyStats(WeaponBaseStats stats)
    {
        // 무기의 최종 데미지를 증가시킵니다.
        stats.damage += damageIncrease;
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
