using System;
using UnityEngine;

[Serializable]
public class AoEWeaponStats
{
    public float areaOfEffectRadius = 5.0f;

    public void Set(AoEWeaponStats other)
    {
        areaOfEffectRadius = other.areaOfEffectRadius;
    }
}
