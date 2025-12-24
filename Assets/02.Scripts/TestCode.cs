using System;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public AutoAttack test;
    
    public ScriptableObject augment;
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
            test.AddAugment(augment as IAugment);
        }

        if (m_InputActions.Player.P.WasReleasedThisFrame())
        {
            test.RemoveAugment(augment as IAugment);
        }
    }
}
