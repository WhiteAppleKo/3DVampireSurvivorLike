using UnityEngine;

namespace Features.Player
{
    [CreateAssetMenu(fileName = "PureDataPlayer", menuName = "PureData/Entity/Player")]
    public class PureDataPlayer : ScriptableObject
    {
        [Header("Base Stats")]
        public int BaseMaxHp = 100;
        public float BaseMoveSpeed = 5.0f;
        public float BaseTurnSpeed = 10.0f;

        [Header("Leveling")]
        public int BaseExpToLevelUp = 100;
        public int ExpIncreasePerLevel = 10;
    }
}
