using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    public abstract class BaseAbility : ScriptableObject
    {
        public string abilityID;
        public string abilityName;
        [TextArea] public string description;
        public Sprite icon;
        public int iconNumber;
        public string abilityType;
        public string valueType;
    }
}
