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

        int min = Mathf.Min(Mathf.FloorToInt(time) / 60, 99); // ��

        float sec = Mathf.Min((Mathf.Round(time * 100f) - min * 6000) / 100.0f, 59.99f); // �b (0.01�b�܂�)

        gameObject.GetComponent<RubyTextMeshProUGUI>().uneditedText = ($"X {(signed ? "�{" : "�|")} {min:00}:{sec:00.00}");
    }

#if UNITY_EDITOR

    // �C���X�y�N�^�[����X�V���ꂽ��A�}�e���A�����X�V����
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        SetTime(_time);
    }

#endif
}
