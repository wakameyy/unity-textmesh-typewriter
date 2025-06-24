using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// タイプライターテキスト
/// </summary>
public partial class TypeWriterScript
{
    /*
     * delay ... 待つコマンド
     * speed ... スピード調整
     * その他 ... コールバックされる
     */
    private void ExecTag(TypeWriterTag tag)
    {
        Debug.Log($"Exec tag {tag.ToString()}");
        
    }
}
