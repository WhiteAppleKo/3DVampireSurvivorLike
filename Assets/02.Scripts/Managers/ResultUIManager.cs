using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Features.Player;
using _02.Scripts.UI;

namespace _02.Scripts.Managers
{
    /// <summary>
    /// 게임 오버 시 결과 창을 관리하는 매니저입니다.
    /// 플레이어 사망 시 활성화되며 무기별 통계를 표시합니다.
    /// </summary>
    public class ResultUIManager : SingletoneBase<ResultUIManager>
    {
        [Header("UI References")]
        [SerializeField] private GameObject resultCanvas;
        [SerializeField] private TextMeshProUGUI totalPointsText;
        [SerializeField] private Transform weaponStatContainer;
        [SerializeField] private GameObject weaponStatPrefab;

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        protected override void Awake()
        {
            base.Awake();
            if (resultCanvas != null) resultCanvas.SetActive(false);

            if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        /// <summary>
        /// SubscribeManager에서 호출하여 플레이어 모델과 연결합니다.
        /// </summary>
        public void Bind(RuntimeDataPlayer model)
        {
            if (model == null) return;
            model.OnDead += ShowResult;
        }

        public void ShowResult()
        {
            if (resultCanvas == null)
            {
                Debug.LogError("[ResultUIManager] Result Canvas가 할당되지 않았습니다.");
                return;
            }

            // 1. 게임 일시정지
            TimeScaleManager.Instance.SetTimeScale(0);
            
            // 2. 데이터 채우기
            PopulateStats();

            // 3. UI 활성화
            resultCanvas.SetActive(true);
            
            // 4. 마우스 커서 활성화
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void PopulateStats()
        {
            // 포인트 표시 (기본값 0)
            if (totalPointsText != null)
            {
                totalPointsText.text = $"Total Points: {BattleStatisticsManager.Instance.TotalPoints}";
            }

            // 기존 리스트 아이템 제거
            if (weaponStatContainer != null)
            {
                foreach (Transform child in weaponStatContainer)
                {
                    Destroy(child.gameObject);
                }

                // 무기별 통계 아이템 생성
                foreach (var statPair in BattleStatisticsManager.Instance.WeaponStats)
                {
                    var stat = statPair.Value;
                    if (weaponStatPrefab != null)
                    {
                        var itemObj = Instantiate(weaponStatPrefab, weaponStatContainer);
                        var itemUI = itemObj.GetComponent<WeaponStatItemUI>();
                        if (itemUI != null)
                        {
                            itemUI.SetData(stat.WeaponData, stat.KillCount, stat.TotalDamage);
                        }
                    }
                }
            }
        }

        public void RestartGame()
        {
            TimeScaleManager.Instance.SetTimeScale(1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            TimeScaleManager.Instance.SetTimeScale(1);
            // Title 씬 로드 (인덱스 0 또는 이름으로 로드)
            SceneManager.LoadScene("Title"); 
        }
    }
}
