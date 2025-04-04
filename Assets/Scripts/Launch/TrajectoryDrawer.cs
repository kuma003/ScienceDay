using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DashedLine�̃��b�p
public class TrajectoryDrawer : SynchronizeData
{
    // �O��
    [SerializeField] GameObject TraceLine;
    // �\���� (Dashed)
    [SerializeField] GameObject DashedLine;


    // �j���̒���
    [SerializeField] private float _length = 1f;

    // �j���̊Ԋu
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

    // �C���X�y�N�^�[����X�V���ꂽ��A�}�e���A�����X�V����
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        Refresh();
    }

#endif*/
}
