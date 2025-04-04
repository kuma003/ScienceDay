using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : SynchronizeData
{
    private RectTransform m_RectTransform;
    private float scaling = 2f; // スケーリング値

    private void Start()
    {
        m_RectTransform = gameObject.GetComponent<RectTransform>();
        scaling = DataManager.Instance.saveData.backgroundScaling; // スケーリング値を取得
    }
    public override void Reflesh()
    {
        
        m_RectTransform.localPosition = new Vector3(0, _coord.y > 0 ? -2160 * _coord.y / 10 * scaling : 0, 400);
    }
}
