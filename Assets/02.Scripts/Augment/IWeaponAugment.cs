using UnityEngine;

public interface IWeaponAugment
{
    /// <summary>
    /// 무기의 최종 스탯을 변경합니다.
    /// </summary>
    /// <param name="stats">변경될 스탯 객체</param>
    void ModifyStats(WeaponBaseStats stats);
}
