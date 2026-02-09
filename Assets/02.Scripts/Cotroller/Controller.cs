using _02.Scripts.Cotroller;
using UnityEngine;

namespace _02.Scripts.Cotroller
{
    // [L] Base Logic System for all Entities
    public abstract class Controller : MonoBehaviour
    {
        [Header("Base References")]
        public global::AutoAttack autoAttacker;
        
        public bool isMoveDisable = false;
        public bool isDashing = false;

        protected virtual void Awake()
        {
            // [DLV Refactored] Legacy stats removed. 
            // Child classes should initialize their own Models here.
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public abstract void ApplyDamage(int amount);

        protected abstract void Die(int prev, int current);

        // DLV Interface for shared stats
        public abstract float CurrentMoveSpeed { get; }
    }
}