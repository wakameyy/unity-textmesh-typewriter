#if ENABLE_TEXTMESH_PRO_TYPEWRITERSCRIPT
using System;
using TMPro;
using UnityEngine;

public class TypeWtiterTextComponentForTextMeshPro : ITypeWtiterTextComponent, ITextPreprocessor
{
    [SerializeField] private TMP_Text m_text;
    public override string text
    {
        get
        {
            return m_text.text;
        }
        set
        {
            m_text.text = value;
        }
    }
    public override int maxVisibleCharacters
    {
        get
        {
            return m_text.maxVisibleCharacters;
        }
        set
        {
            m_text.maxVisibleCharacters = value;
        }
    }
    public override int characterCount
    {
        get
        {
            return m_text.textInfo.characterCount;
        }
        set
        {
            m_text.textInfo.characterCount = value;
        }
    }
    public override Func<string, string> textPreprocessor { get; set; }
    public override Action<string> onTextPreprocessor { get; set; }
    public override void Init()
    {
        m_text.textPreprocessor = this;
    }
    public string PreprocessText(string text)
    {
        onTextPreprocessor?.Invoke(text);
        return textPreprocessor?.Invoke(text);
    }
}
#endif // ENABLE_TEXTMESH_PRO_TYPEWRITERSCRIPT
