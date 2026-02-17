using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02.Scripts.UI.Title
{
    [CreateAssetMenu(fileName = "Button_LoadGame", menuName = "PureData/Buttons/LoadGame")]
    public class TitleButton_LoadGame : PureDataButton
    {
        [Header("Scene Settings")]
        [SerializeField] private string targetScene = "TestScene";

        public override void Apply()
        {
            if (DataHub.Instance.HasSaveFile)
            {
                Debug.Log("[TitleButton] 게임 불러오기 시도.");
                DataHub.Instance.LoadGame();
                SceneManager.LoadScene(targetScene);
            }
            else
            {
                Debug.LogWarning("[TitleButton] 세이브 파일이 없습니다!");
            }
        }
    }
}
