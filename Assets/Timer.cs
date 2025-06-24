using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Timer : MonoBehaviour
{
    public bool reset;
    public TextMeshProUGUI m_text;
    public TextMeshProUGUI m_text2;
    public TypeWriterScript typeWriterScript;
    private float m_time = 0;
    public string str;
    public int cps = 1;
    void Start()
    {
        str = m_text2.text;
        typeWriterScript.Init(cps);
    }

    void OnGUI()
    {
        if (GUILayout.Button("PlayText"))
        {
            typeWriterScript.PlayText(str, () => Debug.Log("onEnd!!!"));
        }
        if (GUILayout.Button("Play"))
        {
            typeWriterScript.Play();
        }
        if (GUILayout.Button("Pause"))
        {
            typeWriterScript.Pause();
        }
        if (GUILayout.Button("Stop"))
        {
            typeWriterScript.Stop();
        }
        if (GUILayout.Button("Skip"))
        {
            typeWriterScript.Skip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            m_time = 0;
            reset = false;
        }
        m_time += Time.deltaTime;
        m_text.text =
                    " | total: " + m_text2.textInfo.characterCount +
                    " | visibleText: " + m_text2.maxVisibleCharacters +
                    " | len: " + m_text2.text.Length +
                    " | " + m_time.ToString("0.00") + " sec";
    }
}
