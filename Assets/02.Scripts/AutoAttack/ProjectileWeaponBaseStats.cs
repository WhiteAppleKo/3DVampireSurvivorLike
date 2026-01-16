using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[Serializable]
public class ProjectileWeaponStats
{
    public int projectileCount = 1;
    public float findTargetRange = 5.0f;

    public void Set(ProjectileWeaponStats stats)
    {
        projectileCount = stats.projectileCount;
        findTargetRange = stats.findTargetRange;
    }
}
