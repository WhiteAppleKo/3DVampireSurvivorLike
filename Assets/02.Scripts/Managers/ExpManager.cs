using System;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : SingletoneBase<ExpManager>
{
    public Controller playerController;
    public GameObject expPrefab;
    
    private List<GameObject> m_PooledExplosions;
    private ChoiceSystem m_ChoiceManager;
    private void Start()
    {
        m_PooledExplosions = new List<GameObject>();
        for (int i = 0; i < 50; i++)
        {
            GameObject obj = Instantiate(expPrefab);
            m_PooledExplosions.Add(obj);
            obj.SetActive(false);
        }
        m_ChoiceManager = GetComponent<ChoiceSystem>();
    }
    
    private GameObject GetObject()
    {
        // 리스트를 순회하며 비활성화된 경험치를 찾는다
        foreach (var exp in m_PooledExplosions)
        {
            if (!exp.activeInHierarchy)
            {
                return exp;
            }
        }
        
        GameObject newObj = Instantiate(expPrefab);
        m_PooledExplosions.Add(newObj);
        return newObj;
    }

    public void SetExp(int amount, Vector3 pos)
    {
        var obj = GetObject();
        obj.SetActive(true);
        var exp = obj.GetComponent<ExpCristal>();
        exp.SetValue(amount);
        obj.transform.position = pos;
    }

    public void SetTarget(ExpCristal exp)
    {
        exp.SetTarget(playerController);
    }

    public void PlayerLevelUp()
    {
        playerController.FinalStats.playerStats.LevelUp();
        playerController.FinalStats.playerStats.exp.ResetToMin();
        m_ChoiceManager.PopUpChoiceUI();
        for (int i = 0; i < 3; i++)
        {
            m_ChoiceManager.SetChoices(i);
        }
    }
}
