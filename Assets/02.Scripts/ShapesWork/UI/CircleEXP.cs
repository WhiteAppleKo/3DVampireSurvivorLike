using System;
using Features.Player;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CircleEXP : MonoBehaviour
{
    public float detectingRadius = 3f;
    
    private Disc m_ExpDisc;
    [SerializeField] private LayerMask m_TargetLayer;
    private RuntimeDataPlayer m_Model;

    private void Awake()
    {
        m_ExpDisc = GetComponent<Disc>();
    }

    public void Bind(RuntimeDataPlayer model)
    {
        if (m_Model != null)
        {
            m_Model.OnExpChanged -= ChangeExpValue;
        }

        m_Model = model;
        m_Model.OnExpChanged += ChangeExpValue;
        
        // 초기값 설정
        ChangeExpValue(m_Model.CurrentExp, m_Model.MaxExp);
    }

    private void OnDisable()
    {
        if (m_Model != null)
        {
            m_Model.OnExpChanged -= ChangeExpValue;
        }
    }

    private void ChangeExpValue(int current, int max)
    {
        float ratio = (float)current / max;
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
