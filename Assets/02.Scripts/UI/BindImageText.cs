using System;
using _02.Scripts.Managers.Choice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BindImageText : MonoBehaviour
{
    private Image m_Image;
    private TextMeshProUGUI m_TMPro;
    private BaseAbility m_Ability;

    private void Awake()
    {
        m_Image = GetComponentInChildren<Image>();
        m_TMPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        m_TMPro.text = text;
    }

    public void SetImage(Sprite sprite)
    {
        m_Image.sprite = sprite;
    }

    public void SetAbility(BaseAbility ability)
    {
        m_Ability = ability;
    }

    public BaseAbility GetAbility()
    {
        return m_Ability;
    }
}
