using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public ClampInt enemyHp;

    private void Awake()
    {
        enemyHp = new ClampInt(0, 100, 100);
    }
}
