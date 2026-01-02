using System;
using System.Collections.Generic;
using _02.Scripts.Managers.Choice;
using UnityEngine;

public class ChoiceSystem : MonoBehaviour
{
    public StatAbilityDatabase statAbilityDatabase;
    
    private BaseAbility[] m_Abilities;
    private int m_StatCount;
    private int m_WeponCont;

    private void Awake()
    {
        m_Abilities = new BaseAbility[3];
    }

    public void ChoiceAbilities()
    {
        //0이면 스탯 1이면 무기
        int rnd;
        int choiceIndex;
        for (int i = 0; i < m_Abilities.Length; i++)
        {
            rnd = UnityEngine.Random.Range(0, 2);
            switch (rnd)
            {
                case 0:
                    choiceIndex = UnityEngine.Random.Range(0, statAbilityDatabase.statAbilities.Count);
                    m_Abilities[i] = statAbilityDatabase.statAbilities[choiceIndex];
                    break;
                case 1:
                    choiceIndex = UnityEngine.Random.Range(0, statAbilityDatabase.statAbilities.Count);
                    m_Abilities[i] = statAbilityDatabase.statAbilities[choiceIndex];
                    break;
            }
        }
        PopUpChoiceUI();
    }

    private void PopUpChoiceUI()
    {
        
    }

    public void CloseChoiceUI()
    {
        
    }
}

