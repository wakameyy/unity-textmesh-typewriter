using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// TypeWriterのタグ情報
/// </summary>
[Serializable]
public class TypeWriterTag
{
    [SerializeField]
    protected string m_tag;
    [SerializeField]
    protected string m_tagType;
    [SerializeField]
    protected int m_index;
    public int Index => m_index;
    [Serializable]
    public class TypeWriterTagArgument
    {
        public string Type;
        public string Value;
        public TypeWriterTagArgument(string _type, string _value)
        {
            Type = _type;
            Value = _value;
        }
        public override string ToString()
        {
            return $"TypeWriterTagArgument : {Type} : {Value}"; ;
        }
    }
    [SerializeField]
    protected List<TypeWriterTagArgument> m_args = new List<TypeWriterTagArgument>();
    public TypeWriterTag(int index, string tag)
    {
        this.m_index = index;
        this.m_tag = tag;
        Debug.Log($"TypeWriterTag [{tag.Length}]");
        var temp = tag.Substring(1, tag.Length - 2).Trim();
        // パラメータがない
        if (!temp.Contains(" "))
        {
            this.m_tagType = temp;
        }
        else
        {
            var firstSpaceIndex = temp.IndexOf(" ");
            this.m_tagType = temp.Substring(0, firstSpaceIndex).Trim();
            var paramStr = temp.Substring(firstSpaceIndex).Trim();
            Debug.Log($"[{paramStr}]");
            ParseArguments(paramStr);
        }
        Debug.Log($"command:{this}");
    }

    public bool HasArg(string argType)
    {
        // return m_args.ContainsKey(arg);
        return m_args.Any(arg => arg.Type == argType);
    }

    public TypeWriterTagArgument GetArgValue(string argType)
    {
        if (!HasArg(argType)) return null;
        // return m_args[argType];
        return m_args.FirstOrDefault(arg => arg.Type == argType);
    }

    /// <summary>
    /// 引数の解析
    /// </summary>
    /// <param name="param"></param>
    private void ParseArguments(string param)
    {
        var str = param;
        string arg, val;
        while (_parse(ref str, out arg, out val))
        {
            m_args.Add(new TypeWriterTagArgument(arg, val));
        }
        bool _parse(ref string _str, out string _arg, out string _val)
        {
            try
            {
                var result = false;
                var isArg = true;
                var beginParseValue = false;
                var isStringValue = false;
                var isEscape = false;
                _arg = string.Empty;
                _val = string.Empty;

                int index = 0;
                int maxLength = _str.Length;
                while (index < maxLength)
                {
                    char c = str[index];
                    if (isArg)
                    {
                        // 無視する
                        if (c == '<' || c == ' ' || c == '>')
                        {
                            ++index;
                            continue;
                        }
                        // 解析終了
                        if (c == '=')
                        {
                            ++index;
                            isArg = false;
                            result = true;
                            continue;
                        }
                        ++index;
                        _arg += c;
                    }
                    else
                    {
                        // valueの解析開始
                        if (!beginParseValue)
                        {
                            beginParseValue = true;

                            if (c == '"')
                            {
                                isStringValue = true;
                                ++index;
                                continue;
                            }
                        }
                        if (isStringValue)
                        {
                            // エスケープ開始
                            if (c == '\\' && !isEscape)
                            {
                                isEscape = true;
                                ++index;
                                continue;
                            }
                            // 解析終了
                            if (c == '"' && !isEscape)
                            {
                                _str = _str.Substring(index + 1);
                                return true;
                            }
                            // 
                            if (c == '"' && isEscape)
                            {
                                isEscape = false;
                            }
                        }
                        else
                        {
                            if (c == ' ' || c == '>')
                            {
                                _str = _str.Substring(index + 1);
                                return true;
                            }
                        }
                        ++index;
                        _val += c;
                    }
                }
                // ループが周りきった
                _str = string.Empty;
                return result;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _arg = null;
                _val = null;
                return false;
            }
        }
    }
    public override string ToString()
    {
        var str = $"type:{this.m_tagType}";
        if (m_args.Any())
        {
            foreach (var item in m_args)
            {
                str += $"\n{item.ToString()}";
            }
        }
        return str;
    }
}