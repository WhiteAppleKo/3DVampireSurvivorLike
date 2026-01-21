using System;
using UnityEngine;

namespace _02.Scripts.Managers
{
    public class AudioManager : SingletoneBase<AudioManager>
    {
        private void OnEnable()
        {
            if (BattleManager.Instance != null)
            {
                Debug.Log("PlayAudio 구독");
                BattleManager.Instance.onDamageEvent += PlayAudio;
            }
        }

        private void OnDisable()
        {
            if (BattleManager.Instance != null)
            {
                Debug.Log("PlayAudio 구독 해제");
                BattleManager.Instance.onDamageEvent -= PlayAudio;
            }
        }

        private void PlayAudio(BattleManager.DamageEventStruct damageEvent)
        {
            Debug.Log("PlayAudio");
            if (damageEvent.senderWeapon != null && damageEvent.senderWeapon.audioSource != null)
            {
                damageEvent.senderWeapon.audioSource.Play();
            }
        }
    }
}
