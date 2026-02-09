using _02.Scripts.Managers.Spawn;
using Features.Stage;
using Shapes;
using UnityEngine;

namespace _02.Scripts.Managers.Stage
{
    public class StageManager : SingletoneBase<StageManager>
    {
        [Header("Stage Settings")]
        public float stageTimeLimit = 300f; // 5분
        public int currentStage = 1;
    
        [Header("DLV Data")]
        [SerializeField] private PureDataBaseStage pureDataBase;
        public SpawnManager spawnManager;

        public float ElapsedTime { get; private set; }
        public System.Action<float> OnTimeChanged;

        private void Start()
        {
            Debug.Log($"[StageManager] 스테이지 시스템 시작. 현재 스테이지: {currentStage}, 제한 시간: {stageTimeLimit}초");
            currentStage = DataHub.Instance.GetCurrentStageData();
            
            var stageData = GetStageInformation(currentStage);
            if (stageData != null)
            {
                spawnManager.StartNewStage(stageData);
                
                // [추가] 게임 시작 시 첫 무기 선택 UI 팝업
                if (currentStage == 1 && ExpManager.Instance != null)
                {
                    ExpManager.Instance.ChoiceFirstWeapon();
                }
            }
            
            ResetTimer();
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime;
            OnTimeChanged?.Invoke(ElapsedTime);
            CheckStageTime();
        }

        private void CheckStageTime()
        {
            if (ElapsedTime >= stageTimeLimit)
            {
                CompleteStage();
            }
        }

        public void ResetTimer()
        {
            ElapsedTime = 0;
            OnTimeChanged?.Invoke(ElapsedTime);
        }

        private void CompleteStage()
        {
            Debug.Log($"[StageManager] 스테이지 {currentStage} 완료!");

            currentStage++;
            var nextStage = GetStageInformation(currentStage);
        
            if (nextStage != null)
            {
                spawnManager.StartNewStage(nextStage);
            }

            ResetTimer();
            Debug.Log("[StageManager] 타이머가 초기화되었습니다.");

            AutoSave();

            ShowAugmentSelection();
        }

        private void AutoSave()
        {
            DataHub.Instance.SaveGame();
            Debug.Log($"[StageManager] 스테이지 {currentStage} 데이터 자동 저장 완료.");
        }

        private void ShowAugmentSelection()
        {
            Debug.Log("[StageManager] 스페셜 증강 선택 창을 엽니다.");
        }

        public PureDataStage GetStageInformation(int stageIndex)
        {
            if (pureDataBase == null)
            {
                Debug.LogError("[StageManager] PureDataBaseStage가 할당되지 않았습니다.");
                return null;
            }

            string stageID = stageIndex.ToString();
            var stageData = pureDataBase.GetData(stageID);

            if (stageData == null)
            {
                if (stageIndex - 1 < pureDataBase.StageList.Count)
                {
                    stageData = pureDataBase.StageList[stageIndex - 1];
                }
            }

            if (stageData != null)
            {
                Debug.Log($"[StageManager] 스테이지({stageIndex}) 정보 로드 완료: {stageData.name}");
                return stageData;
            }
            else
            {
                Debug.LogWarning($"[StageManager] 스테이지 {stageIndex} 정보를 찾을 수 없습니다.");
                return null;
            }
        }
    }
}
