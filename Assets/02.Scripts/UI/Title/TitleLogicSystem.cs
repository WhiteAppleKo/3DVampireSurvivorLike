using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts.UI.Title
{
    /// <summary>
    /// [L] Title 씬의 비즈니스 로직을 담당합니다.
    /// 버튼 ID에 따라 적절한 판단을 내립니다.
    /// </summary>
    public class TitleLogicSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string inGameSceneName = "TestScene";

        /// <summary>
        /// 모든 버튼 액션을 통합 처리하는 핸들러
        /// </summary>
        public void HandleButtonAction(string buttonID)
        {
            Debug.Log($"[TitleLogic] Action Handle: {buttonID}");

            switch (buttonID)
            {
                case "NewGame":
                    StartNewGame();
                    break;
                case "Load":
                    LoadGame();
                    break;
                case "Options":
                    OpenOptions();
                    break;
                case "Exit":
                    ExitGame();
                    break;
                default:
                    Debug.LogWarning($"[TitleLogic] 알 수 없는 버튼 ID: {buttonID}");
                    break;
            }
        }

        private void StartNewGame()
        {
            Debug.Log("[TitleLogic] 새 게임 시작. 기존 데이터를 삭제합니다.");
            DataHub.Instance.DeleteSaveData();
            SceneManager.LoadScene(inGameSceneName);
        }

        private void LoadGame()
        {
            Debug.Log("[TitleLogic] 게임 불러오기 시도.");
            // DataHub는 로드된 상태이므로 씬만 이동
            SceneManager.LoadScene(inGameSceneName);
        }

        private void OpenOptions()
        {
            Debug.Log("[TitleLogic] 옵션 창 열기 (구현 예정)");
        }

        private void ExitGame()
        {
            Debug.Log("[TitleLogic] 게임 종료.");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
