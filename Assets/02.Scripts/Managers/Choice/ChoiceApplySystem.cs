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
    [Header("References")]
    public PlayerController player;
    public global::AutoAttack playerAutoAttack;
    
    [Header("UI References")]
    [SerializeField] private ChoiceUIView choiceUIView;
    private ScrollItemScaler m_ScrollItemScaler;

    private void Start()
    {
        m_ScrollItemScaler = GetComponent<ScrollItemScaler>();
    }

    public void ApplySelectedAugment()
    {
        if (m_ScrollItemScaler == null || m_ScrollItemScaler.SelectedItem == null) return;

        var bit = m_ScrollItemScaler.SelectedItem.GetComponent<BindImageText>();
        if (bit == null) return;

        // [Logic] 제네릭 기반 데이터 추출 및 적용
        var pureWeapon = bit.GetData<PureDataWeapon>();
        if (pureWeapon != null)
        {
            AddNewWeapon(pureWeapon);
        }
        else
        {
            var pureStat = bit.GetData<PureDataStatAbility>();
            if (pureStat != null)
            {
                ApplyStatAugment(pureStat);
            }
            else
            {
                var pureWeaponAbility = bit.GetData<PureDataWeaponAbility>();
                if (pureWeaponAbility != null)
                {
                    ApplyWeaponAugment(pureWeaponAbility);
                }
            }
        }

        // UI 닫기
        if (choiceUIView != null) choiceUIView.CloseUI();
    }

    private void AddNewWeapon(PureDataWeapon pureWeapon)
    {
        if (pureWeapon.Prefab != null)
        {
            var weaponInstance = Instantiate(pureWeapon.Prefab, playerAutoAttack.transform);
            var weaponComponent = weaponInstance.GetComponent<Weapon>();
            playerAutoAttack.AddWeapon(weaponComponent);
        }
    }

    private void ApplyStatAugment(PureDataStatAbility pureStat)
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
        }
    }

    private void ApplyWeaponAugment(PureDataWeaponAbility pureWeaponAbility)
    {
        playerAutoAttack.AddPureAugment(pureWeaponAbility);
    }
}
