using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts.UI.Title
{
    [CreateAssetMenu(fileName = "Button_NewGame", menuName = "PureData/Buttons/NewGame")]
    public class TitleButton_LoadScene : PureDataButton
    {
        [Header("Scene Settings")]
        [SerializeField] private string targetScene = "TestScene";

        public override void Apply()
        {
            Debug.Log("[TitleButton] 새 게임 시작. 세이브 데이터를 삭제합니다.");
            DataHub.Instance.DeleteSaveData();
            SceneManager.LoadScene(targetScene);
        }
    }
}
