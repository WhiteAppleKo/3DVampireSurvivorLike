using System;

namespace _02.Scripts.AutoAttack.Charge
{
    [Serializable]
    public class ChargeWeaponStat
    {
        public float findTargetRange = 5.0f;

        public void Set(ChargeWeaponStat stats)
        {
            findTargetRange = stats.findTargetRange;
        }
    }
}
