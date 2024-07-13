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

        float alt = _coord.y;

        // 高度に応じたアイコン &　color 変更
        int idx = Mathf.FloorToInt(Mathf.Min(new float[] { alt / altitude_threshold, icons.Length-1, colors.Length-1}));
        if (idx != -1 && _rawImage.texture != icons[idx]) _rawImage.texture = icons[idx]; // 必要ならテクスチャ更新

        _textMeshPro.uneditedText = $"<r=\"こうど\">高度</r> : {(idx < 0 ? "" : "")}<size=55>{alt:000.0}</size> <r=\"メートル\">m</r>";
    }

}
