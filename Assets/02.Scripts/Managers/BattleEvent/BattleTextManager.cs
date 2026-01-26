using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _02.Scripts.Managers.BattleEvent
{
    public class BattleTextManager : MonoBehaviour
    {
        [Header("Setting")] 
        [SerializeField] private DamageText m_DamageTextPrefab;
        [SerializeField] private int m_Poolsize = 50;
        [SerializeField] private float m_TextDuration = 1f;
        [SerializeField] private float m_TextMoveSpeed = 2f;
        [SerializeField] private Vector3 m_Offset = new Vector3(0, 2f, 0);
        
        private List<DamageText> m_DamageTexts;

        private void Start()
        {
            InitializePool();
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent += ShowDamageText;
            }
        }

        private void OnDestroy()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.onDamageEvent -= ShowDamageText;
            }
        }

        private void InitializePool()
        {
            m_DamageTexts = new  List<DamageText>();

            for (int i = 0; i < m_Poolsize; i++)
            {
                CreateNewDamageText();
            }
        }

        private void ShowDamageText(BattleManager.DamageEventStruct damageEvent)
        {
            if (damageEvent.receiver != null)
            {
                SpawnDamageText(damageEvent.damageAmount, damageEvent.receiver.transform.position);
            }
        }

        private DamageText CreateNewDamageText()
        {
            DamageText instance = Instantiate(m_DamageTextPrefab, transform);
            instance.gameObject.SetActive(false);
            m_DamageTexts.Add(instance);
            return instance;
        }

        private DamageText GetFromPool()
        {
            for (int i = 0; i < m_DamageTexts.Count; i++)
            {
                if (m_DamageTexts[i].gameObject.activeInHierarchy == false)
                {
                    return m_DamageTexts[i];
                }
            }
            
            return CreateNewDamageText();
        }

        private void SpawnDamageText(int amount, Vector3 position)
        {
            DamageText textObj = GetFromPool();
            textObj.SetUp(amount, position + m_Offset, m_TextDuration,  m_TextMoveSpeed);
        }
    }
}
