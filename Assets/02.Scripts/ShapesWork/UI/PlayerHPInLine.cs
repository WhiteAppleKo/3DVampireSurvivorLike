using System;
using Shapes;
using UnityEngine;

public class PlayerHPInLine : MonoBehaviour
{
    [GradientUsage(true)]
    public Gradient colorGradient;
    
    private Disc m_HpDisc;
    private float m_Hp;
    private void Awake()
    {
        m_HpDisc = GetComponentInChildren<Disc>();
    }

    private void OnEnable()
    {
        SubscribeManager.Instance.onPlayerHpChangeEvent += ChangeHpValue;
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
        SubscribeManager.Instance.onPlayerHpChangeEvent -= ChangeHpValue;
    }

    public void ChangeHpValue(float ratio)
    {
        m_Hp = ratio;
        m_HpDisc.Radius = m_Hp;
        m_HpDisc.AngRadiansEnd = ratio * Mathf.PI * 2f;
        m_HpDisc.Color = colorGradient.Evaluate(m_Hp);
    }
}
