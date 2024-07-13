using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DashedLine�̃��b�p
public class TrajectoryDrawer : MonoBehaviour
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

    // �C���X�y�N�^�[����X�V���ꂽ��A�}�e���A�����X�V����
    private void OnValidate()
    {
        if (!UnityEditor.EditorApplication.isPlaying) return;

        Refresh();
    }

#endif*/
}
