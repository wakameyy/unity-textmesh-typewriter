using UnityEngine;

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
        switch (tag.TagType)
        {
            case "delay":
                ExecDelay(tag);
                break;
        }
    }
    private void UpdateWaitCommand()
    {
        if (isDelayCommand)
        {
            m_delayTime += Time.deltaTime;
            if (m_delayTime > m_delayDuration)
            {
                Play();
                isDelayCommand = false;
            }
        }
    }

    public bool isDelayCommand { get; private set; }
    public float m_delayDuration;
    public float m_delayTime;
    private void ExecDelay(TypeWriterTag tag)
    {
        var arg = tag.GetArgValue("time");
        if (arg == null) return;
        if (!float.TryParse(arg.Value, out float duration))
        {
            return;
        }
        m_delayTime = 0;
        m_delayDuration = duration;
        isDelayCommand = true;
        WaitCommand();
    }
}
