using System;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.Managers.Choice;
using UnityEngine;

public class ChoiceApplySystem : MonoBehaviour
{
    public GameObject choicePanel;
    public PlayerController player;
    public AutoAttack playerAutoAttack;
    private ScrollItemScaler m_ScrollItemScaler;
    private void Start()
    {
        m_ScrollItemScaler = GetComponent<ScrollItemScaler>();
    }

    public void ApplyAbility()
    {
        if (player == null)
        {
            Debug.LogError("Player 참조가 없습니다!");
            return;
        }
        Debug.Log($"[ChoiceSystem] Player ID: {player.GetInstanceID()} 에게 능력 적용 시도");

        var ab = m_ScrollItemScaler.SelectedItem.GetComponent<BindImageText>().GetAbility();
        CloseChoiceUI();
        string abilityType = ab.abilityType;
        switch (abilityType)
        {
            case "Stat":
                Debug.Log($"[ChoiceSystem] AddAugment 호출 직전. 능력: {ab.abilityName}");
                if (ab.isTemporary == true)
                {
                    (ab as StatAbility).Apply(player.FinalStats);
                    break;
                }
                player.AddAugment((StatAbility)ab);
                Debug.Log($"[ChoiceSystem] AddAugment 호출 완료. 타입: {abilityType}");
                break;
            case "Weapon":
                Debug.Log("Weapon");
                playerAutoAttack.AddAugment((WeaponAbility)ab);
                break;
            default:
                Debug.Log("Default");
                break;
        }
    }

    private void CloseChoiceUI()
    {
        TimeScaleManager.Instance.SetTimeScale(1);
        choicePanel.SetActive(false);
    }
}
