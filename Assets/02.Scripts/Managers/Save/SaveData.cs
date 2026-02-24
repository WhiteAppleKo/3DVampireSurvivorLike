using System;
using System.Collections.Generic;

namespace _02.Scripts.Managers.Save
{
    [Serializable]
    public class GameSaveData
    {
        // === 게임 시스템 정보 (인게임 전용, 초기화 대상) ===
        public int currentStage = 1;
        public int augmentationLevel = 1;

        public PlayerSaveData playerData = new PlayerSaveData();
        public AutoAttackerSaveData autoAttacker = new AutoAttackerSaveData();

        public GameSaveData(PlayerSaveData playerData, AutoAttackerSaveData autoAttacker)
        {
            this.playerData = playerData;
            this.autoAttacker = autoAttacker;
        }

        public GameSaveData()
        {
        }
    }

    /// <summary>
    /// 영구적인 계정 정보를 담는 데이터 클래스 (별도 파일로 관리)
    /// </summary>
    [Serializable]
    public class AccountData
    {
        public long totalPoints = 0;
        public List<string> unlockedFeatureIDs = new List<string>();

        public AccountData()
        {
            totalPoints = 0;
            unlockedFeatureIDs = new List<string>();
        }
    }

    [Serializable]
    public class PlayerSaveData
    {
        // === 플레이어 스탯 정보 ===
        public int playerLevel = 1;
        public int currentExp;
        public int currentHp;
        
        // 획득한 증강 ID 목록
        public List<string> statAugments = new List<string>();

        public PlayerSaveData(int playerLevel, int currentExp, int currentHp, List<string> statAugments)
        {
            this.playerLevel = playerLevel;
            this.currentExp = currentExp;
            this.currentHp = currentHp;
            this.statAugments = statAugments ?? new List<string>();
        }

        public PlayerSaveData()
        {
        }
    }

    [Serializable]
    public class AutoAttackerSaveData
    {
        // === 무기 상태 정보 ===
        public List<string> globalWeaponAugments = new List<string>();
        public List<WeaponSaveData> weaponList = new List<WeaponSaveData>();

        public AutoAttackerSaveData(List<string> globalAugmentsID, List<WeaponSaveData> weaponSaveList)
        {
            globalWeaponAugments = globalAugmentsID ?? new List<string>();
            weaponList = weaponSaveList ?? new List<WeaponSaveData>();
        }

        public AutoAttackerSaveData()
        {
        }
    }

    [Serializable]
    public class WeaponSaveData
    {
        public string weaponID;
        public List<string> localWeaponAugments = new List<string>();

        public WeaponSaveData(string id, List<string> localAugments)
        {
            weaponID = id;
            localWeaponAugments = localAugments ?? new List<string>();
        }

        public WeaponSaveData()
        {
        }
    }
}
