using System;
using Features.Player;
using Shapes;
using UnityEngine;

public class PlayerHPInLine : MonoBehaviour
{
    [GradientUsage(true)]
    public Gradient colorGradient;
    
    private Disc m_HpDisc;
    private float m_Hp;
    private RuntimeDataPlayer m_Model;

    private void Awake()
    {
        m_HpDisc = GetComponentInChildren<Disc>();
    }

    public void Bind(RuntimeDataPlayer model)
    {
        if (m_Model != null)
        {
            m_Model.OnHpChanged -= ChangeHpValue;
        }

        m_Model = model;
        m_Model.OnHpChanged += ChangeHpValue;
        
        // 초기값 설정
        ChangeHpValue(m_Model.CurrentHp, m_Model.MaxHp);
    }

    private void OnDisable()
    {
        if (m_Model != null)
        {
            m_Model.OnHpChanged -= ChangeHpValue;
        }
    }

    public void ChangeHpValue(int current, int max)
    {
        float ratio = (float)current / max;
        m_Hp = ratio;
        m_HpDisc.Radius = m_Hp;
        m_HpDisc.AngRadiansEnd = ratio * Mathf.PI * 2f;
        m_HpDisc.Color = colorGradient.Evaluate(m_Hp);
    }
}
