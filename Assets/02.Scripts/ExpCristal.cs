using System;
using UnityEngine;

public class ExpCristal : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public int expAmount = 10;
    private Controller m_Target;

    public void ExpSetting(Controller controller)
    {
        m_Target = controller;
    }

    public void SetTarget(Controller controller)
    {
        m_Target = controller;
    }

    public void SetValue(int amount)
    {
        expAmount = amount;
    }

    private void Update()
    {
        if (m_Target != null)
        {
            moveSpeed += acceleration * Time.deltaTime;
            
            transform.position = Vector3.MoveTowards(transform.position,  
                m_Target.transform.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_Target.transform.position) < 0.1f)
            {
                BattleManager.Instance.BroadcastExpEvent(expAmount);
                m_Target = null;
                gameObject.SetActive(false);
            }
        }
    }
}
