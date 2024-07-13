using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : SynchronizeData
{

    public override void Reflesh()
    {
        bool signed = (_time > 0);
        var time = Mathf.Abs(_time);

        int min = Mathf.Min(Mathf.FloorToInt(time) / 60, 99); // ��

        float sec = Mathf.Min((Mathf.Round(time * 100f) - min * 6000) / 100.0f, 59.99f); // �b (0.01�b�܂�)

        gameObject.GetComponent<RubyTextMeshProUGUI>().uneditedText = ($"X {(signed ? "�{" : "�|")} {min:00}:{sec:00.00}");
    }

}
