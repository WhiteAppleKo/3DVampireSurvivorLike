using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.UI
{
    /// <summary>
    /// [V] 하이브리드 스크롤 UX를 사용하는 모든 UI의 기반 클래스입니다.
    /// </summary>
    public abstract class BaseHybridScrollUIView : MonoBehaviour
    {
        [Header("Common UI Components")]
        [SerializeField] protected GameObject mainPanel;
        [SerializeField] protected List<ButtonUIView> buttons;
        [SerializeField] protected Button confirmButton;

        [Header("Common Scroll Settings")]
        [SerializeField] protected float scrollSensitivity = 0.01f;
        [SerializeField] protected float lerpSpeed = 12f;

        protected float m_TargetScrollPos = 0f;
        protected float m_CurrentScrollPos = 0f;
        protected int m_LastFocusedIndex = -1; 
        protected Vector2 m_LastMousePos;
        protected bool m_IsDragging = false;
        protected bool m_IsActive = true; 

        public System.Action<int> OnFocusChanged;

        protected virtual void Start()
        {
            InitializeButtons();

            m_LastFocusedIndex = Mathf.RoundToInt(m_CurrentScrollPos);

            // [수정] 여기서 직접 리스너를 달면 ButtonUIView의 applyOnClicked 설정을 무시하고 무조건 실행됩니다.
            // 따라서 이 중복 리스너를 제거합니다. 버튼 클릭 로직은 각 ButtonUIView에서 관리합니다.
            /*
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(ConfirmSelection);
            }
            */

            if (mainPanel != null)
            {
                m_IsActive = mainPanel.activeSelf;
            }
        }

        protected void InitializeButtons()
        {
            if (buttons == null) return;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == null) continue;

                int index = i;
                buttons[index].OnClicked = null; 
                buttons[index].OnClicked = () => HandleButtonClick(index);
            }
        }

        private void HandleButtonClick(int index)
        {
            Debug.Log($"[BaseUI] 버튼 {index} 클릭 수신. 이동 목표: {index}");
            m_IsDragging = false; 
            SetTargetScrollPos(index);
        }

        protected virtual void Update()
        {
            if (m_IsActive)
            {
                HandleInput();
            }

            // Lerp 이동 (드래그 중이 아닐 때만 목표 지점으로 부드럽게 이동)
            if (!m_IsDragging)
            {
                m_CurrentScrollPos = Mathf.Lerp(m_CurrentScrollPos, m_TargetScrollPos, Time.unscaledDeltaTime * lerpSpeed);
            }

            UpdateVisuals();
        }

        protected virtual void HandleInput()
        {
            // 1. 마우스 휠
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                m_TargetScrollPos = Mathf.Clamp(m_TargetScrollPos - (scrollInput * 5f), 0, buttons.Count - 1);
            }

            // 2. 키보드 입력
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                m_TargetScrollPos = Mathf.Max(0, m_TargetScrollPos - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                m_TargetScrollPos = Mathf.Min(buttons.Count - 1, m_TargetScrollPos + 1);
            }

            // 3. 마우스 드래그 로직 개선
            if (Input.GetMouseButtonDown(0))
            {
                m_LastMousePos = Input.mousePosition;
                // 바로 드래그를 켜지 않고, 이동이 감지될 때 켜는 것이 안전하지만
                // 여기서는 기존 로직을 유지하되 종료 시점을 정교화합니다.
                m_IsDragging = true;
            }
            else if (Input.GetMouseButton(0) && m_IsDragging)
            {
                Vector2 currentMousePos = Input.mousePosition;
                float deltaY = currentMousePos.y - m_LastMousePos.y; 
                
                if (Mathf.Abs(deltaY) > 0.1f) // 미세한 움직임 체크
                {
                    m_CurrentScrollPos = Mathf.Clamp(m_CurrentScrollPos + (deltaY * scrollSensitivity), 0, buttons.Count - 1);
                    m_TargetScrollPos = m_CurrentScrollPos;
                }
                
                m_LastMousePos = currentMousePos;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // [수정] 실제로 드래그 이동이 발생했을 때만 스냅합니다.
                // 단순 클릭 시에는 HandleButtonClick에서 설정한 TargetScrollPos를 유지해야 합니다.
                if (m_IsDragging)
                {
                    m_IsDragging = false;
                    // 드래그 거리가 거의 없다면 스냅하지 않음 (클릭 보호)
                    if (Mathf.Abs(m_CurrentScrollPos - m_TargetScrollPos) > 0.05f)
                    {
                        m_TargetScrollPos = Mathf.RoundToInt(m_CurrentScrollPos);
                    }
                }
            }

            // 4. 선택 확인
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmSelection();
            }
        }

        protected virtual void UpdateVisuals()
        {
            if (buttons == null) return;
            
            bool isMoving = m_IsDragging || Mathf.Abs(m_CurrentScrollPos - m_TargetScrollPos) > 0.01f;
            int currentFocusedIndex = Mathf.RoundToInt(m_CurrentScrollPos);

            // [추가] 포커스된 인덱스가 변경되면 이벤트 알림
            if (currentFocusedIndex != m_LastFocusedIndex)
            {
                m_LastFocusedIndex = currentFocusedIndex;
                if (currentFocusedIndex >= 0 && currentFocusedIndex < buttons.Count)
                {
                    OnFocusChanged?.Invoke(currentFocusedIndex);
                }
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == null) continue;
                float distance = Mathf.Abs(i - m_CurrentScrollPos);
                bool isFocused = distance < 0.5f;
                buttons[i].SetUXState(isFocused, distance, isMoving);
            }
        }

        public void SetTargetScrollPos(int index)
        {
            m_TargetScrollPos = index;
        }

        public virtual void ConfirmSelection()
        {
            int finalIndex = Mathf.RoundToInt(m_TargetScrollPos);
            if (finalIndex >= 0 && finalIndex < buttons.Count)
            {
                var selectedButton = buttons[finalIndex];
                if (selectedButton != null && selectedButton.CurrentContent != null)
                {
                    selectedButton.CurrentContent.Apply();
                }
            }
        }
        
        public virtual void OpenUI()
        {
            m_IsActive = true;
            gameObject.SetActive(true);
            if (mainPanel != null) mainPanel.SetActive(true);
            UpdateVisuals();
        }

        public virtual void CloseUI()
        {
            m_IsActive = false;
            if (mainPanel != null) mainPanel.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
