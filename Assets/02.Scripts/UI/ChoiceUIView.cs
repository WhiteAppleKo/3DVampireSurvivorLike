using System.Collections.Generic;
using _02.Scripts.Managers.Choice;
using Features.Augment;
using UnityEngine;

namespace _02.Scripts.UI
{
    /// <summary>
    /// [V] 인게임 증강 선택창 관리자입니다.
    /// ChoiceSystem의 명령을 받아 화면을 구성하고 활성화합니다.
    /// </summary>
    public class ChoiceUIView : BaseHybridScrollUIView
    {
        [Header("Choice Specific UI")]
        [SerializeField] private DescriptionUIView descriptionView;

        protected override void Start()
        {
            base.Start();

            // 포커스 변경 이벤트 구독
            OnFocusChanged += UpdateDescription;
            if(gameObject.activeSelf) gameObject.SetActive(false);
        }

        private void UpdateDescription(int index)
        {
            if (descriptionView == null) return;

            if (index >= 0 && index < buttons.Count)
            {
                var selectedButton = buttons[index];
                if (selectedButton != null && selectedButton.gameObject.activeSelf)
                {
                    descriptionView.Bind(selectedButton.CurrentContent);
                }
            }
        }

        /// <summary>
        /// ChoiceSystem에서 호출하는 외부 주입 메서드
        /// </summary>
        public void ShowChoices(List<PureDataAugment> choices)
        {
            Debug.Log($"[ChoiceUIView] ShowChoices 수신. 선택지 수: {choices.Count}");

            // 1. 데이터 바인딩
            for (int i = 0; i < buttons.Count; i++)
            {
                if (i < choices.Count)
                {
                    buttons[i].gameObject.SetActive(true);
                    buttons[i].Bind(choices[i]);
                    buttons[i].SetInteractable(true);
                }
                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }

            // [추가] 런타임에 버튼 이벤트 재연결
            InitializeButtons();

            // 2. 초기 스크롤 위치 설정 (중앙)
            m_TargetScrollPos = (choices.Count - 1) * 0.5f;
            m_CurrentScrollPos = m_TargetScrollPos;

            // 초기 설명 갱신
            UpdateDescription(Mathf.RoundToInt(m_CurrentScrollPos));

            // 3. 시간 정지 및 UI 활성화
            TimeScaleManager.Instance.SetTimeScale(0);
            OpenUI();
        }

        public override void OpenUI()
        {
            base.OpenUI();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void CloseUI()
        {
            base.CloseUI();
            TimeScaleManager.Instance.SetTimeScale(1);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void ConfirmSelection()
        {
            int finalIndex = Mathf.RoundToInt(m_TargetScrollPos);
            if (finalIndex >= 0 && finalIndex < buttons.Count)
            {
                var selectedButton = buttons[finalIndex];
                if (selectedButton != null && selectedButton.gameObject.activeSelf)
                {
                    // 데이터 주도 Apply 실행 (증강 적용)
                    base.ConfirmSelection();
                    
                    // 선택 완료 후 UI 닫기
                    CloseUI();
                }
            }
        }
    }
}
