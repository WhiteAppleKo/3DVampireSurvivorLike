using System;
using System.Collections.Generic;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Choice;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    private InputSystem_Actions m_InputActions;
    public GameObject player;
    public HexGridRenderer hexGridRenderer;
    public LayerMask layerMask;
    public ExpManager lvup;

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
            lvup.PlayerLevelUp();
            //hexGridRenderer.StartCharge(player.transform.position, 2, 2, layerMask, OnMeteorExplosion);
            //SaveManager.Instance.SaveGame();
        }

        if (m_InputActions.Player.P.WasReleasedThisFrame())
        {
            SaveManager.Instance.LoadGame();
        }
    }

    void OnMeteorExplosion(List<Collider> colliders)
    {
        Debug.Log("explode");
    }
}
