using System;
using UnityEngine;

public class EnemyController : Controller
{
    public int exp;

    [SerializeField]
    private Controller m_Target;
    protected override void Awake()
    {
        base.Awake();
        autoAttacker.GameStart();
    }

    public void SetTarget(Controller target)
    {
        m_Target = target;
    }

    protected override void Die(int prev, int current)
    {
        base.Die(prev, current);
        ExpManager.Instance.SetExp(exp, transform.position);
    }
}
