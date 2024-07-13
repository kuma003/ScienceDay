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
        if (traj.coord.Count == 0) return; // �f�[�^���Ȃ�

        // �擪�̎����̑O��̃C���f�b�N�X
        int idx2 = traj.time.FindIndex(t => t > time);
        if (idx2 == -1)
            idx2 = traj.time.Count - 1; // out of index
        int idx1 = (idx2 == 0) ? 0 : idx2 - 1;
        float t = Mathf.InverseLerp(traj.time[idx1], traj.time[idx2], time); // ���}�p�p�����^

        float alt = Mathf.Lerp(traj.coord[idx1].y, traj.coord[idx2].y, t);

        // ���x�ɉ������A�C�R�� &�@color �ύX
        int idx = Mathf.FloorToInt(Mathf.Min(new float[] { alt / altitude_threshold, icons.Length-1, colors.Length-1}));
        if (idx != -1 && _rawImage.texture != icons[idx]) _rawImage.texture = icons[idx]; // �K�v�Ȃ�e�N�X�`���X�V
        Debug.Log(alt);
        Debug.Log(icons.Length - 1);
        Debug.Log(colors.Length - 1);
        _textMeshPro.uneditedText = $"<r=\"������\">���x</r> : {(idx < 0 ? "" : "")}<size=55>{alt:000.0}</size> <r=\"���[�g��\">m</r>";





    }

}
