using UnityEngine;

public interface IAugment
{
    /// <summary>
    /// 무기의 최종 스탯을 변경합니다.
    /// </summary>
    /// <param name="stats">변경될 스탯 객체</param>
    void ModifyStats(WeaponBaseStats stats);

    /// <summary>
    /// 무기가 공격할 때마다 호출될 효과입니다.
    /// </summary>
    /// <param name="weapon">공격을 실행한 무기</param>
    void OnAttack(Weapon weapon);

    void EnemyDead(Weapon weapon);
}
