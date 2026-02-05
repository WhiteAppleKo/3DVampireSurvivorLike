using UnityEngine;

namespace _02.Scripts.AutoAttack
{
    public class WeaponData : BaseWeaponData
    {
        // 임포터에서 호출하는 데이터 설정 메서드
        public void SetSo(string id, string name, string type, string delay, string damage, string findRange, string projectileCount,
            GameObject prefab, int iconSpriteNumber, string description)
        {
            weaponID = id;
            weaponName = name;
            weaponType = type;
            attackDelay = float.Parse(delay);
            weaponDamage = int.Parse(damage);
            effectRange = float.Parse(findRange);
            this.projectileCount = int.Parse(projectileCount);
            weaponPrefab = prefab;
            iconNumber = iconSpriteNumber;
            weaponDescription = description;

            if (weaponPrefab == null)
            {
                Debug.LogWarning($"[WeaponData] {id}번 무기 프리팹이 비어있습니다.");
            }
            
            // 기존의 GetComponent<Weapon>().baseStats.WeaponDataLoadLogic(this) 부분은 
            // 데이터가 로직을 직접 수정하는 행위이므로 DLV 원칙에 따라 제거합니다.
            // 대신 무기 생성(System Awake) 시점에서 이 데이터를 읽어가도록 리팩토링되었습니다.
        }
    }
}