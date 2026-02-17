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

    /// <summary>
    /// 외부(ChoiceUIView)에서 선택된 데이터를 직접 전달받아 게임에 적용합니다.
    /// </summary>
    public void ApplyAugmentDirectly(PureDataAugment augment)
    {
        if (augment == null) return;

        // [Logic] 데이터 타입에 따른 분기 처리
        if (augment is PureDataWeapon pureWeapon)
        {
            AddNewWeapon(pureWeapon);
        }
        else if (augment is PureDataStatAbility pureStat)
        {
            ApplyStatAugment(pureStat);
        }
        else if (augment is PureDataWeaponAbility pureWeaponAbility)
        {
            ApplyWeaponAugment(pureWeaponAbility);
        }

        Debug.Log($"[ChoiceApply] 증강 적용 완료: {augment.ID}");
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
