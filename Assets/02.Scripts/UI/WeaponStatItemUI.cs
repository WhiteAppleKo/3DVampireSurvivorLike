using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Features.Weapon;

namespace _02.Scripts.UI
{
    /// <summary>
    /// 결과 화면 리스트에서 개별 무기의 통계를 표시하는 UI 요소입니다.
    /// </summary>
    public class WeaponStatItemUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private TextMeshProUGUI totalDamageText;

        /// <summary>
        /// 무기 데이터와 통계를 기반으로 UI를 설정합니다.
        /// </summary>
        public void SetData(PureDataWeapon weaponData, int killCount, long totalDamage)
        {
            if (weaponData != null)
            {
                if (weaponIcon != null) weaponIcon.sprite = weaponData.Icon;
                if (weaponNameText != null) weaponNameText.text = weaponData.Name;
            }

            // 숫자만 출력하도록 변경
            if (killCountText != null) killCountText.text = killCount.ToString();
            if (totalDamageText != null) totalDamageText.text = totalDamage.ToString();
        }
    }
}
