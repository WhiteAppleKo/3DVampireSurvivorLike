using System;
using UnityEngine;

public class EnemyController : Controller
{
    public int exp;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Die(int prev, int current)
    {
        base.Die(prev, current);
        BattleManager.Instance.BroadcastExpEvent(exp);
    }
}
