using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _time;

    private void Start()
    {
        SetTime(-5);
    }

    public void SetTime(float time)
    {
        if (_time == time) return;
        _time = time;

        bool signed = (time > 0);
        time = Mathf.Abs(time);

        int min = Mathf.Min(Mathf.FloorToInt(time) / 60, 99); // 分

        float sec = Mathf.Min((Mathf.Round(time * 100f) - min * 6000) / 100.0f, 59.99f); // 秒 (0.01秒まで)

        gameObject.GetComponent<RubyTextMeshProUGUI>().uneditedText = ($"X {(signed ? "＋" : "－")} {min:00}:{sec:00.00}");
    }

#if UNITY_EDITOR

    // インスペクターから更新されたら、マテリアルを更新する
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        SetTime(_time);
    }

#endif
}
