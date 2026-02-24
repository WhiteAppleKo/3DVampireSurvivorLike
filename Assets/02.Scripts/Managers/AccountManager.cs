using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using _02.Scripts.Managers.Save;

namespace _02.Scripts.Managers
{
    /// <summary>
    /// 플레이어 계정 데이터(누적 포인트, 해금 요소 등)를 관리하는 매니저입니다.
    /// 게임 세션(Run) 초기화와 무관하게 유지되는 영구 데이터를 관리합니다.
    /// </summary>
    public class AccountManager : SingletoneBase<AccountManager>
    {
        private AccountData _accountData = new AccountData();
        private string _savePath;

        public long TotalPoints => _accountData.totalPoints;

        protected override void Awake()
        {
            base.Awake();
            _savePath = Path.Combine(Application.persistentDataPath, "AccountData.json");
            LoadAccountData();
        }

        /// <summary>
        /// 이번 판에서 획득한 포인트를 계산하여 계정에 합산합니다.
        /// </summary>
        public long AddPointsFromRun(int level)
        {
            if (level <= 0) return 0;

            const int basePoint = 100;
            long earnedPoints = ((long)level * (level + 1) / 2) * basePoint;
            
            _accountData.totalPoints += earnedPoints;
            SaveAccountData();
            
            return earnedPoints;
        }

        public void SaveAccountData()
        {
            try
            {
                string json = JsonUtility.ToJson(_accountData, true);
                File.WriteAllText(_savePath, json);
                Debug.Log($"[AccountManager] Account Data Saved to {_savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AccountManager] Failed to save account data: {e.Message}");
            }
        }

        public void LoadAccountData()
        {
            try
            {
                if (File.Exists(_savePath))
                {
                    string json = File.ReadAllText(_savePath);
                    _accountData = JsonUtility.FromJson<AccountData>(json);
                    Debug.Log("[AccountManager] Account Data Loaded.");
                }
                else
                {
                    _accountData = new AccountData();
                    SaveAccountData(); // 초기 파일 생성
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[AccountManager] Failed to load account data: {e.Message}");
                _accountData = new AccountData();
            }
        }
        
        public bool IsFeatureUnlocked(string featureID)
        {
            return _accountData.unlockedFeatureIDs != null && _accountData.unlockedFeatureIDs.Contains(featureID);
        }

        public void UnlockFeature(string featureID)
        {
            if (_accountData.unlockedFeatureIDs == null) _accountData.unlockedFeatureIDs = new List<string>();
            
            if (!_accountData.unlockedFeatureIDs.Contains(featureID))
            {
                _accountData.unlockedFeatureIDs.Add(featureID);
                SaveAccountData();
            }
        }

        public bool SpendPoints(long amount)
        {
            if (_accountData.totalPoints >= amount)
            {
                _accountData.totalPoints -= amount;
                SaveAccountData();
                return true;
            }
            return false;
        }
    }
}
