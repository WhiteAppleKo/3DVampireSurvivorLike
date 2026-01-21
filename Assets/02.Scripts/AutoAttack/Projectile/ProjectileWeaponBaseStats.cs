using System;

namespace _02.Scripts.AutoAttack.Projectile
{
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
}
