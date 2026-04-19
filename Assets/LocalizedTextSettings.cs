using UnityEngine;
using TMPro;
using RTLTMPro;
using System.Collections.Generic;

[RequireComponent(typeof(RTLTextMeshPro))]
public class LocalizedTextSettings : MonoBehaviour
{
    [Header("Text Size")]
    [SerializeField] private float smallSize = 13f;
    [SerializeField] private float mediumSize = 15f;
    [SerializeField] private float largeSize = 17f;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset arabicFont;

    [Header("Localized Text (Index = Language Enum)")]
    [TextArea]
    public List<string> texts;

    private RTLTextMeshPro textComponent;

    private void Awake()
    {
        textComponent = GetComponent<RTLTextMeshPro>();
    }

    private void OnEnable()
    {
        SettingsManager.OnSettingsChanged += ApplySettings;
        ApplySettings();
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= ApplySettings;
    }

    private void ApplySettings()
    {
        if (SettingsManager.Instance == null || textComponent == null) return;

        ApplyLanguage();
        ApplyTextSize();
        ApplyFontAndAlignment();
    }

    private void ApplyLanguage()
    {
        int index = (int)SettingsManager.Instance.currentLanguage;

        if (texts != null && index < texts.Count && !string.IsNullOrEmpty(texts[index]))
        {
            textComponent.text = texts[index];
        }
        else if (texts != null && texts.Count > 0)
        {
            textComponent.text = texts[0];
        }
    }

    private void ApplyTextSize()
    {
        switch (SettingsManager.Instance.currentTextSize)
        {
            case SettingsManager.TextSize.Small:
                textComponent.fontSize = smallSize;
                break;

            case SettingsManager.TextSize.Medium:
                textComponent.fontSize = mediumSize;
                break;

            case SettingsManager.TextSize.Large:
                textComponent.fontSize = largeSize;
                break;
        }
    }

    private void ApplyFontAndAlignment()
    {
        bool isArabic = SettingsManager.Instance.currentLanguage == SettingsManager.Language.Arabic;

        if (isArabic)
        {
            if (arabicFont != null)
                textComponent.font = arabicFont;

            textComponent.isRightToLeftText = true;
            textComponent.alignment = TextAlignmentOptions.TopRight;
        }
        else
        {
            if (defaultFont != null)
                textComponent.font = defaultFont;

            textComponent.isRightToLeftText = false;
            textComponent.alignment = TextAlignmentOptions.TopLeft;
        }

        textComponent.ForceMeshUpdate();
    }
}