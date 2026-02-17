using UnityEngine;

namespace _02.Scripts.UI.Title
{
    [CreateAssetMenu(fileName = "Button_Exit", menuName = "PureData/Buttons/Exit")]
    public class TitleButton_Exit : PureDataButton
    {
        public override void Apply()
        {
            Debug.Log("[TitleButton] 게임 종료.");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
