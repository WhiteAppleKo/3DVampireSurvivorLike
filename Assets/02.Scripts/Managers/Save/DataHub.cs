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
            Debug.Log($"[DataHub] 게임 저장 완료.");
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
                Debug.Log("[DataHub] 데이터 로드 성공.");
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
        saveList.Add(saveableData);
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
