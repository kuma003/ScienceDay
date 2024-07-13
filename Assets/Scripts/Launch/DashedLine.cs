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

    // �j���̒���
    [SerializeField] public float _length = 1f;

    // �j���̊Ԋu
    [SerializeField] public float _space = 1f;

    [SerializeField] public float _lineWidth = 1f;

    // �j���p�̃}�e���A��
    [SerializeField] private Material dashedMaterial;

    private TimeRange _timeRange = new TimeRange() { start = 0, end = 10 };

    // ����\������擪�̎���
    public TimeRange timeRange
    {
        get { return _timeRange; }
        set {
            _timeRange = (value.start < value.end) ? value : new TimeRange() { start = value.end, end = value.start }; // �K�� start < end
            Refresh();
        }
    }

    // �}�e���A���̃v���p�e�BID
    private static readonly int PropLength = Shader.PropertyToID("_Length");
    private static readonly int PropSpace  = Shader.PropertyToID("_Space");

    // �}�e���A���C���X�^���X
    private Material _material;


    // �j���̒����ƊԊu���X�V����
    public void Refresh()
    {
        if (lineRenderer == null) return;

        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;

        Vector3[] Path = CalcPathFromTimeRange();

        lineRenderer.positionCount = Path.Length; // �����w��
        lineRenderer.SetPositions(Path); // ���W���Z�b�g.

        // �S�̂̒������v�Z
        var totalLength = CalculateLength();
        
        // �S�̂̒����Ɋ�Â��A�����ƃX�y�[�X�̊������v�Z
        var ratio = 1 / totalLength;
        var lengthRatio = _length * ratio;
        var spaceRatio = _space * ratio;

        // �}�e���A���Ɋ�����ݒ�
        lineRenderer.material.SetFloat(PropLength, lengthRatio);
        lineRenderer.material.SetFloat(PropSpace, spaceRatio);
    }

    void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = dashedMaterial;

        Refresh();
    }

    // ���Ԕ͈͕��̃p�X���擾
    private Vector3[] CalcPathFromTimeRange()
    {
        var traj = DataManager.Instance.trajectory;
        if (traj.coord.Count == 0) return new Vector3[0];

        // �擪�̎����̑O��̃C���f�b�N�X
        int start_idx2 = traj.time.FindIndex(time => time > _timeRange.start);
        if (start_idx2 == -1)
            start_idx2 = traj.time.Count - 1; // out of index
        int start_idx1 = (start_idx2 == 0) ? 0 : start_idx2 - 1;
        float start_t = (timeRange.start - traj.time[start_idx1]) / (traj.time[start_idx2] - traj.time[start_idx1]); // ���}�p�p�����^
        Vector3 start = Vector3.Lerp(traj.coord[start_idx1], traj.coord[start_idx2], start_t);

        // �����̎����̑O��̃C���f�b�N�X
        int end_idx1 = traj.time.FindLastIndex(time => time <= _timeRange.end); // �ꉞ��������_���Č�납��T��
        if (end_idx1 == -1)
            end_idx1 = 0; // out of index
        int end_idx2 = (end_idx1 == traj.time.Count() - 1) ?  end_idx1: end_idx1+1;

        float end_t = (timeRange.end - traj.time[end_idx1]) / (traj.time[end_idx2] - traj.time[end_idx1]); // ���}�p�p�����^
        Vector3 end = Vector3.Lerp(traj.coord[end_idx1], traj.coord[end_idx2], end_t);

        List<Vector3> returnVal = traj.coord.GetRange(start_idx2, end_idx1 - start_idx2 + 1); // �͈͕��̃f�[�^���擾.
        returnVal.Insert(0, start);
        returnVal.Add(end);

        return returnVal.ToArray();
    }


    // �S�̂̒����̒������v�Z����
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
            // ���[�v����ꍇ�́A�ŏ��ƍŌ�̒��_�̋��������Z����
            totalLength += Vector3.Distance(
                lineRenderer.GetPosition(0),
                lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        }

        return totalLength;
    }

#if UNITY_EDITOR

    // �C���X�y�N�^�[����X�V���ꂽ��A�}�e���A�����X�V����
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        Refresh();
    }

#endif
}
