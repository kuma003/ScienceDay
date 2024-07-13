using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

public struct TimeRange
{
    public float start;
    public float end;
}


public class DashedLine : MonoBehaviour
{
    LineRenderer lineRenderer;

    // 破線の長さ
    [SerializeField] public float _length = 1f;

    // 破線の間隔
    [SerializeField] public float _space = 1f;

    [SerializeField] public float _lineWidth = 1f;

    // 破線用のマテリアル
    [SerializeField] private Material dashedMaterial;

    private TimeRange _timeRange = new TimeRange() { start = 0, end = 10 };

    // 線を表示する先頭の時刻
    public TimeRange timeRange
    {
        get { return _timeRange; }
        set {
            _timeRange = (value.start < value.end) ? value : new TimeRange() { start = value.end, end = value.start }; // 必ず start < end
            Refresh();
        }
    }

    // マテリアルのプロパティID
    private static readonly int PropLength = Shader.PropertyToID("_Length");
    private static readonly int PropSpace  = Shader.PropertyToID("_Space");

    // マテリアルインスタンス
    private Material _material;


    // 破線の長さと間隔を更新する
    public void Refresh()
    {
        if (lineRenderer == null) return;

        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;

        Vector3[] Path = CalcPathFromTimeRange();

        lineRenderer.positionCount = Path.Length; // 個数を指定
        lineRenderer.SetPositions(Path); // 座標をセット.

        // 全体の長さを計算
        var totalLength = CalculateLength();
        
        // 全体の長さに基づき、長さとスペースの割合を計算
        var ratio = 1 / totalLength;
        var lengthRatio = _length * ratio;
        var spaceRatio = _space * ratio;

        // マテリアルに割合を設定
        lineRenderer.material.SetFloat(PropLength, lengthRatio);
        lineRenderer.material.SetFloat(PropSpace, spaceRatio);
    }

    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = dashedMaterial;

        Refresh();
    }

    // 時間範囲分のパスを取得
    private Vector3[] CalcPathFromTimeRange()
    {
        var traj = DataManager.Instance.trajectory;
        if (traj.coord.Count == 0) return new Vector3[0];

        // 先頭の時刻の前後のインデックス
        int start_idx2 = traj.time.FindIndex(time => time > _timeRange.start);
        if (start_idx2 == -1)
            start_idx2 = traj.time.Count - 1; // out of index
        int start_idx1 = (start_idx2 == 0) ? 0 : start_idx2 - 1;
        float start_t = (timeRange.start - traj.time[start_idx1]) / (traj.time[start_idx2] - traj.time[start_idx1]); // 内挿用パラメタ
        Vector3 start = Vector3.Lerp(traj.coord[start_idx1], traj.coord[start_idx2], start_t);

        // 末尾の時刻の前後のインデックス
        int end_idx1 = traj.time.FindLastIndex(time => time <= _timeRange.end); // 一応高速化を狙って後ろから探す
        if (end_idx1 == -1)
            end_idx1 = 0; // out of index
        int end_idx2 = (end_idx1 == traj.time.Count() - 1) ?  end_idx1: end_idx1+1;

        float end_t = (timeRange.end - traj.time[end_idx1]) / (traj.time[end_idx2] - traj.time[end_idx1]); // 内挿用パラメタ
        Vector3 end = Vector3.Lerp(traj.coord[end_idx1], traj.coord[end_idx2], end_t);

        List<Vector3> returnVal = traj.coord.GetRange(start_idx2, end_idx1 - start_idx2 + 1); // 範囲分のデータを取得.
        returnVal.Insert(0, start);
        returnVal.Add(end);

        return returnVal.ToArray();
    }


    // 全体の直線の長さを計算する
    private float CalculateLength()
    {
        var totalLength = 0f;

        for (var i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            totalLength += Vector3.Distance(
                lineRenderer.GetPosition(i),
                lineRenderer.GetPosition(i + 1)
            );
        }

        if (lineRenderer.loop)
        {
            // ループする場合は、最初と最後の頂点の距離も加算する
            totalLength += Vector3.Distance(
                lineRenderer.GetPosition(0),
                lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        }

        return totalLength;
    }

#if UNITY_EDITOR

    // インスペクターから更新されたら、マテリアルを更新する
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        Refresh();
    }

#endif
}
