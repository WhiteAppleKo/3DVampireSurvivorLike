using _02.Scripts.Managers.Spawn;
using Shapes;
using UnityEngine;

namespace _02.Scripts.Managers.Stage
{
    public class StageManager : SingletoneBase<StageManager>
    {
        [Header("Stage Settings")]
        public float stageTimeLimit = 300f; // 5분
        public int currentStage = 1;
    
        public StageDataBase stageDataBase;
        public SpawnManager spawnManager;

        private void Start()
        {
            Debug.Log($"[StageManager] 스테이지 시스템 시작. 현재 스테이지: {currentStage}, 제한 시간: {stageTimeLimit}초");
            spawnManager.StartNewStage(GetStageInformation(currentStage));
        }

        private void Update()
        {
            CheckStageTime();
        }

        /// <summary>
        /// 현재 타이머 시간을 확인하여 제한 시간을 넘겼는지 검사합니다.
        /// </summary>
        private void CheckStageTime()
        {
            if (IMTimer.Instance != null && IMTimer.Instance.ElapsedTime >= stageTimeLimit)
            {
                CompleteStage();
            }
        }

        /// <summary>
        /// 스테이지 완료 시 호출되는 메서드입니다.
        /// </summary>
        private void CompleteStage()
        {
            Debug.Log($"[StageManager] 스테이지 {currentStage} 완료!");

            // 1. 게임 일시정지 (선택사항, 증강 선택 시 멈추는 것이 일반적)
            // Time.timeScale = 0f; 

            // 2. 스테이지 정보 갱신
            currentStage++;
            var nextStage = GetStageInformation(currentStage);
        
            // 3. 타이머 초기화
            IMTimer.Instance.ResetTimer();
            Debug.Log("[StageManager] 타이머가 초기화되었습니다.");

            // 4. 자동 저장
            AutoSave();

            // 5. 증강 선택지 제시
            ShowAugmentSelection();
        }

        /// <summary>
        /// 현재 진행 상황을 저장합니다.
        /// </summary>
        private void AutoSave()
        {
            SaveManager.Instance.SaveGame();
            Debug.Log($"[StageManager] 스테이지 {currentStage} 데이터 자동 저장 완료.");
        }

        /// <summary>
        /// 스페셜 증강 선택 UI를 표시합니다.
        /// </summary>
        private void ShowAugmentSelection()
        {
            // TODO: 실제 증강 선택 UI 호출 로직 구현 필요
            Debug.Log("[StageManager] 스페셜 증강 선택 창을 엽니다.");
        }

        /// <summary>
        /// 다음 스테이지의 정보를 데이터베이스에서 가져와 설정합니다.
        /// </summary>
        /// <returns>다음 스테이지 데이터</returns>
        public StageData GetStageInformation(int stageIndex)
        {
            if (stageDataBase == null || stageDataBase.stageDatas == null || stageDataBase.stageDatas.Count == 0)
            {
                Debug.LogError("[StageManager] StageDataBase가 비어있거나 할당되지 않았습니다.");
                return null;
            }

            // 리스트 인덱스는 0부터 시작하므로 currentStage - 1 사용
            int nextIndex = stageIndex - 1;

            if (nextIndex < stageDataBase.stageDatas.Count)
            {
                StageData nextData = stageDataBase.stageDatas[nextIndex];
                Debug.Log($"[StageManager] 다음 스테이지({stageIndex}) 정보 로드 완료: {nextData.name}");
                return nextData;
            }
            else
            {
                Debug.LogWarning("[StageManager] 더 이상 다음 스테이지가 없습니다. 마지막 스테이지를 반복하거나 게임 클리어 처리가 필요합니다.");
                return null;
            }
        }
    }
}
