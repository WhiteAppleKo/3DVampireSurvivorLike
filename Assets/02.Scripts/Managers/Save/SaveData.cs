using System;
using System.Collections.Generic;

namespace _02.Scripts.Managers.Save
{
    [Serializable]
    public class GameSaveData
    {
        // === 게임 시스템 정보 ===
        public int currentStage = 1;
        public int augmentationLevel; // 증강 레벨

        public PlayerSaveData playerData;
        public AutoAttackerSaveData autoAttacker;
        // 기본 생성자
        public GameSaveData(PlayerSaveData playerData, AutoAttackerSaveData autoAttacker)
        {
            this.playerData = playerData;
            this.autoAttacker = autoAttacker;
        }

        public GameSaveData()
        {
            
        }
    }

    [Serializable]
    public class PlayerSaveData
    {
        // === 플레이어 스탯 정보 ===
        public int playerLevel = 1;
        public int currentExp;
        public int currentHp;
        
        // 획득한 증강 ID 목록 (UI 표시 또는 복구용)
        public List<string> statAugments;

        public PlayerSaveData(int playerLevel, int currentExp, int currentHp, List<string> statAugments)
        {
            this.playerLevel = playerLevel;
            this.currentExp = currentExp;
            this.currentHp = currentHp;
            this.statAugments = statAugments;
        }

        public PlayerSaveData()
        {
            
        }
    }
    [Serializable]
    public class AutoAttackerSaveData
    {
        // === 무기 상태 정보 ===
        public List<string> globalWeaponAugments;
        public List<WeaponSaveData> weaponList = new List<WeaponSaveData>();
        public AutoAttackerSaveData(List<string> globalAugmentsID, List<WeaponSaveData> weaponSaveList)
        {
            globalWeaponAugments = globalAugmentsID;
            weaponList = weaponSaveList;
        }

        public AutoAttackerSaveData()
        {
            
        }
    }

    [Serializable]
    public class WeaponSaveData
    {
        public string weaponID;
        // 해당 무기에 적용된 증강 ID들
        public List<string> localWeaponAugments;

        public WeaponSaveData(string id, List<string> localAugments)
        {
            weaponID = id;
            localWeaponAugments = localAugments;
        }

        public WeaponSaveData()
        {
            
        }
    }
}