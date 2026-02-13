using UnityEngine;
using UnityEngine.SceneManagement;
using _02.Scripts.Managers.Save;

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
            // DataHub의 데이터를 초기화하고 저장 파일을 삭제합니다.
            DataHub.Instance.DeleteSaveData();
            SceneManager.LoadScene(inGameSceneName);
        }

        private void LoadGame()
        {
            Debug.Log("[TitleLogic] 게임 불러오기 시도.");
            // 파일에서 데이터를 최신화한 후 씬을 이동합니다.
            DataHub.Instance.LoadGame();
            SceneManager.LoadScene(inGameSceneName);
        }

        private void OpenOptions()
        {
            Debug.Log("[TitleLogic] 옵션 창 열기 (UI 미구현 - 추후 추가 예정)");
            // TODO: 옵션 UI 패널을 활성화하는 로직 추가
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
