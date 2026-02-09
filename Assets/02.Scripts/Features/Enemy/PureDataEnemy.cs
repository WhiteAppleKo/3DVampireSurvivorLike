using UnityEngine;

namespace Features.Enemy
{
    [CreateAssetMenu(fileName = "PureDataEnemy", menuName = "PureData/Entity/Enemy")]
    public class PureDataEnemy : ScriptableObject
    {
        [field: SerializeField] public string ID { get; set; }
        [field: SerializeField] public string MonsterName { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }

        [Header("Base Stats")]
        public int BaseMaxHp = 10;
        public float BaseMoveSpeed = 3.0f;
        public float BaseTurnSpeed = 5.0f;
        public int BaseExpAmount = 5;
        public LayerMask TargetLayer;

        [Header("Movement")]
        public float MinimumDistance = 0.01f;
    }
}