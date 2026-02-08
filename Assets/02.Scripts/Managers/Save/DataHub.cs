using System.Collections.Generic;
using System.IO;
using _02.Scripts.AutoAttack;
using _02.Scripts.Managers.Save;
using UnityEngine;

public class DataHub : SingletoneBase<DataHub>
{
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "GameSaveData.json");

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
        if (CurrentSaveData == null)
        {
            CurrentSaveData = new GameSaveData();
        }
        
        UpdateSaveData();
        
        string json = JsonUtility.ToJson(CurrentSaveData, true); 
        
        try
        {
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"[DataHub] 게임 저장 완료: {SaveFilePath}");
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
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                CurrentSaveData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log("[DataHub] 데이터 로드 성공.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[DataHub] 로드 실패 (파일 손상 가능성): {e.Message}");
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
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("[DataHub] 저장 파일 삭제 완료.");
        }
        CreateNewSaveData();
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
