using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AltitudeDiplay : SynchronizeData
{
    [SerializeField] Texture[] icons = new Texture[0];
    [SerializeField] Color[] colors = new Color[0];
    [SerializeField] float altitude_threshold;


    private RubyTextMeshProUGUI _textMeshPro;
    private RawImage _rawImage;
    // Start is called before the first frame update
    void Start()
    {
        _textMeshPro = gameObject.GetComponentInChildren<RubyTextMeshProUGUI>();
        _rawImage = gameObject.GetComponentInChildren<RawImage>();
        
    }

    public override void Reflesh()
    {
        if (_textMeshPro == null) return;

        float alt = _coord.y > 0 ? _coord.y : 0; // �n�ʂ�艺�͖���

        // ���x�ɉ������A�C�R�� &�@color �ύX
        int idx = Mathf.FloorToInt(Mathf.Min(new float[] { alt / altitude_threshold, icons.Length-1, colors.Length-1}));
        if (idx != -1 && _rawImage.texture != icons[idx]) _rawImage.texture = icons[idx]; // �K�v�Ȃ�e�N�X�`���X�V

        _textMeshPro.uneditedText = $"<r=\"������\">���x</r> : <size=22>{alt:000.0}</size> <r=\"���[�g��\">m</r>";
    }

}
