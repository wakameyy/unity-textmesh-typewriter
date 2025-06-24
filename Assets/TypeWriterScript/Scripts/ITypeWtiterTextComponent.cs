using System;
using UnityEngine;

public abstract class ITypeWtiterTextComponent : MonoBehaviour
{
    public abstract string text { get; set; }
    public abstract int maxVisibleCharacters { get; set; }
    public abstract int characterCount { get; set; }
    public abstract Func<string, string> textPreprocessor { get; set; }
    public abstract Action<string> onTextPreprocessor { get; set; }
    public abstract void Init();
}