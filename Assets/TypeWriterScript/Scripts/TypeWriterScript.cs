using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// タイプライターテキスト
/// </summary>
public partial class TypeWriterScript : MonoBehaviour, ITextPreprocessor
{
    public enum State
    {
        /// <summary> 初期化前</summary>
        NONE,
        /// <summary> 再生中</summary>
        PLAYING,
        /// <summary> 一時停止</summary>
        PAUSE,
        /// <summary> 停止</summary>
        STOP,
        /// <summary> Skip</summary>
        SKIP,
        /// <summary> 終了</summary>
        END,
    }
    /// <summary> 表示に使用するテキスト</summary>
    [SerializeField] private ITypeWtiterTextComponent m_textComponent;
    /// <summary> 病患の表示文字数</summary>
    [SerializeField] private int m_defaultNumPerSec = 1;
    private int m_numPerSec = 1;
    /// <summary> 再生状態</summary>
    [SerializeField] private State m_state;
    /// <summary> 再生状態。</summary>
    public State state => m_state;
    /// <summary> 表示済みのテキスト</summary>
    private int m_displayedCharacterCount;
    /// <summary> 経過時間</summary>
    private float m_time = 0;
    /// <summary> 倍速係数</summary>
    private float m_speedRatio = 1f;
    /// <summary> 終了コールバック</summary>
    private Action m_onEndText;

#if UNITY_EDITOR
    // m_tagsの確認用
    [SerializeField]
#endif
    /// <summary> テキスト内のタグ情報</summary>
    private LinkedList<TypeWriterTag> m_tags = new LinkedList<TypeWriterTag>();
    private LinkedListNode<TypeWriterTag> m_node;
    /// <summary> オリジナルテキスト</summary>
    private string m_originText;
    /// <summary>  前回の表示済みインデックス</summary>
    private int m_prevDisplayedCharacterCount;
    /// <summary> テキスト内のタグ情報</summary>
    private string m_reserveText = string.Empty;
    private void Awake()
    {
        m_originText = m_textComponent.text;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="numPerSec"></param>
    public void Init(int numPerSec)
    {
        m_numPerSec = numPerSec;
        m_textComponent.textPreprocessor = PreprocessText;
        m_textComponent.text = ParseTag(m_originText, ref m_tags);
        if (m_tags.Count > 0)
        {
            m_node = m_tags.First;
        }
        m_textComponent.Init();
    }

    public string PreprocessText(string text)
    {
        if (m_displayedCharacterCount > m_prevDisplayedCharacterCount)
        {
            m_reserveText += text.Substring(m_prevDisplayedCharacterCount, m_displayedCharacterCount - m_prevDisplayedCharacterCount);
            while (m_node != null)
            {
                if (m_node.Value.Index > m_displayedCharacterCount)
                {
                    break;
                }
                var node = m_node;
                m_node = node.Next;
                ExecTag(node.Value);
            }
        }

        return text;
    }
    public static string ParseTag(string text, ref LinkedList<TypeWriterTag> tags)
    {
        tags.Clear();

        var _text = text;
        while (GetTag(ref _text, out var tag))
        {
            tags.AddLast(tag);
        }
        return _text;
    }
    public static bool GetTag(ref string text, out TypeWriterTag tag)
    {
        var match = Regex.Match(text, "<(.*?)>");
        if (match.Groups.Any() && match.Groups[0].Value.Count() != 0)
        {
            var capture = match.Groups[0];
            var str1 = text.Substring(0, capture.Index);
            var str2 = text.Substring(capture.Index + capture.Value.Length);
            text = str1 + str2;
            tag = new TypeWriterTag(capture.Index, capture.Value);
            return true;
        }
        tag = null;
        return false;
    }


    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        m_textComponent.maxVisibleCharacters = 0;
        m_displayedCharacterCount = 0;
        m_time = 0;
    }
    public void PlayText(string text, Action onEndText = null)
    {
        m_onEndText = onEndText;
        m_state = State.NONE;
        Reset();
        m_textComponent.text = text;
        Init(m_defaultNumPerSec);
        Play();
    }
    public void Play()
    {
        m_state = State.PLAYING;
    }

    public void Pause()
    {
        m_state = State.PAUSE;
    }
    public void Stop()
    {
        m_state = State.STOP;
        m_displayedCharacterCount = 0;
    }

    public void Skip()
    {
        m_state = State.SKIP;
        m_displayedCharacterCount = m_textComponent.characterCount;
        m_textComponent.maxVisibleCharacters = m_displayedCharacterCount;
        OnEnd();
    }

    public bool IsPlaying()
    {
        return m_state == State.PLAYING;
    }
    private void OnEnd()
    {
        m_state = State.END;
        Action end = m_onEndText;
        m_onEndText = null;
        end?.Invoke();
    }

    private void Update()
    {
        if (!IsPlaying())
        {
            return;
        }

        m_time += Time.deltaTime * m_speedRatio;
        // 文字カウントの閾値
        var threshold = 1f / m_numPerSec;
        if (m_time < threshold)
        {
            return;
        }
        // 経過時間が何文字分かをチェックする
        var length = (int)(m_time / threshold);
        m_prevDisplayedCharacterCount = m_displayedCharacterCount;
        m_displayedCharacterCount += length;
        m_textComponent.maxVisibleCharacters = m_displayedCharacterCount;
        m_time -= length * threshold;

        // すべての文字を読みこんだら
        if (m_textComponent.characterCount == m_displayedCharacterCount)
        {
            OnEnd();
        }
    }
}
