using _02.Scripts.Managers.Spawn;
using _02.Scripts.Managers.Save; // ISaveable 참조 추가
using Features.Stage;
using Shapes;
using UnityEngine;

namespace _02.Scripts.Managers.Stage
{
    public class StageManager : SingletoneBase<StageManager>, ISaveable
    {
        [Header("Stage Settings")]
        public float stageTimeLimit = 300f; // 5분
        public int currentStage = 1;
    
        [Header("DLV Data")]
        [SerializeField] private PureDataBaseStage pureDataBase;
        public SpawnManager spawnManager;

        public float ElapsedTime { get; private set; }
        public System.Action<float> OnTimeChanged;

        protected override void Awake()
        {
            base.Awake();
            ((ISaveable)this).RegistSaveAble();
        }

        private void Start()
        {
            // [Timing Fix] Start에서 즉시 시작하지 않고, LoadData()가 완료될 때까지 기다립니다.
            // DataHub가 한 프레임 뒤에 RestoreAll()을 호출해줄 것입니다.
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

        private void StartCurrentStage()
        {
            Debug.Log($"[StageManager] === 실제 스테이지 시작 시도: {currentStage} ===");
            var stageData = GetStageInformation(currentStage);
            if (stageData != null)
            {
                spawnManager.StartNewStage(stageData);
            }
            else
            {
                Debug.LogError($"[StageManager] 스테이지 {currentStage} 정보를 데이터베이스에서 찾을 수 없습니다!");
            }
        }

        private void CompleteStage()
        {
            Debug.Log($"[StageManager] 스테이지 {currentStage} 완료! 다음 스테이지로 넘어갑니다.");

            currentStage++;
            var nextStage = GetStageInformation(currentStage);
        
            if (nextStage != null)
            {
                spawnManager.StartNewStage(nextStage);
            }

            ResetTimer();
            Debug.Log("[StageManager] 타이머 초기화 완료.");

            AutoSave();

            ShowAugmentSelection();
        }

        private void AutoSave()
        {
            Debug.Log($"[StageManager] 자동 저장 프로세스 개시. 현재 스테이지: {currentStage}");
            DataHub.Instance.SaveGame();
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
                Debug.Log($"[StageManager] 스테이지({stageIndex}) 정보 조회 완료: {stageData.name}");
                return stageData;
            }
            else
            {
                Debug.LogWarning($"[StageManager] 스테이지 {stageIndex} 정보를 찾을 수 없습니다.");
                return null;
            }
        }

        #region 세이브 및 로드 (ISaveable 구현)
        public void SaveData()
        {
            // DataHub의 CurrentSaveData 필드를 직접 갱신합니다.
            if (DataHub.Instance.CurrentSaveData != null)
            {
                DataHub.Instance.CurrentSaveData.currentStage = currentStage;
                Debug.Log($"[StageManager] SaveData 성공: DataHub.currentStage를 {currentStage}로 갱신함.");
            }
            else
            {
                Debug.LogError("[StageManager] SaveData 실패: DataHub.CurrentSaveData가 Null입니다!");
            }
        }

        public void LoadData()
        {
            int loadedStage = DataHub.Instance.GetCurrentStageData();
            Debug.Log($"[StageManager] LoadData 호출: DataHub로부터 {loadedStage} 받아옴. (현재값: {currentStage})");
            currentStage = loadedStage;
        }

        /// <summary>
        /// FlowManager에 의해 호출되는 게임 시작 지점입니다.
        /// </summary>
        public void StartStageByFlow()
        {
            Debug.Log($"[StageManager] FlowManager 시작 신호 수신. 최종 스테이지: {currentStage}");
            StartCurrentStage();
        }
        #endregion
    }
}
