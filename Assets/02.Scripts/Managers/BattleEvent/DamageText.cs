using UnityEngine;
using TMPro;

namespace _02.Scripts.Managers.BattleEvent
{
    public class DamageText : MonoBehaviour
    {
        private float m_FadeDuration = 1f;
        private float m_Speed = 2f;
        
        private TextMeshPro m_TextMesh;
        private Color m_OriginalColor;
        private float m_Timer;

        private void Awake()
        {
            m_TextMesh = GetComponent<TextMeshPro>();
            m_OriginalColor = m_TextMesh.color;
        }

        private void Update()
        {
            m_Timer += Time.deltaTime;
            
            //위로 이동
            transform.position += Vector3.forward * m_Speed * Time.deltaTime;
            
            //페이드 아웃
            float alpha = Mathf.Lerp(1f, 0f, m_Timer / m_FadeDuration);
            m_TextMesh.color = new Color(m_OriginalColor.r, m_OriginalColor.g, m_OriginalColor.b, alpha);

            if (m_Timer >= m_FadeDuration)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetUp(int damageAmount, Vector3 startPos, float textduration, float speed)
        {
            m_TextMesh.text = damageAmount.ToString();
            transform.position = startPos;
            m_Timer = 0f;
            m_Speed = speed;
            m_FadeDuration = textduration;
            m_TextMesh.color = m_OriginalColor;
            gameObject.SetActive(true);
        }
    }
}
