using System;
using Shapes;
using UnityEngine;

public class PlayerHPInLine : MonoBehaviour
{
    [GradientUsage(true)]
    public Gradient colorGradient;
    public UIManager uiManager;
    
    private Disc m_HpDisc;
    private float m_Hp;
    private void Start()
    {
        m_HpDisc = GetComponentInChildren<Disc>();
    }

    private void OnEnable()
    {
        uiManager.onPlayerHpChangeEvent += ChangeHpValue;
    }
    
    private void OnDisable()
    {
        uiManager.onPlayerHpChangeEvent -= ChangeHpValue;
    }

    public void ChangeHpValue(float ratio)
    {
        m_Hp = ratio;
        m_HpDisc.Radius = m_Hp;
        m_HpDisc.Color = colorGradient.Evaluate(m_Hp);
    }
}
