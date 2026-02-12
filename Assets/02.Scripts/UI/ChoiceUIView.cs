using System.Collections.Generic;
using _02.Scripts.Managers.Choice;
using _02.Scripts.UI;
using Features.Augment;
using Features.Weapon;
using UnityEngine;

namespace _02.Scripts.UI
{
    // [V] UI View Layer for Choice System
    public class ChoiceUIView : MonoBehaviour
    {
        [Header("Logic Reference")]
        [SerializeField] private ChoiceSystem choiceSystem;

        [Header("UI Components")]
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private BindImageText[] bindImageText;

        public void Bind(ChoiceSystem system)
        {
            if (choiceSystem != null)
            {
                choiceSystem.OnAugmentsGenerated -= RefreshUI;
            }

            choiceSystem = system;
            if (choiceSystem != null)
            {
                choiceSystem.OnAugmentsGenerated += RefreshUI;
                Debug.Log("[ChoiceUIView] ChoiceSystem 바인딩 완료");
            }
        }

        private void OnDisable()
        {
            if (choiceSystem != null)
            {
                choiceSystem.OnAugmentsGenerated -= RefreshUI;
            }
        }

        private void RefreshUI(List<PureDataAugment> choices)
        {
            Debug.Log($"[ChoiceUIView] RefreshUI 호출됨. 선택지 수: {choices.Count}");
            // 1. 패널 활성화 및 시간 정지
            TimeScaleManager.Instance.SetTimeScale(0);
            choicePanel.SetActive(true);

            // 2. 데이터 바인딩
            for (int i = 0; i < bindImageText.Length; i++)
            {
                if (i < choices.Count)
                {
                    bindImageText[i].gameObject.SetActive(true);
                    BindData(bindImageText[i], choices[i]);
                }
                else
                {
                    bindImageText[i].gameObject.SetActive(false);
                }
            }
        }

        private void BindData(BindImageText bit, PureDataAugment augment)
        {
            // [V] 신규 통합 바인딩 메서드 사용
            bit.Bind(augment);
        }

        public void CloseUI()
        {
            TimeScaleManager.Instance.SetTimeScale(1);
            choicePanel.SetActive(false);
        }
    }
}
