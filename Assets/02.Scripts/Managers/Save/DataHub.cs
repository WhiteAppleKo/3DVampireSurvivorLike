using System.Collections;
using System.Collections.Generic;
using System.IO;
using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Save;
using UnityEngine;

public class DataHub : SingletoneBase<DataHub>
{
    private const string SAVE_FILE_NAME = "GameSaveData.json";
    private ISaveSystem m_SaveSystem;

    public GameSaveData CurrentSaveData { get; private set; }
    public List<ISaveable> saveList = new List<ISaveable>();
    
    /// <summary>
    /// 실제 저장된 파일이 있는지 여부를 반환합니다.
    /// </summary>
    public bool HasSaveFile => m_SaveSystem != null && m_SaveSystem.Exists(SAVE_FILE_NAME);

    [Header("DLV Databases")]
    public Features.Weapon.PureDataBaseWeapon weaponDatabase;
    public Features.Augment.PureDataBaseWeaponAbility weaponAbilityDatabase;
    public Features.Augment.PureDataBaseStatAbility statAbilityDatabase;

    protected override void Awake()
    {
        base.Awake();
        dontDestroyOnLoad = true;
        
        // [Logic] 저장 시스템 초기화 (Json 방식 사용)
        m_SaveSystem = new JsonSaveSystem();
        
        LoadGame();
    }

    private void OnEnable()
    {
        // FlowManager에서 호출할 것이므로 씬 로드 이벤트 구독 제거
    }

    private void OnDisable()
    {
        // FlowManager에서 호출할 것이므로 씬 로드 이벤트 구독 제거
    }

    // [Refactor] FlowManager에서 명시적으로 호출합니다.
    public void RestoreAll()
    {
        Debug.Log($"[DataHub] 데이터 복구 프로세스 시작 (등록된 객체: {saveList.Count}개)");
        CleanupSaveList();
        
        for (int i = 0; i < saveList.Count; i++)
        {
            if (saveList[i] != null && (!(saveList[i] is Object obj) || obj != null))
            {
                saveList[i].LoadData();
            }
        }
    }

    private void CleanupSaveList()
    {
        for (int i = saveList.Count - 1; i >= 0; i--)
        {
            if (saveList[i] == null || (saveList[i] is Object obj && obj == null))
            {
                saveList.RemoveAt(i);
            }
        }
    }

    // --- 데이터 조회 헬퍼 메서드 ---
    public Features.Weapon.PureDataWeapon GetWeaponData(string id) => weaponDatabase?.GetData(id);
    public Features.Augment.PureDataWeaponAbility GetWeaponAbilityData(string id) => weaponAbilityDatabase?.GetData(id);
    public Features.Augment.PureDataStatAbility GetStatAbilityData(string id) => statAbilityDatabase?.GetData(id);

    /// <summary>
    /// 현재 게임 상태를 파일로 저장합니다.
    /// </summary>
    public void SaveGame()
    {
        if (CurrentSaveData == null) CreateNewSaveData();
        
        UpdateSaveData();
        
        try
        {
            m_SaveSystem.Save(SAVE_FILE_NAME, CurrentSaveData);
            Debug.Log($"[DataHub] 게임 저장 완료. (저장된 스테이지: {CurrentSaveData.currentStage})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataHub] 저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 파일에서 게임 데이터를 불러옵니다.
    /// </summary>
    public void LoadGame()
    {
        if (m_SaveSystem.Exists(SAVE_FILE_NAME))
        {
            try
            {
                CurrentSaveData = m_SaveSystem.Load<GameSaveData>(SAVE_FILE_NAME);
                Debug.Log($"[DataHub] 데이터 로드 성공. (불러온 스테이지: {CurrentSaveData.currentStage})");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DataHub] 로드 실패: {e.Message}");
                CreateNewSaveData();
            }
        }
        else
        {
            Debug.Log("[DataHub] 저장된 파일이 없어 새로 시작합니다.");
            CreateNewSaveData();
        }
    }

    public void DeleteSaveData()
    {
        m_SaveSystem.Delete(SAVE_FILE_NAME);
        CreateNewSaveData();
        Debug.Log("[DataHub] 저장 데이터 삭제 완료.");
    }

    private void CreateNewSaveData()
    {
        CurrentSaveData = new GameSaveData();
    }

    private void UpdateSaveData()
    {
        foreach (var saveable in saveList)
        {
            saveable.SaveData();
        }
    }
    public void RegistSaveData(ISaveable saveableData)
    {
        // 중복 등록 방지
        if (!saveList.Contains(saveableData))
        {
            saveList.Add(saveableData);
        }
    }

    public void SetPlayerData(PlayerSaveData saveData)
    {
        CurrentSaveData.playerData = saveData;
    }
    
    public void SetWeaponData(AutoAttackerSaveData saveData)
    {
        CurrentSaveData.autoAttacker = saveData;
    }

    public PlayerSaveData LoadPlayerSaveData()
    {
        return CurrentSaveData?.playerData;
    }

    public AutoAttackerSaveData LoadAutoAttackerSaveData()
    {
        return CurrentSaveData?.autoAttacker;
    }
    
    public int GetCurrentStageData()
    {
        return CurrentSaveData.currentStage;
    }
}
