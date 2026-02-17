using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    /// <summary>
    /// [D] 증강 능력을 위한 베이스 클래스 (레거시 코드 호환용)
    /// </summary>
    public abstract class BaseAbility : ScriptableObject
    {
        public bool isTemporary;
        public string abilityID;
        public string abilityName;
        [TextArea] public string description;
        public Sprite icon;
        public int iconNumber;
        public string abilityType;
        public string valueType;
    }
}
