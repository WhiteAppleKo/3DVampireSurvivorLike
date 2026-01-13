using System.Collections.Generic;
using System.IO;
using _02.Scripts.Managers.Save;
using UnityEngine;

public class SaveManager : SingletoneBase<SaveManager>
{
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "GameSaveData.json");

    public GameSaveData CurrentSaveData { get; private set; }
    public List<ISaveable> saveList = new List<ISaveable>();

    protected override void Awake()
    {
        base.Awake();
        dontDestroyOnLoad = true;
        
        // 게임 시작 시 데이터를 로드하거나 새로 생성
        LoadGame();
    }

    /// <summary>
    /// 현재 게임 상태를 파일로 저장합니다.
    /// StageManager나 PlayerController에서 호출하여 데이터를 채운 뒤 이 메서드를 부르세요.
    /// </summary>
    public void SaveGame()
    {
        if (CurrentSaveData == null)
        {
            CurrentSaveData = new GameSaveData();
        }

        // TODO: 실제 게임 오브젝트들(플레이어, 무기 매니저 등)로부터 최신 값을 
        // CurrentSaveData에 갱신하는 로직이 필요할 수 있습니다. 
        // 예: CollectDataFromPlayer();

        string json = JsonUtility.ToJson(CurrentSaveData, true); // true: 가독성 좋게 줄바꿈 포함
        
        try
        {
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"[SaveManager] 게임 저장 완료: {SaveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 파일에서 게임 데이터를 불러옵니다. 파일이 없으면 새 데이터를 만듭니다.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                CurrentSaveData = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log("[SaveManager] 데이터 로드 성공.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveManager] 로드 실패 (파일 손상 가능성): {e.Message}");
                // 실패 시 새 데이터로 시작
                CreateNewSaveData();
            }
        }
        else
        {
            Debug.Log("[SaveManager] 저장된 파일이 없어 새로 시작합니다.");
            CreateNewSaveData();
        }
    }

    /// <summary>
    /// 저장된 데이터를 삭제하고 초기화합니다.
    /// </summary>
    public void DeleteSaveData()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("[SaveManager] 저장 파일 삭제 완료.");
        }
        CreateNewSaveData();
    }

    private void CreateNewSaveData()
    {
        CurrentSaveData = new GameSaveData();
        // 초기값 설정이 필요하다면 여기서 진행
    }

    public void RegistSaveData(ISaveable saveableData)
    {
        saveList.Add(saveableData);
    }
}
