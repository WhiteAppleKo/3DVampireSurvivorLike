using System;
using System.Collections.Generic;
using _02.Scripts.Managers.Choice;
using UnityEngine;
using UnityEngine.Serialization;

public class ChoiceSystem : MonoBehaviour
{
    public StatAbilityDatabase statAbilityDatabase;
    public GameObject choicePanel;
    public BindImageText[] bindImageText;
    
    private BaseAbility[] m_Abilities;
    private int m_StatCount;
    private int m_WeponCont;

    private void Awake()
    {
        m_Abilities = new BaseAbility[3];
    }

    public void SetChoices(int index)
    {
        var ch = ChoiceAbilities();
        bindImageText[index].SetImage(ch.icon);
        bindImageText[index].SetText(ch.description);
        bindImageText[index].SetAbility(ch);
    }

    private int m_Rnd;
    private int m_ChoiceIndex;
    private BaseAbility ChoiceAbilities()
    {
        //0이면 스탯 1이면 무기
        m_Rnd = UnityEngine.Random.Range(0, 2);
        switch (m_Rnd)
        {
            case 0:
                m_ChoiceIndex = UnityEngine.Random.Range(0, statAbilityDatabase.statAbilities.Count);
                return statAbilityDatabase.statAbilities[m_ChoiceIndex];
            case 1:
                m_ChoiceIndex = UnityEngine.Random.Range(0, statAbilityDatabase.statAbilities.Count);
                return statAbilityDatabase.statAbilities[m_ChoiceIndex];
        }

        return null;
    }

    public void PopUpChoiceUI()
    {
        TimeScaleManager.Instance.SetTimeScale(0);
        choicePanel.SetActive(true);
    }
}

