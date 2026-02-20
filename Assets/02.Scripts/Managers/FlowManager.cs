using UnityEngine;
using UnityEngine.SceneManagement;
using _02.Scripts.Managers.Save;
using _02.Scripts.Managers.Stage;

namespace _02.Scripts.Managers
{
    /// <summary>
    /// [L] 인게임 게임 흐름을 총괄하는 오케스트레이터입니다.
    /// 모든 매니저들의 로드 순서와 시작 시점을 제어합니다.
    /// </summary>
    public class FlowManager : SingletoneBase<FlowManager>
    {
        public bool IsGameReady { get; private set; } = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 인게임 씬(TestScene 등)일 경우에만 시퀀스를 시작합니다.
            if (scene.name.Contains("Title"))
            {
                IsGameReady = false;
                return;
            }

            StartCoroutine(Co_GameStartSequence());
        }

        private System.Collections.IEnumerator Co_GameStartSequence()
        {
            Debug.Log("[FlowManager] === 게임 시작 시퀀스 개시 ===");

            // 1. 모든 객체의 Awake가 끝날 때까지 한 프레임 대기 (Registration 완료 보장)
            yield return null;

            // 2. 데이터 복구 (Data Restoration)
            Debug.Log("[FlowManager] STEP 1: 데이터 복구 시작");
            DataHub.Instance.RestoreAll();
            
            // 3. 시스템 바인딩 및 초기화 (System Binding)
            Debug.Log("[FlowManager] STEP 2: 시스템 바인딩");
            if (SubscribeManager.Instance != null)
            {
                SubscribeManager.Instance.GameStart();
            }

            // 4. 게임 실행 (Game Start)
            Debug.Log("[FlowManager] STEP 3: 게임 실행 신호 발송");
            
            // 스테이지 시작
            if (StageManager.Instance != null)
            {
                StageManager.Instance.StartStageByFlow();
            }

            // 플레이어 및 기타 시스템 활성화 (필요 시 추가)
            
            IsGameReady = true;
            Debug.Log("[FlowManager] === 게임 시작 시퀀스 완료 ===");
        }
    }
}
