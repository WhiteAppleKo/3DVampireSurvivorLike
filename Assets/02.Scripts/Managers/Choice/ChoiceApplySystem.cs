using System;
using _02.Scripts.Augment.BaseAugment;
using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Choice;
using _02.Scripts.UI;
using Features.Augment;
using Features.Weapon;
using UnityEngine;

public class ChoiceApplySystem : MonoBehaviour
{
    public GameObject choicePanel;
    public PlayerController player;
    public global::AutoAttack playerAutoAttack;
    private ScrollItemScaler m_ScrollItemScaler;
    private void Start()
    {
        m_ScrollItemScaler = GetComponent<ScrollItemScaler>();
    }

    public void ApplyAbility()
    {
        if (player == null || m_ScrollItemScaler.SelectedItem == null) return;

        var bit = m_ScrollItemScaler.SelectedItem.GetComponent<BindImageText>();
        bool applied = false;

        // 1. DLV: 새로운 무기 데이터 처리 (신규 무기 추가)
        if (bit.GetPureWeapon(out PureDataWeapon pureWeapon))
        {
            if (pureWeapon.Prefab != null)
            {
                var weaponInstance = Instantiate(pureWeapon.Prefab, playerAutoAttack.transform);
                var weaponComponent = weaponInstance.GetComponent<Weapon>();
                playerAutoAttack.AddWeapon(weaponComponent);
                applied = true;
            }
        }
        // 2. DLV: 새로운 능력치 증강 처리 (체력, 이동속도 등)
        else if (bit.GetPureStatAbility(out PureDataStatAbility pureStat))
        {
            switch (pureStat.TargetStatType)
            {
                case StatAbility.e_StatType.Health:
                    player.Model.Heal((int)pureStat.ValueAmount);
                    break;
                case StatAbility.e_StatType.MaxHp:
                    player.Model.AddMaxHpModifier((int)pureStat.ValueAmount);
                    break;
                case StatAbility.e_StatType.MoveSpeed:
                    player.Model.AddMoveSpeedModifier(pureStat.ValueAmount);
                    break;
                default:
                    Debug.LogWarning($"[ChoiceSystem] 미구현 Stat 타입: {pureStat.TargetStatType}");
                    break;
            }
            applied = true; 
        }
        // 3. DLV: 새로운 무기 성능 강화 처리 (공격 속도, 데미지 등)
        else if (bit.GetPureWeaponAbility(out PureDataWeaponAbility pureWeaponAbility))
        {
            playerAutoAttack.AddPureAugment(pureWeaponAbility);
            applied = true;
        }

        if (applied)
        {
            CloseChoiceUI();
        }
    }

    private void CloseChoiceUI()
    {
        TimeScaleManager.Instance.SetTimeScale(1);
        choicePanel.SetActive(false);
    }
}
