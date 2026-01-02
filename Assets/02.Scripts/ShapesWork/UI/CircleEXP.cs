using System;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CircleEXP : MonoBehaviour
{
    public float detectingRadius = 3f;
    
    private Disc m_ExpDisc;
    [SerializeField] private LayerMask m_TargetLayer;

    private void Awake()
    {
        m_ExpDisc = GetComponent<Disc>();
    }
    private void OnEnable()
    {
        SubscribeManager.Instance.onPlayerExpChangeEvent += ChangeExpValue;
    }
    private void OnDisable()
    {
        if (SubscribeManager.isApplicationQuitting)
        {
            return;
        }
        if (!SubscribeManager.HasInstance)
        {
            return;
        }
        SubscribeManager.Instance.onPlayerExpChangeEvent -= ChangeExpValue;
    }

    private void ChangeExpValue(float ratio)
    {
        m_ExpDisc.AngRadiansEnd = ratio * Mathf.PI * 2f + m_ExpDisc.AngRadiansStart;
    }

    private void OnTriggerEnter(Collider other)
    {
        if((m_TargetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            ExpManager.Instance.SetTarget(other.GetComponent<ExpCristal>());
        }
    }
}
