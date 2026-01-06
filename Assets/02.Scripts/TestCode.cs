using System;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Choice;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public Controller player;
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
            ExpManager.Instance.PlayerLevelUp();
        }

        if (m_InputActions.Player.P.WasReleasedThisFrame())
        {
            
        }
    }
}
