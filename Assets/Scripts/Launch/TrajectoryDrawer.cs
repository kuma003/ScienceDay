using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DashedLineのラッパ
public class TrajectoryDrawer : SynchronizeData
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

    [SerializeField] public Vector3 offset = Vector3.zero;


    private void Start()
    {
        TraceLine.GetComponent<DashedLine>()._length = _length;
        TraceLine.GetComponent<DashedLine>()._space = 0;
        TraceLine.GetComponent<DashedLine>()._lineWidth = _lineWidth;
        TraceLine.GetComponent<DashedLine>().startPoint = offset;


        DashedLine.GetComponent<DashedLine>()._length = _length;
        DashedLine.GetComponent<DashedLine>()._space = _space;
        DashedLine.GetComponent<DashedLine>()._lineWidth = _lineWidth;
        DashedLine.GetComponent<DashedLine>().startPoint = offset;
    }

    public override void Reflesh()
    {
        TraceLine.GetComponent<DashedLine>().timeRange = new TimeRange { start = 0, end = _time };
        DashedLine.GetComponent<DashedLine>().timeRange = new TimeRange { start = _time, end = float.MaxValue };
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
