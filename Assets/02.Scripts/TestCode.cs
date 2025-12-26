using System;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public PlayerController test;
    private Weapon nulls;
    private InputSystem_Actions m_InputActions;

    private void Awake()
    {
        m_InputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        m_InputActions.Player.Enable();
    }

    private void OnDisable()
    {
        m_InputActions.Player.Disable();
    }
    
    void Update()
    {
        if (m_InputActions.Player.O.WasReleasedThisFrame())
        {
            var m_DamageEvent = new BattleManager.DamageEventStruct(10, nulls, test, test);
            BattleManager.Instance.BroadcastDamageEvent(m_DamageEvent);
        }

        if (m_InputActions.Player.P.WasReleasedThisFrame())
        {
            
        }
    }
}
