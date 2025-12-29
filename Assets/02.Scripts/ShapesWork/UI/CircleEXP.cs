using System;
using Shapes;
using Unity.VisualScripting;
using UnityEngine;

public class CircleEXP : MonoBehaviour
{
    private Disc m_ExpDisc;

    private void Awake()
    {
        m_ExpDisc = GetComponent<Disc>();
    }
    private void OnEnable()
    {
        UIManager.Instance.onPlayerExpChangeEvent += ChangeExpValue;
    }
    private void OnDisable()
    {
        if (UIManager.isApplicationQuitting)
        {
            return;
        }
        if (!UIManager.HasInstance)
        {
            return;
        }
        UIManager.Instance.onPlayerExpChangeEvent -= ChangeExpValue;
    }

    private void ChangeExpValue(float ratio)
    {
        m_ExpDisc.AngRadiansEnd = ratio * Mathf.PI * 2f + m_ExpDisc.AngRadiansStart;
    }
}
