using UnityEngine;
using _02.Scripts.UI;

namespace Features.Augment
{
    /// <summary>
    /// [D] 모든 증강(Augment) 데이터의 베이스 클래스
    /// </summary>
    public abstract class PureDataAugment : ScriptableObject, IBindableUIContent
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Type { get; set; } // 기존 Importer 호환용
        [field: SerializeField] public int IconNumber { get; set; } // 기존 Importer 호환용
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public Sprite Icon { get; set; }
        
        // CSV 임포트 시 사용되는 공통 데이터
        [field: SerializeField] public string ValueType { get; set; } // Fixed / Percentage / Percent
        [field: SerializeField] public float ValueAmount { get; set; }

        /// <summary>
        /// 데이터가 선택되었을 때 실행될 적용 로직입니다.
        /// 자식 클래스(StatAbility, WeaponAbility, Weapon)에서 구현합니다.
        /// </summary>
        public abstract void Apply();
    }
}
