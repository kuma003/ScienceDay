using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DashedLineのラッパ
public class TrajectoryDrawer : MonoBehaviour
{
    // 軌跡
    [SerializeField] GameObject TraceLine;
    // 予測線 (Dashed)
    [SerializeField] GameObject DashedLine;


    // 破線の長さ
    [SerializeField] private float _length = 1f;

    // 破線の間隔
    [SerializeField] private float _space = 1f;

    [SerializeField] private float _lineWidth = 1f;

    private float time;

    private void Start()
    {
        TraceLine.GetComponent<DashedLine>()._length = _length;
        TraceLine.GetComponent<DashedLine>()._space = 0;
        TraceLine.GetComponent<DashedLine>()._lineWidth = _lineWidth;

        DashedLine.GetComponent<DashedLine>()._length = _length;
        DashedLine.GetComponent<DashedLine>()._space = _space;
        DashedLine.GetComponent<DashedLine>()._lineWidth = _lineWidth;
    }

    public void SetTime(float time)
    {
        if (this.time == time) return;
        this.time = time; 
        TraceLine.GetComponent<DashedLine>().timeRange = new TimeRange { start = 0, end = time };
        DashedLine.GetComponent<DashedLine>().timeRange = new TimeRange { start = time, end = float.MaxValue };
        Debug.Log(time);
    }

/*#if UNITY_EDITOR

    // インスペクターから更新されたら、マテリアルを更新する
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        Refresh();
    }

#endif*/
}
