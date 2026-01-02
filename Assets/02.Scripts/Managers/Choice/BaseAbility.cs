using UnityEngine;

namespace _02.Scripts.Managers.Choice
{
    public abstract class BaseAbility : ScriptableObject
    {
        public string abilityID;
        public string abilityName;
        [TextArea] public string description;
        public Sprite icon;
        

        public abstract void Apply(Controller player);
    }
}
