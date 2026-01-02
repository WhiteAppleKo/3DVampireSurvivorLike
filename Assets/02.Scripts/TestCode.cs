using System;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private InputSystem_Actions m_InputActions;
    public ScriptableObject statAugment;
    public PlayerController playerController;

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
            var stat = statAugment as IStatAugment;
            stat.ModifyStats(playerController.FinalStats);
        }

        if (m_InputActions.Player.P.WasReleasedThisFrame())
        {
            
        }
    }
}
