using System;
using System.Collections.Generic;
using Features.Augment;
using Features.Weapon;
using UnityEngine;

namespace Features.Weapon
{
    /// <summary>
    /// [D] 무기 인벤토리 및 글로벌 증강 상태를 관리하는 런타임 모델
    /// </summary>
    public class RuntimeDataInventory
    {
        public List<RuntimeDataWeapon> WeaponModels { get; private set; } = new List<RuntimeDataWeapon>();
        public List<PureDataWeaponAbility> GlobalAugments { get; private set; } = new List<PureDataWeaponAbility>();
        
        public int MaxSlotCount { get; private set; } = 5;
        
        // 이벤트
        public event Action<RuntimeDataWeapon> OnWeaponAdded;
        public event Action<PureDataWeaponAbility> OnGlobalAugmentAdded;

        public bool CanAddWeapon => WeaponModels.Count < MaxSlotCount;

        public void AddWeaponModel(RuntimeDataWeapon model)
        {
            if (!CanAddWeapon) return;
            
            WeaponModels.Add(model);
            OnWeaponAdded?.Invoke(model);
            
            // 기존 글로벌 증강 소급 적용
            foreach (var augment in GlobalAugments)
            {
                model.ApplyPureAugment(augment);
            }
        }

        public void AddGlobalAugment(PureDataWeaponAbility augment)
        {
            GlobalAugments.Add(augment);
            OnGlobalAugmentAdded?.Invoke(augment);
            
            // 현재 장착된 모든 무기에 적용
            foreach (var model in WeaponModels)
            {
                model.ApplyPureAugment(augment);
            }
        }

        public void Clear()
        {
            WeaponModels.Clear();
            GlobalAugments.Clear();
        }
    }
}
