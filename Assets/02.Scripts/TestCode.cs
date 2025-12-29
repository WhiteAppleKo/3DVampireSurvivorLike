using System;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public PlayerController test;
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
            BattleManager.Instance.BroadcastExpEvent(10);
        }

        if (m_InputActions.Player.P.WasReleasedThisFrame())
        {
            
        }
    }
}
