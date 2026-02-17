using UnityEngine;
using _02.Scripts.AutoAttack;

namespace Features.Weapon
{
    [CreateAssetMenu(fileName = "PureDataWeapon", menuName = "PureData/Combat/Weapon")]
    public class PureDataWeapon : Features.Augment.PureDataAugment
    {
        [field: SerializeField] public float AttackDelay { get; set; }
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float EffectRange { get; set; }
        [field: SerializeField] public int ProjectileCount { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public AudioClip AttackSound { get; set; }

        public override void Apply()
        {
            var player = SubscribeManager.Instance.playerController;
            if (player == null || player.autoAttacker == null || Prefab == null) return;

            // 1. 무기 프리팹 생성 (부모는 무기 시스템 오브젝트)
            var weaponInstance = Object.Instantiate(Prefab, player.autoAttacker.transform);
            
            // 2. 무기 위치 및 회전 초기화 (플레이어 위치와 동기화)
            weaponInstance.transform.localPosition = Vector3.zero;
            weaponInstance.transform.localRotation = Quaternion.identity;
            
            // 3. 무기 컴포넌트 가져오기
            var weaponComponent = weaponInstance.GetComponent<_02.Scripts.AutoAttack.Weapon>();
            if (weaponComponent != null)
            {
                // 4. 시스템에 등록
                player.autoAttacker.AddWeapon(weaponComponent);
                Debug.Log($"[WeaponAugment] 신규 무기 장착 및 위치 동기화 완료: {Name}");
            }
            else
            {
                Debug.LogError($"[WeaponAugment] {Prefab.name}에 Weapon 컴포넌트가 없습니다!");
            }
        }
    }
}