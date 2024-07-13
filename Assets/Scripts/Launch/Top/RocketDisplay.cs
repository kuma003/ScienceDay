using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RocketDisplay : MonoBehaviour
{
    [SerializeField] float _rotateAngle = 30f;

    [SerializeField] float _rotateRate = 90f;

    private Vector3 rotateAxis = Vector3.zero;

    private void Start()
    {
        gameObject.GetComponent<Rocket>().zenith = _rotateAngle;
    }

    void Update()
    {
        gameObject.GetComponent<Rocket>().azimuth += _rotateRate * Time.deltaTime;
    }
}
