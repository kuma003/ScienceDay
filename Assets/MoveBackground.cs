using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : SynchronizeData
{
    private RectTransform m_RectTransform;

    private void Start()
    {
        m_RectTransform = gameObject.GetComponent<RectTransform>();
    }
    public override void Reflesh()
    {
        m_RectTransform.localPosition = new Vector3(0, -1920 * _coord.y / 10, 400);
    }
}
