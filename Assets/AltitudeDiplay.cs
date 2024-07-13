using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AltitudeDiplay : MonoBehaviour
{
    [SerializeField] Texture[] icons = new Texture[0];
    [SerializeField] Color[] colors = new Color[0];
    [SerializeField] float altitude_threshold;


    private float _time;
    private RubyTextMeshProUGUI _textMeshPro;
    private RawImage _rawImage;
    // Start is called before the first frame update
    void Start()
    {
        _textMeshPro = gameObject.GetComponentInChildren<RubyTextMeshProUGUI>();
        _rawImage = gameObject.GetComponentInChildren<RawImage>();
        SetTime(-5);
        Debug.Log(_textMeshPro.text);
        
    }

    public void SetTime(float time)
    {
        if (_textMeshPro == null || _time == time) return;
        _time = time;

        FlightData traj = DataManager.Instance.trajectory;
        if (traj.coord.Count == 0) return; // データがない

        // 先頭の時刻の前後のインデックス
        int idx2 = traj.time.FindIndex(t => t > time);
        if (idx2 == -1)
            idx2 = traj.time.Count - 1; // out of index
        int idx1 = (idx2 == 0) ? 0 : idx2 - 1;
        float t = Mathf.InverseLerp(traj.time[idx1], traj.time[idx2], time); // 内挿用パラメタ

        float alt = Mathf.Lerp(traj.coord[idx1].y, traj.coord[idx2].y, t);

        // 高度に応じたアイコン &　color 変更
        int idx = Mathf.FloorToInt(Mathf.Min(new float[] { alt / altitude_threshold, icons.Length-1, colors.Length-1}));
        if (idx != -1 && _rawImage.texture != icons[idx]) _rawImage.texture = icons[idx]; // 必要ならテクスチャ更新
        Debug.Log(alt);
        Debug.Log(icons.Length - 1);
        Debug.Log(colors.Length - 1);
        _textMeshPro.uneditedText = $"<r=\"こうど\">高度</r> : {(idx < 0 ? "" : "")}<size=55>{alt:000.0}</size> <r=\"メートル\">m</r>";





    }

}
