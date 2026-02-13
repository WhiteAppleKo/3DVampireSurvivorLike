using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [V] Title 씬의 전체 버튼들을 관리하고 로직과 연결합니다.
    /// 인게임 Choice UX를 도입하여 부드러운 실시간 스크롤 및 자석식 스냅 기능을 제공합니다.
    /// </summary>
    public class TitleUIView : MonoBehaviour
    {
        [Header("Logic Reference")]
        [SerializeField] private TitleLogicSystem logicSystem;

        [Header("Title Buttons")]
        [SerializeField] private List<TitleButtonUIView> titleButtons;

        [Header("Interaction Settings")]
        [SerializeField] private Button confirmButton; 
        [SerializeField] private float scrollSensitivity = 0.005f; 
        [SerializeField] private float lerpSpeed = 10f; 

        [Header("Option Settings (Test)")]
        public ChoiceUXType choiceUXType = ChoiceUXType.Continuous;

        private float m_TargetScrollPos = 0f; 
        private float m_CurrentScrollPos = 0f; 
        
        private Vector2 m_LastMousePos;
        private bool m_IsDragging = false;

        private void Start()
        {
            if (logicSystem == null) return;

            for (int i = 0; i < titleButtons.Count; i++)
            {
                var buttonView = titleButtons[i];
                if (buttonView == null) continue;

                int index = i;
                buttonView.OnClicked += (id) => 
                {
                    m_TargetScrollPos = index;
                };

                if (buttonView.ButtonID == "Load")
                {
                    bool canLoad = DataHub.Instance.HasSaveFile;
                    buttonView.SetInteractable(canLoad);
                }
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(ConfirmSelection);
            }
        }

        private void Update()
        {
            HandleInput();
            
            if (!m_IsDragging)
            {
                m_CurrentScrollPos = Mathf.Lerp(m_CurrentScrollPos, m_TargetScrollPos, Time.unscaledDeltaTime * lerpSpeed);
            }

            UpdateVisuals();
        }

        private void HandleInput()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                m_TargetScrollPos = Mathf.Clamp(m_TargetScrollPos - (scrollInput * 10f), 0, titleButtons.Count - 1);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                m_TargetScrollPos = Mathf.Max(0, m_TargetScrollPos - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                m_TargetScrollPos = Mathf.Min(titleButtons.Count - 1, m_TargetScrollPos + 1);
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_LastMousePos = Input.mousePosition;
                m_IsDragging = true;
            }
            else if (Input.GetMouseButton(0) && m_IsDragging)
            {
                Vector2 currentMousePos = Input.mousePosition;
                float deltaY = currentMousePos.y - m_LastMousePos.y;
                
                m_CurrentScrollPos = Mathf.Clamp(m_CurrentScrollPos + (deltaY * scrollSensitivity), 0, titleButtons.Count - 1);
                m_TargetScrollPos = m_CurrentScrollPos; 
                
                m_LastMousePos = currentMousePos;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_IsDragging = false;
                m_TargetScrollPos = Mathf.RoundToInt(m_CurrentScrollPos);
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmSelection();
            }
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < titleButtons.Count; i++)
            {
                if (titleButtons[i] == null) continue;

                float distance = Mathf.Abs(i - m_CurrentScrollPos);
                bool isFocused = distance < 0.5f;
                
                // 상세 UX 상태 주입 (거리 기반 확대 지원)
                titleButtons[i].SetUXState(isFocused, distance, choiceUXType);
            }
        }

        private void ConfirmSelection()
        {
            int finalIndex = Mathf.RoundToInt(m_TargetScrollPos);
            if (finalIndex >= 0 && finalIndex < titleButtons.Count)
            {
                var selectedButton = titleButtons[finalIndex];
                if (selectedButton != null)
                {
                    if (selectedButton.ButtonID == "Load" && !DataHub.Instance.HasSaveFile) return;
                    logicSystem.HandleButtonAction(selectedButton.ButtonID);
                }
            }
        }
    }
}
